using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.Core.Enums;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/WebHooks")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WebHooksController : TwilioController
    {
        private readonly IPatientMessagingService _messagingService;
        private readonly ProviderQuery _providerQuery;

        public WebHooksController(IPatientMessagingService messagingService, ProviderQuery providerQuery)
        {
            _messagingService = messagingService;
            _providerQuery = providerQuery;
        }

        private TwiMLResult _GetCallScriptAsync(MessageTemplateType templateType, CallScriptTokens tokens, string officeCode, 
            string key, string callSid, bool isAnsweringMachine)
        {
            var response = new VoiceResponse();

            if (tokens == null)
            {
                response.Append(new Say("An error has occurred. Goodbye.", "Polly.Matthew", language: "en-US"));
            }
            else
            {
                response.Append(new Say(tokens.Message, tokens.Voice, language: tokens.Language));
                // Do not say the prompt to for verification messages or if an answering machine picked up
                if (templateType == MessageTemplateType.AppointmentConfirmation && !isAnsweringMachine)
                {
                    var action = new Uri(tokens.ResponseUrl);
                    var gather = new Gather(numDigits: 1, action: action)
                        .Say(tokens.VoicePromptText, tokens.Voice, language: tokens.Language);
                    response.Append(gather);
                }
            }

            
            
            return TwiML(response);
        }

        /// <summary>
        /// application/x-www-form-urlencoded
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="officeCode"></param>
        /// <param name="key"></param>
        /// <param name="callScriptRequest"></param>
        /// <returns></returns>
        [HttpPost("CallScript")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/xml")]

        public async Task<TwiMLResult> GetCallScriptAsync(
            [FromQuery] MessageTemplateType templateType,
            [FromQuery] string officeCode,
            [FromQuery] string key,
            [FromForm] TwilioCallScriptRequest callScriptRequest)
        {
            var callScriptTokens = await _messagingService.GetCallScriptTokensAsync(
                officeCode,
                key,
                callScriptRequest.CallSid);

            // With Twilio's Answering Machine Detection enabled, they try to detect the beep at the end of an answering machine message,
            // The following values are possible for AnsweredBy: machine_end_beep, machine_end_silence, machine_end_other, human, fax, unknown
            // Any of the machine_end values indicate we hit an answering machine.
            var isAnsweringMachine = !string.IsNullOrEmpty(callScriptRequest.AnsweredBy) &&
                                      callScriptRequest.AnsweredBy.StartsWith("machine_end");

            var response = _GetCallScriptAsync(templateType, callScriptTokens, officeCode, key, callScriptRequest.CallSid, isAnsweringMachine);
            return response;
        }

        [HttpPost("CallResponse")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/xml")]
        public async Task<TwiMLResult> PostCallResponseAsync([FromQuery] string officeCode, [FromQuery] string key,
            [FromForm] TwilioCallResponse callResponse)
        {
            var (patientResponse, template) = await _messagingService.HandlePatientVoiceNotificationResponseAsync(callResponse);
            if (template == null)
            {
                throw new Exception($"Could not find template associated with call sid {callResponse.CallSid}");
            }

            var callScriptTokens = await _messagingService.GetCallScriptTokensAsync(
                officeCode,
                key,
                callResponse.CallSid);

            switch (patientResponse)
            {
                case PatientNotificationResponse.Cancel:
                    return TwiML(new VoiceResponse().Append(new Say(template.VoiceCancelText, callScriptTokens.Voice, language: callScriptTokens.Language)));
                case PatientNotificationResponse.Confirm:
                    return TwiML(new VoiceResponse().Append(new Say(template.VoiceConfirmText, callScriptTokens.Voice, language: callScriptTokens.Language)));
                case PatientNotificationResponse.Reschedule:
                    return TwiML(new VoiceResponse().Append(new Say(template.VoiceRescheduleText, callScriptTokens.Voice, language: callScriptTokens.Language)));
                case PatientNotificationResponse.Repeat:
                    return _GetCallScriptAsync(template.TemplateType, callScriptTokens, officeCode, key, callResponse.CallSid, false);
                default: // invalid input
                    callScriptTokens.Message = template.VoiceMisunderstoodText;
                    return _GetCallScriptAsync(template.TemplateType, callScriptTokens, officeCode, key, callResponse.CallSid, false);
            }
            
            
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("CallStatus")]
        public async Task<ActionResult> PostCallStatusAsync([FromQuery] string officeCode, [FromQuery] string key,
            [FromForm] TwilioCallResponse callResponse)
        {
            await _messagingService.UpdateCallStatusAsync(callResponse.CallSid, callResponse.CallStatus, callResponse.Digits);
            return new OkResult();
        }


        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("SmsStatus")]
        public async Task<ActionResult> PostSmsStatusAsync([FromQuery] string officeCode, [FromQuery] string key,
            [FromForm] TwilioSmsStatus status)
        {
            await _messagingService.UpdateSmsStatusAsync(status);
            return new OkResult();
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("SmsResponse")]
        public async Task<ActionResult> PostSmsResponseAsync([FromQuery] string officeCode, [FromQuery] string key,
            [FromForm] TwilioSmsResponse smsResponse)
        {
            var (patientResponse, messageTemplate, patientId, appointmentId) = await _messagingService.HandlePatientSmsNotificationResponseAsync(smsResponse);
            if (messageTemplate != null)
            {
                var request = Request;
                string responseMessage = null;
                switch (patientResponse)
                {
                    case PatientNotificationResponse.Cancel:
                        responseMessage = messageTemplate.SmsCancelText;
                        break;
                    case PatientNotificationResponse.Confirm:
                        responseMessage = messageTemplate.SmsConfirmText;
                        break;
                    case PatientNotificationResponse.Reschedule:
                        responseMessage = messageTemplate.SmsRescheduleText;
                        break;
                    default: // invalid input
                        responseMessage = messageTemplate.SmsMisunderstoodText;
                        break;
                }
                if(!string.IsNullOrEmpty(smsResponse.From) && !string.IsNullOrEmpty(responseMessage))
                {
                    var userId = await _providerQuery.GetUserIdFromProviderAsync(messageTemplate.ProviderId);
                    await _messagingService.SendSmsReplyAsync(smsResponse.From, responseMessage, patientId, userId, messageTemplate.Id, appointmentId);
                }
            }
            
            return new OkResult();
        }
        
    }
}