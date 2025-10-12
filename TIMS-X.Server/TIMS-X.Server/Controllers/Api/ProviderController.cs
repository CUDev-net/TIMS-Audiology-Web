using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Legacy;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Filters;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ProviderController : ControllerBase
    {
        private readonly ProviderQuery _providerQuery;
        private readonly PracticeQuery _practiceQuery;

        private readonly IPatientMessagingService _patientMessagingService;
        public ProviderController(ProviderQuery providerQuery, PracticeQuery practiceQuery, IPatientMessagingService patientMessagingService)
        {
            _providerQuery = providerQuery;
            _practiceQuery = practiceQuery;
            _patientMessagingService = patientMessagingService;
        }

        [HttpGet("All")]
        public async Task<ActionResult> GetAllProvidersAsync(bool includeInactive)
        {
            var providers = await _providerQuery.GetProvidersAsync(includeInactive);
            return Ok(providers);
        }

        [HttpPut("MessageTemplate")]
        public async Task<ActionResult> PutMessageTemplateAsync(MessageTemplate messageTemplate)
        {
            await _providerQuery.PutMessageTemplateAsync(messageTemplate);
            return Ok();
        }

        [HttpGet("MessageTemplate")]
        public async Task<ActionResult> GetMessageTemplateAsync(int providerId, MessageTemplateType templateType, LanguageEnum language)
        {
            var template = await _providerQuery.GetMessageTemplateAsync(providerId, templateType, language);
            return Ok(template);
        }

        [HttpGet("FromUserId")]
        public async Task<ActionResult> GetProviderFromUserIdAsync(int userId)
        {
            var provider = await _providerQuery.GetProviderFromUserIdAsync(userId);
            return Ok(provider);
        }
    }
}