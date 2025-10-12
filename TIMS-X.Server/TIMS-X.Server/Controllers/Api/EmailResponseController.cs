using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Server.Config;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using TIMS_X.Server.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/EmailResponse")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EmailResponseController : Controller
    {
        private readonly IPatientMessagingService _messagingService;
        private readonly PracticeQuery _practiceQuery;
        private readonly SchedulerQuery _schedulerQuery;
        private readonly AppSettings _appSettings;

        public EmailResponseController(IPatientMessagingService messagingService, PracticeQuery practiceQuery, SchedulerQuery schedulerQuery, IConfiguration configuration)
        {
            _messagingService = messagingService;
            _practiceQuery = practiceQuery;
            _schedulerQuery = schedulerQuery;
            _appSettings = configuration.Get<AppSettings>();
        }

        [HttpGet("Logo")]
        public async Task<ActionResult> GetLogoAsync(string officeCode, int siteId)
        {
            var logo = (await _practiceQuery.GetSiteAsync(siteId))?.Logo;
            if (logo != null && logo.Length > 0)
            {
                return File(logo.ToArray(), "image/bmp");
            }
            return File("~/images/logo_full.png", "image/png");
        }


        public async Task<ActionResult> Index(string officeCode, string response)
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
            else if(template != null)
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
            
            ViewData["ResponseCode"] = emailResponse.ResponseCode;
            ViewData["Text"] = message;
            ViewData["SiteId"] = siteId;
            ViewData["OfficeCode"] = officeCode;
            return RedirectToPage("/ConfirmAppointment");
        }
    }
}
