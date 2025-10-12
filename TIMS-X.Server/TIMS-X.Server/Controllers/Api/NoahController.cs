using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Noah;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using TIMS_X.Server.Models.Noah;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NoahController : ControllerBase
    {
        private readonly NoahService _noahService;
        
        public NoahController(NoahService noahService)
        {
            _noahService = noahService;
        }

        [HttpGet("NHAX")]
        public async Task<ActionResult> GetNHAXAsync([FromQuery] int patientId)
        {
            var url = $"{this.Request.Scheme}://{this.Request.Host}";
            var data = await _noahService.GetNHAXAsync(patientId, url);
            return Ok(data);
        }

        [HttpPut("ArchiveAction")]
        public async Task PutArchiveActionAsync([FromBody] int actionId)
        {
            await _noahService.ArchiveActionAsync(actionId);
        }

        [HttpPut("ArchiveUnboundAction")]
        public async Task PutArchiveUnboundActionAsync([FromBody]int actionId)
        {
            await _noahService.ArchiveUnboundActionAsync(actionId);
        }

        [HttpDelete("AppPermissions")]
        public async Task DeleteAppPermissionsAsync([FromQuery] int[] permissionIds)
        {
            await _noahService.DeleteAppPermissionsAsync(permissionIds);
        }

        [HttpDelete("DashboardAlert")]
        public async Task DeleteDashboardAlertAsync([FromQuery] Guid alertGuid)
        {
            await _noahService.DeleteDashboardAlertAsync(alertGuid);
        }

        [HttpDelete("DashboardAlertArchive")]
        public async Task DeleteDashboardAlertArchiveAsync([FromQuery] Guid alertGuid)
        {
            await _noahService.DeleteDashboardAlertArchiveAsync(alertGuid);
        }


        [HttpDelete("Action")]
        public async Task DeleteActionAsync([FromQuery] int actionId)
        {
            await _noahService.DeleteActionAsync(actionId);
        }

        [HttpDelete("Preference")]
        public async Task DeletePreferenceAsync([FromQuery] int preferenceId)
        {
            await _noahService.DeletePreferenceAsync(preferenceId);
        }

        [HttpDelete("ActionReferences")]
        public async Task DeleteActionReferencesAsync([FromQuery] int actionId)
        {
            await _noahService.DeleteActionReferencesAsync(actionId);
        }

        [HttpDelete("ArchivedActions")]
        public async Task DeleteArchivedActionsAsync([FromQuery] int actionId)
        {
            await _noahService.DeleteArchivedActionsAsync(actionId);
        }

        [HttpDelete("ArchivedUnboundActions")]
        public async Task DeleteArchivedUnboundActionsAsync([FromQuery] int actionId)
        {
            await _noahService.DeleteArchivedUnboundActionsAsync(actionId);
        }

        [HttpDelete("FastView")]
        public async Task DeleteFastViewAsync([FromQuery] int actionId)
        {
            await _noahService.DeleteFastViewAsync(actionId);
        }

        [HttpDelete("MobileAppPermissions")]
        public async Task DeleteMobileAppPermissionsAsync([FromQuery] int moduleId, [FromQuery] int[] permissionIds)
        {
            await _noahService.DeleteMobileAppPermissionsAsync(moduleId, permissionIds);
        }

        [HttpDelete("PatientIdentification")]
        public async Task DeletePatientIdentificationAsync([FromQuery] int patientId, [FromQuery] int manufacturerId)
        {
            await _noahService.DeletePatientIdentificationAsync(patientId, manufacturerId);
        }

        [HttpDelete("PatientSetup")]
        public async Task DeletePatientSetupAsync([FromQuery] int patientId, [FromQuery] int moduleId)
        {
            await _noahService.DeletePatientSetupAsync(patientId, moduleId);
        }

        [HttpDelete("Session")]
        public async Task DeleteSessionAsync([FromQuery] int sessionId)
        {
            await _noahService.DeleteSessionAsync(sessionId);
        }

        [HttpDelete("UnboundAction")]
        public async Task DeleteUnboundActionAsync([FromQuery] int unboundActionId)
        {
            await _noahService.DeleteUnboundActionAsync(unboundActionId);
        }

        [HttpDelete("UnboundActionReferences")]
        public async Task DeleteUnboundActionReferences([FromQuery] int actionId)
        {
            await _noahService.DeleteUnboundActionReferencesAsync(actionId);
        }

        [HttpDelete("UserSetup")]
        public async Task DeleteUserSetup([FromQuery] int userId, [FromQuery] int moduleId)
        {
            await _noahService.DeleteUserSetupAsync(userId, moduleId);
        }

        [AllowAnonymous]
        [HttpPost("ValidateLogin")]
        public async Task<ActionResult> ValidateLoginAsync(N4Login login)
        {
            var userId = await _noahService.ValidateLoginAsync(login.Username, login.Password);
            return Ok(userId);
        }

        [HttpGet("Action")]
        public async Task<ActionResult> GetActionAsync([FromQuery] int actionId)
        {
            var action = await _noahService.GetActionAsync(actionId);
            return Ok(action);
        }

        [HttpGet("Action2")]
        public async Task<ActionResult> GetActionAsync([FromQuery] Guid actionGuid)
        {
            var action = await _noahService.GetActionAsync(actionGuid);
            return Ok(action);
        }

        [HttpGet("ActionReferencedBy")]
        public async Task<ActionResult> GetActionReferencedByAsync([FromQuery] int actionId)
        {
            var action = await _noahService.GetActionReferencedByAsync(actionId);
            return Ok(action);
        }

        [HttpGet("ActionCollection")]
        public async Task<ActionResult> GetActionCollectionAsync([FromQuery] int moduleId)
        {
            var actions = await _noahService.GetActionCollectionAsync(moduleId);
            return Ok(actions);
        }

        [HttpGet("ActionCollectionCount")]
        public async Task<ActionResult> GetActionCollectionCountAsync([FromQuery] int moduleId)
        {
            var count = await _noahService.GetActionCollectionCountAsync(moduleId);
            return Ok(count);
        }

        [HttpGet("ActionData")]
        public async Task<ActionResult> GetActionDataAsync([FromQuery] int actionId)
        {
            var actionData = await _noahService.GetActionDataAsync(actionId);
            return Ok(actionData);
        }

        [HttpGet("ActionReferences")]
        public async Task<ActionResult> GetActionReferencesAsync([FromQuery] int actionId)
        {
            var actionReferences = await _noahService.GetActionReferencesAsync(actionId);
            return Ok(actionReferences);
        }

        [HttpGet("Actions")]
        public async Task<ActionResult> GetActionsAsync([FromQuery] int sessionId)
        {
            var timer = new Stopwatch();
            timer.Start();
            var actions = await _noahService.GetActionsAsync(sessionId);
            timer.Stop();
            Console.WriteLine($"GetActionsAsync(sessionId: {sessionId}) took {timer.Elapsed.TotalSeconds} secs.");
            return Ok(actions);
        }

        [HttpGet("SearchActions")]
        public async Task<ActionResult> SearchActionsAsync([FromQuery] int patientId, [FromQuery] DateTime? actionGroup, [FromQuery] bool returnLatest, [FromQuery] int[] dataTypes, [FromQuery] int[] moduleIds)
        {
            var actions = await _noahService.SearchActionsAsync(patientId, actionGroup, returnLatest, dataTypes, moduleIds);
            return Ok(actions);
        }

        [HttpGet("AppPermissions")]
        public async Task<ActionResult> GetAppPermissionsAsync()
        {
            var appPermissions = await _noahService.GetAppPermissionsAsync();
            return Ok(appPermissions);
        }

        [HttpGet("ArchivedActions")]
        public async Task<ActionResult> GetArchivedActionsAsync([FromQuery] int actionId)
        {
            var archivedActions = await _noahService.GetArchivedActionsAsync(actionId);
            return Ok(archivedActions);
        }

        [HttpGet("ArchivedActionsCount")]
        public async Task<ActionResult> GetArchivedActionsCountAsync([FromQuery] int actionId)
        {
            var archivedActionsCount = await _noahService.GetArchivedActionsCountAsync(actionId);
            return Ok(archivedActionsCount);
        }

        [HttpGet("ArchivedUnboundActions")]
        public async Task<ActionResult> GetArchivedUnboundActionsAsync([FromQuery] int actionId)
        {
            var archivedUnboundActions = await _noahService.GetArchivedUnboundActionsAsync(actionId);
            return Ok(archivedUnboundActions);
        }

        [HttpGet("ArchivedUnboundActionsCount")]
        public async Task<ActionResult> GetArchivedUnboundActionsCountAsync([FromQuery] int actionId)
        {
            var archivedUnboundActionsCount = await _noahService.GetArchivedUnboundActionsCountAsync(actionId);
            return Ok(archivedUnboundActionsCount);
        }

        [HttpGet("FastView")]
        public async Task<ActionResult> GetFastViewAsync([FromQuery] int actionId)
        {
            var fastView = await _noahService.GetFastViewAsync(actionId);
            return Ok(fastView);
        }

        [HttpGet("ManufacturerSetups")]
        public async Task<ActionResult> GetManufacturerSetupsAsync([FromQuery] int manufacturerId, [FromQuery] string[] manufacturerKeys)
        {
            var manufacturerSetups = await _noahService.GetManufacturerSetupsAsync(manufacturerId, manufacturerKeys);
            return Ok(manufacturerSetups);
        }

        [HttpGet("ManufacturerSetupKeys")]
        public async Task<ActionResult> GetManufacturerSetupKeysAsync([FromQuery] int manufacturerId)
        {
            var manufacturerSetupKeys = await _noahService.GetManufacturerSetupKeysAsync(manufacturerId);
            return Ok(manufacturerSetupKeys);
        }

        [HttpGet("MobileAppPermissions")]
        public async Task<ActionResult> GetMobileAppPermissionsAsync([FromQuery] int moduleId)
        {
            var mobileAppPermissions = await _noahService.GetMobileAppPermissionsAsync(moduleId);
            return Ok(mobileAppPermissions);
        }

        [HttpGet("MobileApps")]
        public async Task<ActionResult> GetMobileAppsAsync()
        {
            var mobileApps = await _noahService.GetMobileAppsAsync();
            return Ok(mobileApps);
        }

        [HttpGet("ConnectionInfo")]
        public ActionResult GetConnectionInfo()
        {
            var connectionInfo = _noahService.GetConnectionInfo();
            return Ok(connectionInfo);
        }

        [HttpGet("Patient")]
        public async Task<ActionResult> GetPatientAsync([FromQuery] int patientId)
        {
            var patient = await _noahService.GetPatientAsync(patientId);
            return Ok(patient);
        }

        [HttpGet("Patients")]
        public async Task<ActionResult> GetPatientsAsync([FromQuery] string searchText, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var patients = await _noahService.GetPatientsAsync(searchText, page, pageSize);
            return Ok(patients);
        }

        [HttpGet("PatientsCount")]
        public async Task<ActionResult> GetPatientsCountAsync([FromQuery] string searchText)
        {
            var patientCount = await _noahService.GetPatientsCountAsync(searchText);
            return Ok(patientCount);
        }

        [HttpGet("PatientSetup")]
        public async Task<ActionResult> GetPatientSetupAsync([FromQuery] int patientId, [FromQuery] int moduleId)
        {
            var patientSetup = await _noahService.GetPatientSetupAsync(patientId, moduleId);
            return Ok(patientSetup);
        }

        [HttpGet("PatientSetups")]
        public async Task<ActionResult> GetPatientSetupsAsync([FromQuery] int patientId, [FromQuery] int moduleId)
        {
            var patientSetups = await _noahService.GetPatientSetupsAsync(patientId, moduleId);
            return Ok(patientSetups);
        }

        [HttpGet("PatientSetupsForModule")]
        public async Task<ActionResult> GetPatientSetupsForModuleAsync([FromQuery] int moduleId)
        {
            var patientSetups = await _noahService.GetPatientSetupsForModuleAsync(moduleId);
            return Ok(patientSetups);
        }

        [HttpGet("PatientIdsFromIdentification")]
        public async Task<ActionResult> GetPatientIdsFromIdentificationAsync([FromQuery] string identification, [FromQuery] int manufacturerId)
        {
            var patientIds = await _noahService.GetPatientIdsFromIdentificationAsync(identification, manufacturerId);
            return Ok(patientIds);
        }



        [HttpGet("Preference")]
        public async Task<ActionResult> GetPreferenceAsync([FromQuery] int preferenceId)
        {
            var preference = await _noahService.GetPreferenceAsync(preferenceId);
            return Ok(preference);
        }

        [HttpGet("Session")]
        public async Task<ActionResult> GetSessionAsync([FromQuery] int sessionId)
        {
            var session = await _noahService.GetSessionAsync(sessionId);
            return Ok(session);
        }

        [HttpGet("Sessions")]
        public async Task<ActionResult> GetSessionsAsync([FromQuery] int patientId)
        {
            var sessions = await _noahService.GetSessionsAsync(patientId);
            return Ok(sessions);
        }

        [HttpGet("UnboundAction")]
        public async Task<ActionResult> GetUnboundActionAsync([FromQuery] int actionId)
        {
            var unboundAction = await _noahService.GetUnboundActionAsync(actionId);
            return Ok(unboundAction);
        }

        [HttpGet("UnboundActionData")]
        public async Task<ActionResult> GetUnboundActionDataAsync([FromQuery] int actionId)
        {
            var unboundActionData = await _noahService.GetUnboundActionDataAsync(actionId);
            return Ok(unboundActionData);
        }

        [HttpGet("UnboundActionIds")]
        public async Task<ActionResult> GetUnboundActionIdsAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var unboundActionIds = await _noahService.GetUnboundActionIdsAsync(startDate, endDate);
            return Ok(unboundActionIds);
        }

        [HttpGet("UnboundActionReferences")]
        public async Task<ActionResult> GetUnboundActionReferencesAsync([FromQuery] int actionId)
        {
            var unboundActionReferences = await _noahService.GetUnboundActionReferencesAsync(actionId);
            return Ok(unboundActionReferences);
        }

        [HttpGet("UnboundActions")]
        public async Task<ActionResult> GetUnboundActionsAsync([FromQuery] int[] actionIds)
        {
            var unboundActionReferences = await _noahService.GetUnboundActionsAsync(actionIds);
            return Ok(unboundActionReferences);
        }

        [HttpGet("UpdatedActions")]
        public async Task<ActionResult> GetUpdatedActionsAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] DateTime startTime,
           [FromQuery] DateTime? endTime, [FromQuery] short[] dataTypes)
        {
            var updatedActions = await _noahService.GetUpdatedActionsAsync(page, pageSize, startTime, endTime, dataTypes);
            return Ok(updatedActions);
        }

        [HttpGet("UpdatedPatients")]
        public async Task<ActionResult> GetUpdatedPatientsAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] DateTime startTime,
            [FromQuery] DateTime? endTime)
        {
            var updatedPatients = await _noahService.GetUpdatedPatientsAsync(page, pageSize, startTime, endTime);
            return Ok(updatedPatients);
        }

        [HttpGet("ActionIdsFromDashboardGroup")]
        public async Task<ActionResult> GetActionIdsFromDashboardGroupAsync([FromQuery] Guid group)
        {
            var result = await _noahService.GetActionIdsFromDashboardGroupAsync(group);
            return Ok(result);
        }


        [HttpGet("User")]
        public async Task<ActionResult> GetUserAsync([FromQuery] int? userId = 0, [FromQuery] string username = "")
        {
            N4User user = null;
            if (userId.HasValue && userId.Value > 0)
            {
                user = await _noahService.GetUserAsync(userId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(username))
            {
                user = await _noahService.GetUserAsync(username);
            }
            return Ok(user);
        }

        [HttpGet("Users")]
        public async Task<ActionResult> GetUsersAsync()
        {
            var user = await _noahService.GetUsersAsync();
            return Ok(user);
        }

        [HttpGet("UserPrivilege")]
        public async Task<ActionResult> GetUserPrivilegeAsync([FromQuery] int userId)
        {
            var userPrivilege = await _noahService.GetUserPrivilegeAsync(userId);
            var result = userPrivilege == 1;
            return Ok(result);
        }

        [HttpGet("UserSetup")]
        public async Task<ActionResult> GetUserSetupAsync([FromQuery] int userId, [FromQuery] int moduleId)
        {
            var userSetup = await _noahService.GetUserSetupAsync(userId, moduleId);
            return Ok(userSetup);
        }

        [HttpPut("Action")]
        public async Task<ActionResult> PutActionAsync(N4Action action)
        {
            await _noahService.PutActionAsync(action);
            return Ok(new IdPair { Id = action.Id, Guid = action.ActionGuid });
        }

        [HttpPut("ActionReferences")]
        public async Task PutActionReferencesAsync([FromQuery] int actionId, [FromBody] int[] actionReferences)
        {
            await _noahService.PutActionReferencesAsync(actionId, actionReferences);
        }

        [HttpPut("AppPermissions")]
        public async Task PutAppPermissionsAsync(ArrayWrapper<N4AppPermission> appPermissions)
        {
            await _noahService.PutAppPermissionsAsync(appPermissions.Values);
        }

        [HttpPut("ManufacturerSetups")]
        public async Task PutManufacturerSetupsAsync(ArrayWrapper<N4ManufacturerSetup> manufacturerSetups)
        {
            await _noahService.PutManufacturerSetupsAsync(manufacturerSetups.Values);
        }

        [HttpPut("MobileAppPermissions")]
        public async Task PutMobileAppPermissionsAsync([FromQuery] int moduleId, [FromBody] int[] permissionIds)
        {
            await _noahService.PutMobileAppPermissionsAsync(moduleId, permissionIds);
        }

        [HttpPut("PatientIdentification")]
        public async Task PutPatientIdentificationAsync(N4PatientIdentification patientIdentification)
        {
            await _noahService.PutPatientIdentificationAsync(patientIdentification);
        }

        [HttpPut("PatientSetup")]
        public async Task PutPatientSetupAsync(N4PatientSetup patientSetup)
        {
            await _noahService.PutPatientSetupAsync(patientSetup);
        }

        [HttpPut("Session")]
        public async Task<ActionResult> PutSessionAsync(N4Session session)
        {
            await _noahService.PutSessionAsync(session);
            return Ok(session.Id);
        }

        [HttpPut("UnboundAction")]
        public async Task<IActionResult> PutUnboundActionAsync(N4UnboundAction unboundAction)
        {
            await _noahService.PutUnboundActionAsync(unboundAction);
            return Ok(new IdPair { Id = unboundAction.Id, Guid = unboundAction.ActionGuid });
        }

        [HttpPut("UnboundActionReferences")]
        public async Task PutUnboundActionReferencesAsync([FromQuery] int unboundActionId, int[] unboundActionReferences)
        {
            await _noahService.PutUnboundActionReferencesAsync(unboundActionId, unboundActionReferences);
        }

        [HttpPut("UserSetup")]
        public async Task PutUserSetupAsync(N4UserSetup userSetup)
        {
            await _noahService.PutUserSetupAsync(userSetup);
        }

        [HttpPut("RegisterMobileApp")]
        public async Task RegisterMobileAppAsync(N4MobileApp mobileApp)
        {
            await _noahService.RegisterMobileAppAsync(mobileApp);
        }
        [HttpDelete("UnregisterMobileApp")]
        public async Task UnregisterMobileAppAsync([FromQuery] int moduleId)
        {
            await _noahService.UnregisterMobileAppAsync(moduleId);
        }

        [HttpPut("Preference")]
        public async Task PutPreferenceAsync([FromQuery] int preferenceId, [FromBody] byte[] preference)
        {
            await _noahService.PutPreferenceAsync(preferenceId, preference);
        }

        [HttpPut("DashboardAlert")]
        public async Task PutDashboardAlertAsync(N4DashboardAlert dashboardAlert)
        {
            await _noahService.PutDashboardAlertAsync(dashboardAlert);
        }


        [HttpPut("DashboardAlertArchive")]
        public async Task PutDashboardAlertArchiveAsync(N4DashboardAlertArchive dashboardAlert)
        {
            await _noahService.PutDashboardAlertArchiveAsync(dashboardAlert);
        }

        [HttpGet("DashboardAlert")]
        public async Task<ActionResult> GetDashboardAlertAsync([FromQuery] Guid alertGuid)
        {
            var result = await _noahService.GetDashboardAlertAsync(alertGuid);
            return Ok(result);
        }


        [HttpGet("DashboardAlertArchive")]
        public async Task<ActionResult> GetDashboardAlertArchiveAsync([FromQuery] Guid alertGuid)
        {
            var result = await _noahService.GetDashboardAlertArchiveAsync(alertGuid);
            return Ok(result);
        }


        [HttpGet("DashboardAlerts")]
        public async Task<ActionResult> GetDashboardAlertsAsync([FromQuery] int assignee, [FromQuery] int? userId, [FromQuery] int? patientId, [FromQuery] bool? isRead)
        {
            var result = await _noahService.GetDashboardAlertsAsync(assignee, userId, patientId, isRead);
            return Ok(result);
        }

        [HttpPut("UpdateDashboardAlert")]
        public async Task UpdateDashboardAlertAsync(N4DashboardAlert dashboardAlert)
        {
            await _noahService.UpdateDashboardAlertAsync(dashboardAlert);
        }

        [HttpPut("UpdateDashboardAlertArchive")]
        public async Task UpdateDashboardAlertArchiveAsync(N4DashboardAlertArchive dashboardAlert)
        {
            await _noahService.UpdateDashboardAlertArchiveAsync(dashboardAlert);
        }

        [HttpGet("Payload")]
        public async Task<ActionResult> GetPayloadAsync([FromQuery] int patientId)
        {
            var payload = await _noahService.GetNoahPayloadAsync(patientId);
            return Ok(payload);
        }
        [HttpGet("Payload2")]
        public async Task<ActionResult> GetPayloadAsync([FromQuery] Guid patientGuid)
        {
            var payload = await _noahService.GetNoahPayloadAsync(patientGuid);
            return Ok(payload);
        }
    }
}
