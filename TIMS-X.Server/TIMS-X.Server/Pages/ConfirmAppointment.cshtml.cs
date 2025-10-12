using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TIMS_X.Core.Enums;
using TIMS_X.Server.Config;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Pages
{
    public class ConfirmAppointmentModel : PageModel
    {

        private readonly AppSettings _appSettings;
        private readonly IPatientMessagingService _messagingService;
        private readonly PatientService _patientService;
        private readonly ProviderQuery _providerQuery;
        public ConfirmAppointmentModel(IConfiguration configuration, IPatientMessagingService patientMessagingService, PatientService patientService, ProviderQuery providerQuery)
        {
            _appSettings = configuration.Get<AppSettings>();
            _messagingService = patientMessagingService;
            _patientService = patientService;
            _providerQuery = providerQuery;
        }



        [BindProperty]
        public string Text { get; set; }
        [BindProperty]
        public PatientNotificationResponse ResponseCode { get; set; }

        public async Task<IActionResult> OnGetAsync(string officeCode, string response)
        {

            var base64EncodedBytes = System.Convert.FromBase64String(response);
            var encryptedString = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            var decrypted = CryptographyHelper.Decrypt(encryptedString, _appSettings.Keys.ImagingKey);
            var emailResponse = JsonConvert.DeserializeObject<EmailResponseParameters>(decrypted);

            var (template, siteId, wasModified, patientId) = await _messagingService.HandlePatientEmailNotificationResponseAsync(emailResponse.ResponseCode, emailResponse.LogId);

            string message = null;
            if (!wasModified)
            {
                message = template.EmailMisunderstoodText;
            }
            else if (template != null)
            {
                switch (emailResponse.ResponseCode)
                {
                    case PatientNotificationResponse.Cancel:
                        message = template.EmailCancelText;
                        break;
                    case PatientNotificationResponse.Confirm:
                        message = template.EmailConfirmText;
                        break;
                    case PatientNotificationResponse.Reschedule:
                        message = template.EmailRescheduleText;
                        break;
                    default: // invalid input
                        message = template.EmailMisunderstoodText;
                        break;
                }
            }

            if (message.Contains("{DigitalIntakeFormLink}"))
            {
                string intakeLink = null;

                if (patientId > 0 && template !=  null)
                {
                    var userId = await _providerQuery.GetUserIdFromProviderAsync(template.ProviderId);
                    var (success, link) = await _patientService.GetFormLinkAsync(PatientFormTypeEnum.Intake, patientId, userId);
                    // Append a space, otherwise any text after the link will be concatenated and the link will fail.
                    if (success) intakeLink = $"<a href=\"{link}\">{link}</a>";
                }

                message = message.Replace("{DigitalIntakeFormLink}", intakeLink);
            }

            Text = message;
            ResponseCode = emailResponse.ResponseCode;

            return Page();
        }

    }
}