using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Data;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    [Route("api/[controller]")]
    [ApiController]
#if Release
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    public class UserController : ControllerBase
    {

        private readonly IHubContext<UserManagementHub> _usermgmtHub;
#region Constructors

        public UserController(UserService userService, PracticeQuery practiceQuery, 
            IHubContext<UserManagementHub> usermgmtHub, UserSecurityService userSecurityService, ContextHelper contextHelper)
        {
            _userService = userService;
            _practiceQuery = practiceQuery;
            _usermgmtHub = usermgmtHub;
            _userSecurityService = userSecurityService;
            _contextHelper = contextHelper;
        }

#endregion Constructors

#region UserController Members

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticationForm form)
        {
            var officeCode = Request.Headers.GetValue(StringConstants.OfficeCode).ToString();
            if (string.IsNullOrWhiteSpace(officeCode))
            {
                return Unauthorized();
            }

            User user = null;

            if (await _practiceQuery.UsesAdAuthenticationAsync() && string.IsNullOrWhiteSpace(form.Password))
            {
                user = await _userService.AuthenticateApiUserAdAsync(form, officeCode);
            }
            else
            {
                user = await _userService.AuthenticateApiUserAsync(form, officeCode);
            }

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }


        [HttpPost("SendOTP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SendOneTimePasscodeAsync([FromBody] string code)
        {
            var result = await _userSecurityService.SendOneTimePasscodeAsync(code);
            return Ok();
        }

        [HttpGet("All")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetAllAsync(bool includeInactive)
        {
            var users = await _userService.GetAllAsync(includeInactive);
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("LoginUsers")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetLoginUsersAsync(bool includeInactive)
        {
            var users = await _userService.GetLoginUsersAsync(includeInactive);
            return Ok(users);
        }
        
        [HttpGet("RecentPatientsList")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetRecentPatientsListAsync(int userId)
        {
            var recentPatients = await _userService.GetRecentPatientsListAsync(userId);
            return Ok(recentPatients);
        }

        [HttpGet("AddRecentPatient")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AddRecentPatientAsync(int userId, int patientId)
        {
            if (userId < 0 || patientId <= 0)
                return BadRequest();
            await _userService.AddRecentPatientAsync(userId, patientId);
            return Ok();
        }

        [HttpGet("FirstUserId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetFirstUserIdAsync()
        {
            var userId = await _userService.GetFirstUserIdAsync();
            return Ok(userId);
        }

        [HttpGet("SignedInUsers")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetSignedInUsers()
        {
            var signedInUsers = UserManagementHub.GetSignedInUsers(_contextHelper.OfficeCode);
            return Ok(signedInUsers);
        }

        [HttpGet("ForceSignOutTims")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async  Task<IActionResult> ForceSignOutTimsAsync(int userId)
        {
            if(UserManagementHub.IsUserConnected(_contextHelper.OfficeCode, userId))
            {
                var userHubId = $"{_contextHelper.OfficeCode}-{userId}".ToLower();
                await _usermgmtHub.Clients.User(userHubId).SendAsync("ForceSignOut", _contextHelper.CurrentUser.Id);
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet("NotifySignedOutTims")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> NotifySignedOutTimsAsync(int userId)
        {
            if (UserManagementHub.IsUserConnected(_contextHelper.OfficeCode, userId))
            {
                var userHubId = $"{_contextHelper.OfficeCode}-{userId}".ToLower();
                await _usermgmtHub.Clients.User(userHubId).SendAsync("NotifySignedOut", _contextHelper.CurrentUser.Id);
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet("RequestSignOutTims")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RequestSignOutTimsAsync(int userId)
        {
            if (UserManagementHub.IsUserConnected(_contextHelper.OfficeCode, userId))
            {
                var userHubId = $"{_contextHelper.OfficeCode}-{userId}".ToLower();
                await _usermgmtHub.Clients.User(userHubId).SendAsync("RequestSignOut", _contextHelper.CurrentUser.Id);
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet("RefuseRequestSignOutTims")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CancelRequestSignOutTimsAsync(int requesterId)
        {
            if (UserManagementHub.IsUserConnected(_contextHelper.OfficeCode, requesterId))
            {
                var userHubId = $"{_contextHelper.OfficeCode}-{requesterId}".ToLower();
                await _usermgmtHub.Clients.User(userHubId).SendAsync("RefuseSignOut", _contextHelper.CurrentUser.Id);
                return Ok(true);
            }
            return Ok(false);
        }


#endregion UserController Members

#region Fields

        private readonly UserService _userService;
        private readonly UserSecurityService _userSecurityService;
        private readonly PracticeQuery _practiceQuery;
        private readonly ContextHelper _contextHelper;

        #endregion Fields

    }
}
