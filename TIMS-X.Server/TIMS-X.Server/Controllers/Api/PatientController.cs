using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Legacy;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly IPatientMessagingService _messagingService;
        private readonly ContextHelper _contextHelper;
        private readonly ILogger<PatientController> _logger;
        public PatientController(PatientService patientService, IPatientMessagingService messagingService, ContextHelper contextHelper, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _messagingService = messagingService;
            _contextHelper = contextHelper;
            _logger = logger;
        }

        [HttpGet("Exists")]
        public async Task<ActionResult> ExistsAsync([FromQuery] int id)
        {
            var exists = await _patientService.ExistsAsync(id);
            return Ok(exists);
        }

        [HttpGet("FormLink")]
        public async Task<ActionResult> GetFormLinkAsync([FromQuery] PatientFormTypeEnum formType, [FromQuery] int patientId)
        {
            var link = await _patientService.GetFormLinkAsync(formType, patientId, _contextHelper.CurrentUser.Id);
            return Ok(link);
        }



        [HttpGet("Search")]
        public async Task<ActionResult> SearchAsync(string query, SearchType searchType, bool includeInactive)
        {
            var patients = await _patientService.SearchAsync(query, searchType, includeInactive);
            return Ok(patients);
        }

        [HttpPost("UpdatePatient")]
        public async Task<ActionResult> UpdatePatientAsync([FromQuery] int patientId, [FromBody] PatientUpdateModel patient)
        {
            await _patientService.UpdatePatientAsync(patientId, patient);
            return Ok();
        }

        [HttpPost("Send-Messages")]
        public async Task<ActionResult> SendMessagesAsync(PatientMessageModel model)
        {
            _logger.LogInformation("api/Patient/Send-Messages executing...");
            await _messagingService.SendPatientMessagesAsync(model);
            return Ok();
        }

        [HttpGet("ConversationHistoryMessageCount")]
        public async Task<ActionResult> GetConversationHistoryMessageCountAsync(int userId, int patientId)
        {
            var conversationHistoryMessageCount = await _messagingService.GetConversationHistoryMessageCountAsync(userId, patientId);
            return Ok(conversationHistoryMessageCount);
        }


        [HttpGet("ConversationHistoryMessageCountAll")]
        public async Task<ActionResult> GetConversationHistoryMessageCountAllAsync(int patientId)
        {
            var conversationHistoryMessageCount = await _messagingService.GetConversationHistoryMessageCountAllAsync(patientId);
            return Ok(conversationHistoryMessageCount);
        }

        [HttpGet("ConversationHistory")]
        public async Task<ActionResult> GetConversationHistoryAsync([FromQuery] int userId, [FromQuery] int patientId, [FromQuery] string phoneNumber, [FromQuery] DateTime? cutoffDate)
        {
            return Ok(patientId > 0 
                ? await _messagingService.GetConversationHistoryAsync(userId, patientId, cutoffDate) 
                : await _messagingService.GetConversationHistoryAsync(userId, phoneNumber, cutoffDate));
        }

        [HttpGet("AllConversations")]
        public async Task<ActionResult> GetAllConversationsAsync()
        {
            return Ok(await _messagingService.GetAllConversationsAsync());
        }

        [HttpGet("AllConversationsPaged")]
        public async Task<ActionResult> GetAllConversationsPagedAsync(int page, int pageSize)
        {
            return Ok(await _messagingService.GetAllConversationsPagedAsync(page, pageSize));
        }

        [HttpGet("AllConversationsCount")]
        public async Task<ActionResult> GetAllConversationsCountAsync()
        {
            return Ok(await _messagingService.GetAllConversationsCountAsync());
        }
    }
}
