using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Legacy;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Filters;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PatientSchedulerController : ControllerBase
    {
        private readonly SchedulerQuery _schedulerQuery;
        private readonly ProviderQuery _providerQuery;
        private readonly PracticeQuery _practiceQuery;
        private readonly SchedulerService _schedulerService;

        private readonly IPatientMessagingService _patientMessagingService;
        public PatientSchedulerController(SchedulerQuery schedulerQuery, PracticeQuery practiceQuery, ProviderQuery providerQuery, SchedulerService schedulerService,
            IPatientMessagingService patientMessagingService)
        {
            _schedulerQuery = schedulerQuery;
            _practiceQuery = practiceQuery;
            _providerQuery = providerQuery;
            _schedulerService = schedulerService;
            _patientMessagingService = patientMessagingService;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("Timeslots")]
        public async Task<ActionResult> GetTimeslotsAsync(int siteId, int providerId, DateTime weekBegin, string officeCode)
        {
            var filter = new SchedulerQueryFilter
            {
                From = weekBegin.Date,
                To = weekBegin.ToEndOfDay().AddDays(6),
                ProviderIds = new List<int> { providerId },
                SiteIds = new List<int> { siteId },
                ApptTypeIds = new List<int>(),
                ResourceIds = new List<int>(),
                SpecialtyIds = new List<int>()
            };

            var openings = await _schedulerService.FindAppointmentOpeningsAsync(filter, 30);
            return Ok(openings);
        }

        [HttpGet("Dates")]
        public async Task<ActionResult> GetDatesWithAppointmentsAsync(int year, int month)
        {
            var filter = new SchedulerQueryFilter
            {
                From = new DateTime(year, month, 1),
                To = new DateTime(year, month, DateTime.DaysInMonth(year, month)).Date.ToEndOfDay()
            };
            var dates = await _schedulerQuery.GetDaysScheduledAsync(filter);
            return Ok(dates);
        }

        [HttpGet("Resources")]
        public async Task<ActionResult> GetResourcesAsync(bool includeInactive)
        {
            var resources = await _schedulerQuery.GetResourcesAsync(includeInactive);
            return Ok(resources);
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("Sites")]
        public async Task<ActionResult> GetSitesAsync(string officeCode)
        {
            var sites = await _practiceQuery.GetSitesAsync(false);
            return Ok(sites.Select(site => new NamedEntity {Id = site.Id, Name = site.Name }));
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("Providers")]
        public async Task<ActionResult> GetProvidersAsync(int siteId, string officeCode)
        {
            var providers = await _providerQuery.GetProvidersAsync(siteId, false);
            return Ok(providers.Select(p => new NamedEntity { Id = p.Id, Name = p.LastFirstMiddle }));
        }

        /// <summary>
        /// Returns a list of all patients scheduled in the provided date range at the provided sites.
        /// If no sites (sid) are provided, all sites are queried.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpGet("PatientsScheduled")]
        public async Task<ActionResult> GetPatientsScheduledAsync(DateTime from, DateTime to, int[] sid)
        {

            var patientItems = await _schedulerQuery.GetPatientsScheduledAsync(from, to, sid);
            return Ok(patientItems);
        }

        [HttpGet("PatientsScheduledToday")]
        public async Task<ActionResult> GetPatientsScheduledTodayAsync(int[] providerIds)
        {
            var patientItems = await _schedulerQuery.GetPatientsScheduledTodayAsync(providerIds);
            return Ok(patientItems);
        }

        [HttpGet("SendPatientConfirmation")]
        public async Task<ActionResult> SendPatientConfirmationAsync(int appointmentId, MessageDeliveryMethod deliveryMethod)
        {
            var result = await _patientMessagingService.SendConfirmationMessageAsync(appointmentId, deliveryMethod);
            
            return Ok(result);
        }

        [HttpGet("SendPatientVerification")]
        public async Task<ActionResult> SendPatientVerificationAsync(int appointmentId, MessageDeliveryMethod deliveryMethod)
        {
            var result = await _patientMessagingService.SendVerificationMessageAsync(appointmentId, deliveryMethod);
            return Ok(result);
        }


        [HttpGet("PreviewPatientNotification")]
        public async Task<ActionResult> PreviewPatientNotificationAsync(MessageDeliveryMethod deliveryMethod, int templateId, int siteId, string contact)
        {
            var errors = await _patientMessagingService.PreviewPatientNotificationAsync(deliveryMethod, templateId, siteId, contact);

            return Ok(errors);
        }

        [HttpGet("IsPreviewFinished")]
        public async Task<ActionResult> IsPreviewFinished(int templateId)
        {
            var isFinished = await _patientMessagingService.IsPreviewFinishedAsync(templateId);
            return Ok(isFinished);
        }

        [HttpPost("CreateAppointment")]
        public async Task<ActionResult> CreateAppointmentAsync(AppointmentCreateModel appointment)
        {
            await _schedulerService.CreateAppointmentAsync(appointment);
            return Ok(appointment.Id);
        }

        [HttpPut("UpdateAppointment")]
        public async Task<ActionResult> UpdateAppointmentAsync([FromQuery] bool ignoreWarnings, [FromBody] AppointmentUpdateModel appointment)
        {
            var result = await _schedulerService.UpdateAppointmentAsync(appointment, ignoreWarnings);
            return Ok(result);
        }

        [HttpDelete("DeleteAppointment")]
        public async Task<ActionResult> DeleteAppointmentAsync(int appointmentId)
        {
            await _schedulerService.DeleteAppointmentAsync(appointmentId);
            return Ok();
        }

        [HttpGet("GetNotificationResult")]
        public async Task<ActionResult> GetNotificationResultAsync([FromQuery] MessageTemplateType templateType,
            [FromQuery] MessageDeliveryMethod deliveryMethod, [FromQuery] int appointmentId)
        {
            var notificationResult = await _schedulerService.GetNotificationResultAsync(templateType, deliveryMethod, appointmentId);
            return Ok(notificationResult);
        }

        [HttpGet("GetNotificationResults")]
        public async Task<ActionResult> GetNotificationResultsAsync([FromQuery] MessageTemplateType templateType,
            [FromQuery] MessageDeliveryMethod[] types, [FromQuery] int[] apptIds)
        {
            var notificationResults = await _schedulerService.GetNotificationResultsAsync(templateType, types, apptIds);
            return Ok(notificationResults);
        }
        

    }
}