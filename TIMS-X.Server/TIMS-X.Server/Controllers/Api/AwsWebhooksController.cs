using System;
using System.IO;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Enums;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/AwsWebHooks")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class AwsWebHooksController : Controller
    {
        private readonly IPatientMessagingService _messagingService;
        private readonly ProviderQuery _providerQuery;
        private readonly ILogger<AwsWebHooksController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AwsWebHooksController(IPatientMessagingService messagingService, ProviderQuery providerQuery, ILogger<AwsWebHooksController> logger,
            IHttpContextAccessor contextAccessor)
        {
            _messagingService = messagingService;
            _providerQuery = providerQuery;
            _logger = logger;
            _httpContextAccessor = contextAccessor;
        }

        [HttpPost("SmsResponse")]
        public async Task<ActionResult> PostSmsResponseAsync()
        {
            var requestBody = string.Empty;

            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            _logger.LogInformation($"Request body: {requestBody}");

            var messageType = _httpContextAccessor.HttpContext.Request.Headers["x-amz-sns-message-type"];
            if (string.IsNullOrEmpty(messageType))
            {
                _logger.LogInformation("Missing message type header");
                return BadRequest();
            }
            _logger.LogInformation($"Message Type: {messageType}");
            if (messageType == "SubscriptionConfirmation")
            {
                try
                {
                    // Follow SubscribeURL to confirm subscription
                    var subscriptionConfirmation = JsonConvert.DeserializeObject<SubscriptionConfirmation>(requestBody);
                    _logger.LogInformation($"Auto-confirming subscription at url: {subscriptionConfirmation.SubscribeURL}");

                    using var client = new HttpClient();
                    using var response = await client.GetAsync(subscriptionConfirmation.SubscribeURL);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation($"Response: {content}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }


            }
            else if (messageType == "Notification")
            {
                try
                {
                    var notification = JsonConvert.DeserializeObject<AwsSmsNotification>(requestBody);
                    _logger.LogInformation($"Notification received.");
                    _logger.LogInformation($"From: {notification.OriginationNumber}");
                    _logger.LogInformation($"Message: {notification.MessageBody}");
                    var (patientResponse, messageTemplate, patientId, appointmentId) = await _messagingService.HandlePatientSmsNotificationResponseAsync(notification);
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
                        if (!string.IsNullOrEmpty(notification.OriginationNumber) && !string.IsNullOrEmpty(responseMessage))
                        {
                            var userId = await _providerQuery.GetUserIdFromProviderAsync(messageTemplate.ProviderId);
                            await _messagingService.SendSmsReplyAsync(notification.OriginationNumber, responseMessage, patientId, userId, messageTemplate.Id, appointmentId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            else
            {
                _logger.LogInformation("Unknown message type");
                return BadRequest();
            }

            return new OkResult();
        }


    }
}