using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Noah;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Queries;

public class NoahQuery
{
	#region Constructors

	public NoahQuery(NoahDbContext dbContext, IOptions<AppSettings> appSettings)
	{
		_dbContext = dbContext;
		_appSettings = appSettings.Value;
	}

	#endregion Constructors

	#region NoahQuery Members

	public async Task DeleteActionAsync(int actionId)
	{
		var action = await GetActionAsync(actionId);
		if (action != null)
		{
			_dbContext.N4Actions.Remove(action);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeleteActionReferencesAsync(int actionId)
	{
		var actionRefs = await _dbContext.N4ActionReferences.Where(x => x.ActionId == actionId).ToListAsync();
		_dbContext.N4ActionReferences.RemoveRange(actionRefs);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteAppPermissionAsync(N4AppPermission permission)
	{
		_dbContext.N4AppPermissions.Remove(permission);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteArchivedActionsAsync(int actionId)
	{
		var archivedActionReferences = await _dbContext.N4ActionArchives
			.Where(x => x.OriginalActionId == actionId)
			.ToListAsync();

		var archivedActionIds = archivedActionReferences.Select(x => x.ModifiedActionId).ToList();

		var archivedActions = await _dbContext.N4Actions
			.Where(x => archivedActionIds.Contains(x.Id))
			.ToListAsync();

		_dbContext.N4Actions.RemoveRange(archivedActions);
		_dbContext.N4ActionArchives.RemoveRange(archivedActionReferences);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteArchivedUnboundActionsAsync(int actionId)
	{
		var archivedUnboundActionReferences = await _dbContext.N4UnboundActionArchives
			.Where(x => x.OriginalActionId == actionId)
			.ToListAsync();

		var archivedUnboundActionIds = archivedUnboundActionReferences.Select(x => x.ModifiedActionId).ToList();

		var archivedUnboundActions = await _dbContext.N4UnboundActions
			.Where(x => archivedUnboundActionIds.Contains(x.Id))
			.ToListAsync();

		_dbContext.N4UnboundActions.RemoveRange(archivedUnboundActions);
		_dbContext.N4UnboundActionArchives.RemoveRange(archivedUnboundActionReferences);

		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteDashboardAlertArchiveAsync(Guid alertGuid)
	{
		var alert = await _dbContext.N4DashboardAlertArchives.FirstOrDefaultAsync(x => x.AlertGuid == alertGuid);
		if (alert != null)
		{
			_dbContext.N4DashboardAlertArchives.Remove(alert);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeleteDashboardAlertAsync(Guid alertGuid)
	{
		var alert = await _dbContext.N4DashboardAlerts.FirstOrDefaultAsync(x => x.AlertGuid == alertGuid);
		if (alert != null)
		{
			_dbContext.N4DashboardAlerts.Remove(alert);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeleteFastViewAsync(int actionId)
	{
		var action = await _dbContext.N4Actions.FirstOrDefaultAsync(x => x.Id == actionId);
		if (action != null)
		{
			action.FastViewVersion = 0;
			action.FastViewDataType = 0;
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeleteMobileAppPermissionsAsync(IEnumerable<N4MobileAppPermission> mobilePermissions)
	{
		_dbContext.N4MobileAppPermissions.RemoveRange(mobilePermissions);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeletePatientIdentificationAsync(int patientId, int manufacturerId)
	{
		var patientIdentification = await _dbContext.N4PatientIdentifications
			.Where(x => x.PatientId == patientId && x.ManufacturerId == manufacturerId)
			.FirstOrDefaultAsync();

		if (patientIdentification != null)
		{
			_dbContext.N4PatientIdentifications.Remove(patientIdentification);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeletePatientSetupAsync(int patientId, int moduleId)
	{
		var patientSetup = await _dbContext.N4PatientSetups
			.Where(x => x.PatientId == patientId && x.ModuleId == moduleId)
			.FirstOrDefaultAsync();

		if (patientSetup != null)
		{
			_dbContext.N4PatientSetups.Remove(patientSetup);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeletePreferenceAsync(int preferenceId)
	{
		var preference = await GetPreferenceAsync(preferenceId);
		if (preference != null)
		{
			_dbContext.N4Preferences.Remove(preference);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeleteSessionAsync(int sessionId)
	{
		var session = await GetSessionAsync(sessionId);
		if (session != null)
		{
			_dbContext.N4Sessions.Remove(session);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeleteUnboundActionAsync(int unboundActionId)
	{
		var unboundAction = await GetUnboundActionAsync(unboundActionId);
		if (unboundAction != null)
		{
			_dbContext.N4UnboundActions.Remove(unboundAction);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task DeleteUnboundActionReferencesAsync(int actionId)
	{
		var unboundActionRefs = _dbContext.N4UnboundActionReferences.Where(x => x.ActionId == actionId);
		_dbContext.N4UnboundActionReferences.RemoveRange(unboundActionRefs);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteUserSetupAsync(int userId, int moduleId)
	{
		var userSetup = await _dbContext.N4UserSetups
			.Where(x => x.ModuleId == moduleId && x.UserId == userId)
			.FirstOrDefaultAsync();
		if (userSetup != null)
		{
			_dbContext.N4UserSetups.Remove(userSetup);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task<N4Action> GetActionAsync(int actionId)
	{
		N4Action result = null;
		try
		{
			result = await _dbContext.N4Actions.FirstOrDefaultAsync(x => x.Id == actionId);
			// These data fields are queried in other methods and are not used by the caller of this method.
			// No need to transfer this data over the network
			if (result != null)
			{
				result.PublicData = null;
				result.PrivateData = null;
				result.FastViewData = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<N4Action> GetActionAsync(Guid actionGuid)
	{
		N4Action result = null;
		try
		{
			result = await _dbContext.N4Actions.FirstOrDefaultAsync(x => x.ActionGuid == actionGuid);
			// These data fields are queried in other methods and are not used by the caller of this method.
			// No need to transfer this data over the network
			if (result != null)
			{
				result.PublicData = null;
				result.PrivateData = null;
				result.FastViewData = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4Action>> GetActionCollectionAsync(int moduleId)
	{
		List<N4Action> result = null;
		try
		{
			result = await _dbContext.N4Actions
				.Where(x => x.ModuleId == moduleId && !x.IsArchived)
				.OrderBy(x => x.CreatedDate)
				.ThenBy(x => x.Id)
				.ToListAsync();

			foreach (var action in result)
			{
				action.PublicData = null;
				action.PrivateData = null;
				action.FastViewData = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<int> GetActionCollectionCountAsync(int moduleId)
	{
		var result = 0;
		try
		{
			result = await _dbContext.N4Actions
				.Where(x => x.ModuleId == moduleId && !x.IsArchived)
				.CountAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<N4ActionData> GetActionDataAsync(int actionId)
	{
		N4ActionData result = null;
		try
		{
			var actionData = await _dbContext.N4Actions.Where(x => x.Id == actionId).Select(x =>
				new
				{
					x.CompressedPrivateBlob,
					x.CompressedPublicBlob,
					x.PublicData,
					x.PrivateData
				}).FirstOrDefaultAsync();

			if (actionData != null)
				result = new N4ActionData
				{
					PrivateData = actionData.CompressedPrivateBlob
						? CompressionHelper.DecompressAsync(actionData.PrivateData)
						: actionData.PrivateData,
					PublicData = actionData.CompressedPublicBlob
						? CompressionHelper.DecompressAsync(actionData.PublicData)
						: actionData.PublicData
				};
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<int[]> GetActionIdsFromDashboardGroupAsync(Guid group)
	{
		var ids = new HashSet<int>();
		var idPairs = await _dbContext.N4DashboardAlerts.Where(x => x.Group == group)
			.Select(x => new { x.ActionId, x.NotificationActionId }).ToListAsync();
		idPairs.AddRange(await _dbContext.N4DashboardAlertArchives.Where(x => x.Group == group)
			.Select(x => new { x.ActionId, x.NotificationActionId }).ToListAsync());
		foreach (var idPair in idPairs)
		{
			if (idPair.ActionId.HasValue && idPair.ActionId.Value > 0) ids.Add(idPair.ActionId.Value);
			if (idPair.NotificationActionId.HasValue && idPair.NotificationActionId.Value > 0)
				ids.Add(idPair.NotificationActionId.Value);
		}

		return ids.ToArray();
	}

	public async Task<int[]> GetActionReferencedByAsync(int actionId)
	{
		int[] result = null;
		try
		{
			result = await _dbContext.N4ActionReferences
				.Where(x => x.Reference == actionId)
				.Select(n => n.ActionId)
				.ToArrayAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<int[]> GetActionReferencesAsync(int actionId)
	{
		var actionReferences = await _dbContext.N4ActionReferences
			.Where(x => x.ActionId == actionId)
			.Select(x => x.Reference).ToArrayAsync();

		return actionReferences;
	}

	public async Task<IEnumerable<N4Action>> GetActionsAsync(int sessionId)
	{
		List<N4Action> result = null;
		try
		{
			result = await _dbContext.N4Actions
				.Where(x => x.SessionId == sessionId && !x.IsArchived)
				.OrderBy(x => x.CreatedDate)
				.ThenBy(x => x.Id)
				.ToListAsync();

			foreach (var action in result)
			{
				action.PublicData = null;
				action.PrivateData = null;
				action.FastViewData = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<List<N4Action>> GetActionsNoDataAsync(int sessionId)
	{
		List<N4Action> result = null;
		try
		{
			result = await _dbContext.N4Actions
				.Include(x => x.ActionReferences)
				.Where(x => x.SessionId == sessionId && !x.IsArchived)
				.OrderBy(x => x.CreatedDate)
				.ThenBy(x => x.Id)
				.Select(x => new N4Action
				{
					Id = x.Id,
					SessionId = x.SessionId,
					CreatedDate = x.CreatedDate,
					UpdatedUserId = x.UpdatedUserId,
					UpdatedDate = x.UpdatedDate,
					ModuleId = x.ModuleId,
					DevTypeCode = x.DevTypeCode,
					DataTypeCode = x.DataTypeCode,
					DataFmtCodeStd = x.DataFmtCodeStd,
					DataFmtCodePriv = x.DataFmtCodePriv,
					Description = x.Description,
					Removed = x.Removed,
					Hidden = x.Hidden,
					ActionGroup = x.ActionGroup,
					IsArchived = x.IsArchived,
					CompressedPrivateBlob = x.CompressedPrivateBlob,
					CompressedPublicBlob = x.CompressedPublicBlob,
					SpeechData = x.SpeechData,
					WordRecognitionData = x.WordRecognitionData,
					SetupData = x.SetupData,
					ExtendedData = x.ExtendedData,
					FastViewDataType = x.FastViewDataType,
					FastViewVersion = x.FastViewVersion,
					ActionGuid = x.ActionGuid,
					ActionReferences = x.ActionReferences,
					PublicData = null,
					PrivateData = null,
					FastViewData = null
				})
				.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<N4AppPermission> GetAppPermissionAsync(int permissionId)
	{
		N4AppPermission permission = null;
		try
		{
			permission = await _dbContext.N4AppPermissions.FirstOrDefaultAsync(p => p.Id == permissionId);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return permission;
	}

	public async Task<IEnumerable<N4AppPermission>> GetAppPermissionsAsync()
	{
		IEnumerable<N4AppPermission> result = null;
		try
		{
			result = await _dbContext.N4AppPermissions
				.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4Action>> GetArchivedActionsAsync(int actionId)
	{
		var actionIds = await _dbContext.N4ActionArchives
			.Where(x => x.OriginalActionId == actionId)
			.Select(x => x.ModifiedActionId)
			.ToListAsync();

		var actionArchives = await _dbContext.N4Actions
			.Where(x => actionIds.Contains(x.Id))
			.OrderBy(x => x.CreatedDate)
			.ThenBy(x => x.Id)
			.ToListAsync();

		return actionArchives;
	}

	public async Task<int> GetArchivedActionsCountAsync(int actionId)
	{
		var actionIds = await _dbContext.N4ActionArchives
			.Where(x => x.OriginalActionId == actionId)
			.Select(x => x.ModifiedActionId)
			.ToListAsync();

		var actionArchivesCount = await _dbContext.N4Actions
			.Where(x => actionIds.Contains(x.Id))
			.CountAsync();

		return actionArchivesCount;
	}

	public async Task<IEnumerable<N4UnboundAction>> GetArchivedUnboundActionsAsync(int actionId)
	{
		var unboundActionIds = await _dbContext.N4UnboundActionArchives
			.Where(x => x.OriginalActionId == actionId)
			.Select(x => x.ModifiedActionId)
			.ToListAsync();

		var unboundActionArchives = await _dbContext.N4UnboundActions
			.Where(x => unboundActionIds.Contains(x.Id))
			.ToListAsync();

		return unboundActionArchives;
	}

	public async Task<int> GetArchivedUnboundActionsCountAsync(int actionId)
	{
		var unboundActionIds = await _dbContext.N4UnboundActionArchives
			.Where(x => x.OriginalActionId == actionId)
			.Select(x => x.ModifiedActionId)
			.ToListAsync();

		var unboundActionArchivesCount = await _dbContext.N4UnboundActions
			.Where(x => unboundActionIds.Contains(x.Id))
			.CountAsync();

		return unboundActionArchivesCount;
	}

	public ConnectionInfo GetConnectionInfo()
	{
		var connectionInfo = _dbContext.Database.GetDbConnection();
		return new ConnectionInfo
		{
			Server = connectionInfo.DataSource,
			Database = connectionInfo.Database
		};
	}

	public async Task<N4DashboardAlertArchive> GetDashboardAlertArchiveAsync(Guid alertGuid)
	{
		N4DashboardAlertArchive result = null;
		try
		{
			result = await _dbContext.N4DashboardAlertArchives
				.Where(p => p.AlertGuid == alertGuid)
				.FirstOrDefaultAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4DashboardAlert> GetDashboardAlertAsync(Guid alertGuid)
	{
		N4DashboardAlert result = null;
		try
		{
			result = await _dbContext.N4DashboardAlerts
				.Where(p => p.AlertGuid == alertGuid)
				.FirstOrDefaultAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4DashboardAlert[]> GetDashboardAlertsAsync(int assignee, int? userId, int? patientId,
		bool? isRead)
	{
		N4DashboardAlert[] alertList = null;
		try
		{
			IQueryable<N4DashboardAlert> query = _dbContext.N4DashboardAlerts;

			if (assignee == 2) // User
				query = query.Where(x => x.AssigneeUserId == userId);
			else if (assignee == 3) // Unassigned
				query = query.Where(x => x.AssigneeUserId == null || x.AssigneeUserId == 0);

			if (patientId.HasValue)
			{
				var patientGuid = (await GetPatientAsync(patientId.Value)).PatientGuid;
				query = query.Where(x => x.PatientGuid == patientGuid);
			}

			if (isRead.HasValue)
				query = query.Where(x => x.IsRead == isRead);

			alertList = await query.ToArrayAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return alertList;
	}

	public async Task<N4FastView> GetFastViewAsync(int actionId)
	{
		N4FastView result = null;
		try
		{
			var action = await _dbContext.N4Actions.FirstOrDefaultAsync(x => x.Id == actionId);
			if (action?.FastViewData != null)
				result = new N4FastView
				{
					CreatedDate = action.CreatedDate,
					UpdatedDate = action.UpdatedDate ?? SqlDateTime.MinValue.Value,
					Version = action.FastViewVersion,
					Data = action.FastViewData.ToArray(),
					Format = action.FastViewDataType
				};
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4Session>> GetFullSessionsAsync(int patientId)
	{
		IEnumerable<N4Session> result = null;
		try
		{
			result = await _dbContext.N4Sessions
				.Include(p => p.Actions)
				.ThenInclude(p => p.ActionReferences)
				.Where(p => p.PatientId == patientId)
				.ToListAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<string>> GetManufacturerSetupKeysAsync(int manufacturerId)
	{
		var manufacturerSetupKeys = await _dbContext.N4ManufacturerSetups
			.Where(x => x.ManufacturerId == manufacturerId)
			.Select(x => x.Key)
			.ToListAsync();
		return manufacturerSetupKeys;
	}

	public async Task<IEnumerable<N4ManufacturerSetup>> GetManufacturerSetupsAsync(int manufacturerId, string[] keys)
	{
		var manufacturerSetups = await _dbContext.N4ManufacturerSetups
			.Where(x => x.ManufacturerId == manufacturerId && keys.Contains(x.Key))
			.ToListAsync();
		return manufacturerSetups;
	}

	public async Task<N4MobileApp> GetMobileAppAsync(int moduleId)
	{
		var mobileApp = await _dbContext.N4MobileApps
			.Where(x => x.ModuleId == moduleId)
			.FirstOrDefaultAsync();
		return mobileApp;
	}

	public async Task<IEnumerable<N4MobileAppPermission>> GetMobileAppPermissionsAsync(int? permissionId, int? moduleId)
	{
		List<N4MobileAppPermission> mobilePermissions = null;
		try
		{
			var query = _dbContext.N4MobileAppPermissions
				.AsQueryable();
			if (permissionId.HasValue) query = query.Where(p => p.PermissionId == permissionId);
			if (moduleId.HasValue) query = query.Where(x => x.ModuleId == moduleId);

			mobilePermissions = await query.ToListAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return mobilePermissions;
	}

	public async Task<IEnumerable<N4MobileApp>> GetMobileAppsAsync()
	{
		var mobileApps = await _dbContext.N4MobileApps
			.ToListAsync();
		return mobileApps;
	}

	public async Task<N4Action> GetNewestActionAsync(int patientId)
	{
		var sessionIds = await _dbContext.N4Sessions
			.Where(x => x.PatientId == patientId)
			.Select(x => x.Id)
			.ToListAsync();

		var action = await _dbContext.N4Actions
			.Where(x => sessionIds.Contains(x.SessionId) && (x.Removed == null || !x.Removed.Value) &&
			            (x.Hidden == null || !x.Hidden.Value) && x.DataTypeCode == 1)
			.OrderByDescending(x => x.CreatedDate)
			.FirstOrDefaultAsync();

		return action;
	}

	public async Task<NoahPayload> GetNoahPayloadAsync(int patientId)
	{
		var payload = new NoahPayload();
		payload.Patient = await GetPatientAsync(patientId);
		payload.Actions = new Dictionary<int, List<N4Action>>();
		payload.ActionReferences = new Dictionary<int, int[]>();
		payload.Sessions = (await GetSessionsAsync(patientId)).ToList();

		foreach (var session in payload.Sessions)
		{
			var actions = await GetActionsNoDataAsync(session.Id);
			if (actions != null)
			{
				foreach (var action in actions)
				{
					payload.ActionReferences[action.Id] = action.ActionReferences.Select(x => x.Reference).ToArray();
					action.ActionReferences = null;
				}

				payload.Actions[session.Id] = actions;
			}
		}

		return payload;
	}

	public async Task<NoahPayload> GetNoahPayloadAsync(Guid patientGuid)
	{
		var payload = new NoahPayload();
		payload.Patient = await GetPatientAsync(patientGuid);
		payload.Actions = new Dictionary<int, List<N4Action>>();
		payload.ActionReferences = new Dictionary<int, int[]>();
		payload.Sessions = (await GetSessionsAsync(payload.Patient.Id)).ToList();

		foreach (var session in payload.Sessions)
		{
			var actions = await GetActionsNoDataAsync(session.Id);
			if (actions != null)
			{
				foreach (var action in actions)
				{
					payload.ActionReferences[action.Id] = action.ActionReferences.Select(x => x.Reference).ToArray();
					action.ActionReferences = null;
				}

				payload.Actions[session.Id] = actions;
			}
		}

		return payload;
	}

	public async Task<N4Patient> GetPatientAsync(int patientId)
	{
		N4Patient result = null;
		try
		{
			var patientSessionCount = _dbContext.N4Sessions.Count(x => x.PatientId == patientId);
			var patient = await _dbContext.Patients
				//.Include(x => x.ReferringPhysician)
				//.Include(x => x.Provider)
				.Include(x => x.UpdatedByUser)
				.FirstOrDefaultAsync(x => x.Id == patientId);


			if (patient != null)
				result = new N4Patient
				{
					Id = patient.Id,
					PatientGuid = patient.Guid,
					ActivePatient = (short)(patient.Inactive ? 0 : 1),
					PatientNo = patient.Id.ToString(),
					DateCreated = patient.CreatedDate,
					FirstName = patient.FirstName,
					MiddleName = patient.Initial ?? string.Empty,
					LastName = patient.LastName,
					Gender = (short)(patient.Sex == "M" ? 1 : patient.Sex == "F" ? 2 : 0),
					Birthdate = patient.BirthDate ?? SqlDateTime.MinValue.Value,
					Salutation = patient.Salutation == null ? string.Empty : patient.Salutation.Name,
					Title = string.Empty,
					Address1 = patient.Address1 ?? string.Empty,
					Address2 = patient.Address2 ?? string.Empty,
					State = patient.State ?? string.Empty,
					City = patient.City ?? string.Empty,
					Zip = patient.ZipCode ?? string.Empty,
					HomePhone = patient.HomePhone ?? string.Empty,
					WorkPhone = patient.WorkPhone ?? string.Empty,
					MobilePhone = patient.MobilePhone ?? string.Empty,
					Email = patient.Email ?? string.Empty,
					Ssn = patient.Ssn ?? string.Empty,
					Other1 = patient.CustomText1 ?? string.Empty,
					Other2 = patient.CustomText2 ?? string.Empty,
					Physician = patient.Provider == null ? string.Empty : patient.Provider.FullName,
					Referral = patient.ReferringPhysician == null ? string.Empty : patient.ReferringPhysician.Name,
					CreatedBy = patient.UpdatedByUser == null ? string.Empty : patient.UpdatedByUser.Name,
					SessionCount = patientSessionCount
				};

			//result = await _dbContext.Patients
			//	.Where( x => x.Id == patientId )
			//                .Select(x => new N4Patient()
			//                {
			//                    Id = x.Id,
			//                    PatientGuid = x.Guid,
			//                    ActivePatient = (short)(x.Inactive ? 0 : 1),
			//                    PatientNo = x.Id.ToString(),
			//                    DateCreated = x.CreatedDate,
			//                    FirstName = x.FirstName,
			//                    MiddleName = x.Initial ?? string.Empty,
			//                    LastName = x.LastName,
			//                    Gender = (short)(x.Sex == "M" ? 1 : (x.Sex == "F" ? 2 : 0)),
			//                    Birthdate = x.BirthDate ?? SqlDateTime.MinValue.Value,
			//                    Salutation = x.Salutation == null ? string.Empty : x.Salutation.Name,
			//                    Title = string.Empty,
			//                    Address1 = x.Address1 ?? string.Empty,
			//                    Address2 = x.Address2 ?? string.Empty,
			//                    State = x.State ?? string.Empty,
			//                    City = x.City ?? string.Empty,
			//                    Zip = x.ZipCode ?? string.Empty,
			//                    HomePhone = x.HomePhone ?? string.Empty,
			//                    WorkPhone = x.WorkPhone ?? string.Empty,
			//                    MobilePhone = x.MobilePhone ?? string.Empty,
			//                    Email = x.Email ?? string.Empty,
			//                    Ssn = x.Ssn ?? string.Empty,
			//                    Insurance1 = x.InsuredInsurancePayer == null ? string.Empty : x.InsuredInsurancePayer.Name,
			//                    Insurance2 = x.OtherInsurancePayer == null ? string.Empty : x.OtherInsurancePayer.Name,
			//                    Other1 = x.CustomText1 ?? string.Empty,
			//                    Other2 = x.CustomText2 ?? string.Empty,
			//                    Physician = x.Provider == null ? string.Empty : x.Provider.FullName,
			//                    Referral = x.ReferringPhysician.Name,
			//                    CreatedBy = x.UpdatedByUser == null ? string.Empty : x.UpdatedByUser.Name,
			//                    SessionCount = patientSessionCount
			//                })
			// //               .Select(x => new N4Patient()
			//	//{
			//	//	Id = x.Id,
			//	//	PatientGuid = x.Guid,
			//	//	ActivePatient = (short)(x.Inactive ? 0 : 1),
			//	//	PatientNo = x.Id.ToString(),
			//	//	DateCreated = x.CreatedDate,
			//	//	FirstName = x.FirstName,
			//	//	MiddleName = x.Initial,
			//	//	LastName = x.LastName,
			//	//	Gender = (short)(x.Sex == "M" ? 1 : (x.Sex == "F" ? 2 : 0)),
			//	//	Birthdate = x.BirthDate ?? SqlDateTime.MinValue.Value,
			//	//	Salutation = x.Salutation == null ? string.Empty : x.Salutation.Name,
			//	//	Title = string.Empty,
			//	//	Address1 = x.Address1,
			//	//	Address2 = x.Address2,
			//	//	State = x.State,
			//	//	City = x.City,
			//	//	Zip = x.ZipCode,
			//	//	HomePhone = x.HomePhone,
			//	//	WorkPhone = x.WorkPhone,
			//	//	MobilePhone = x.MobilePhone,
			//	//	Email = x.Email,
			//	//	Ssn = x.Ssn,
			//	//	Other1 = x.CustomText1,
			//	//	Other2 = x.CustomText2,
			//	//	Physician = x.Provider == null ? string.Empty : x.Provider.FullName,
			//	//	Referral = x.ReferringPhysician.Name,
			//	//	CreatedBy = x.UpdatedByUser == null ? string.Empty : x.UpdatedByUser.Name,
			//	//	SessionCount = patientSessionCount
			//	//})
			//	.FirstOrDefaultAsync();
		}
		catch (ArgumentException e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4Patient> GetPatientAsync(Guid patientGuid)
	{
		N4Patient result = null;
		try
		{
			var patientId = await _dbContext.Patients.Where(x => x.Guid == patientGuid).Select(x => x.Id).FirstAsync();
			var patientSessionCount = _dbContext.N4Sessions.Count(x => x.PatientId == patientId);

			result = await _dbContext.Patients
				.Where(x => x.Id == patientId)
				.Select(x => new N4Patient
				{
					Id = x.Id,
					PatientGuid = x.Guid,
					ActivePatient = (short)(x.Inactive ? 0 : 1),
					PatientNo = x.Id.ToString(),
					DateCreated = x.CreatedDate,
					FirstName = x.FirstName,
					MiddleName = x.Initial ?? string.Empty,
					LastName = x.LastName,
					Gender = (short)(x.Sex == "M" ? 1 : x.Sex == "F" ? 2 : 0),
					Birthdate = x.BirthDate ?? SqlDateTime.MinValue.Value,
					Salutation = x.Salutation == null ? string.Empty : x.Salutation.Name,
					Title = string.Empty,
					Address1 = x.Address1 ?? string.Empty,
					Address2 = x.Address2 ?? string.Empty,
					State = x.State ?? string.Empty,
					City = x.City ?? string.Empty,
					Zip = x.ZipCode ?? string.Empty,
					HomePhone = x.HomePhone ?? string.Empty,
					WorkPhone = x.WorkPhone ?? string.Empty,
					MobilePhone = x.MobilePhone ?? string.Empty,
					Email = x.Email ?? string.Empty,
					Ssn = x.Ssn ?? string.Empty,
					Other1 = x.CustomText1 ?? string.Empty,
					Other2 = x.CustomText2 ?? string.Empty,
					Physician = x.Provider == null ? string.Empty : x.Provider.FullName,
					Referral = x.ReferringPhysician.Name,
					CreatedBy = x.UpdatedByUser == null ? string.Empty : x.UpdatedByUser.Name,
					SessionCount = patientSessionCount
				})
				.FirstOrDefaultAsync();
		}
		catch (ArgumentException e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<int[]> GetPatientIdsFromIdentificationAsync(string identification, int manufacturerId)
	{
		int[] patientIds;
		try
		{
			patientIds = await _dbContext.N4PatientIdentifications
				.Where(x => x.IdentificationData == identification && x.ManufacturerId == manufacturerId)
				.Select(x => x.PatientId)
				.ToArrayAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return patientIds;
	}

	public async Task<IEnumerable<N4Patient>> GetPatientsAsync(string searchText, int page, int pageSize)
	{
		IEnumerable<N4Patient> result = null;
		try
		{
			var query = _dbContext.Patients
				.OrderBy(x => x.Id)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchText))
				query = query.Where(p => p.FirstName.Contains(searchText) || p.LastName.Contains(searchText) ||
				                         p.Initial.Contains(searchText) || p.Address1.Contains(searchText) ||
				                         p.Address2.Contains(searchText) || p.ZipCode.Contains(searchText) ||
				                         p.City.Contains(searchText) || p.HomePhone.Contains(searchText) ||
				                         p.WorkPhone.Contains(searchText) || p.OtherPhone.Contains(searchText) ||
				                         p.Ssn.Contains(searchText) || p.Email.Contains(searchText) ||
				                         p.CustomText1.Contains(searchText) || p.CustomText2.Contains(searchText) ||
				                         p.Id.ToString().Contains(searchText))
					.AsQueryable();

			if (page > 1) query = query.Skip((page - 1) * pageSize);
			query = query.Take(pageSize);
			var patients = await query
				.Select(x => new N4Patient
				{
					Id = x.Id,
					PatientGuid = x.Guid,
					ActivePatient = (short)(x.Inactive ? 0 : 1),
					PatientNo = x.Id.ToString(),
					DateCreated = x.CreatedDate,
					FirstName = x.FirstName,
					MiddleName = x.Initial ?? string.Empty,
					LastName = x.LastName,
					Gender = (short)(x.Sex == "M" ? 1 : x.Sex == "F" ? 2 : 0),
					Birthdate = x.BirthDate ?? SqlDateTime.MinValue.Value,
					Salutation = x.Salutation == null ? string.Empty : x.Salutation.Name,
					Title = string.Empty,
					Address1 = x.Address1 ?? string.Empty,
					Address2 = x.Address2 ?? string.Empty,
					State = x.State ?? string.Empty,
					City = x.City ?? string.Empty,
					Zip = x.ZipCode ?? string.Empty,
					HomePhone = x.HomePhone ?? string.Empty,
					WorkPhone = x.WorkPhone ?? string.Empty,
					MobilePhone = x.MobilePhone ?? string.Empty,
					Email = x.Email ?? string.Empty,
					Ssn = x.Ssn ?? string.Empty,
					Other1 = x.CustomText1 ?? string.Empty,
					Other2 = x.CustomText2 ?? string.Empty,
					Physician = x.Provider == null ? string.Empty : x.Provider.FullName,
					Referral = x.ReferringPhysician.Name,
					CreatedBy = x.UpdatedByUser == null ? string.Empty : x.UpdatedByUser.Name,
					SessionCount = 0
				})
				.ToListAsync();

			foreach (var patient in patients)
				patient.SessionCount = _dbContext.N4Sessions.Count(x => x.PatientId == patient.Id);

			result = patients;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<int> GetPatientsCountAsync(string searchText)
	{
		var result = 0;
		try
		{
			result = await _dbContext.Patients
				.Where(p => p.FirstName.Contains(searchText) || p.LastName.Contains(searchText) ||
				            p.Initial.Contains(searchText) || p.Address1.Contains(searchText) ||
				            p.Address2.Contains(searchText) || p.ZipCode.Contains(searchText) ||
				            p.City.Contains(searchText) || p.HomePhone.Contains(searchText) ||
				            p.WorkPhone.Contains(searchText) || p.OtherPhone.Contains(searchText) ||
				            p.Ssn.Contains(searchText) || p.Email.Contains(searchText) ||
				            p.CustomText1.Contains(searchText) || p.CustomText2.Contains(searchText) ||
				            p.Id.ToString().Contains(searchText))
				.CountAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4PatientSetup> GetPatientSetupAsync(int patientId, int moduleId)
	{
		N4PatientSetup result = null;
		try
		{
			result = await _dbContext.N4PatientSetups
				.Where(p => p.PatientId == patientId && p.ModuleId == moduleId)
				.FirstOrDefaultAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4PatientSetup>> GetPatientSetupsAsync(int patientId, int moduleId)
	{
		IEnumerable<N4PatientSetup> result = null;
		try
		{
			result = await _dbContext.N4PatientSetups
				.Where(p => p.PatientId == patientId && p.ModuleId == moduleId)
				.ToListAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4PatientSetup>> GetPatientSetupsForModuleAsync(int moduleId)
	{
		IEnumerable<N4PatientSetup> result = null;
		try
		{
			result = await _dbContext.N4PatientSetups
				.Where(p => p.ModuleId == moduleId)
				.ToListAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4Preference> GetPreferenceAsync(int preferenceId)
	{
		N4Preference result = null;
		try
		{
			result = await _dbContext.N4Preferences
				.Where(p => p.Id == preferenceId)
				.FirstOrDefaultAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4Session> GetSessionAsync(int sessionId)
	{
		N4Session result = null;
		try
		{
			result = await _dbContext.N4Sessions
				.Where(p => p.Id == sessionId)
				.FirstOrDefaultAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4Session>> GetSessionsAsync(int patientId)
	{
		IEnumerable<N4Session> result = null;
		try
		{
			result = await _dbContext.N4Sessions
				.Where(p => p.PatientId == patientId)
				.ToListAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4UnboundAction> GetUnboundActionAsync(int actionId)
	{
		N4UnboundAction result = null;
		try
		{
			result = await _dbContext.N4UnboundActions.FirstOrDefaultAsync(x => x.Id == actionId);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<N4ActionData> GetUnboundActionDataAsync(int actionId)
	{
		N4ActionData result = null;
		try
		{
			var n4Action = await _dbContext.N4UnboundActions
				.FirstOrDefaultAsync(x => x.Id == actionId);

			if (n4Action != null)
				result = new N4ActionData
				{
					PrivateData = n4Action.CompressedPrivateBlob
						? CompressionHelper.DecompressAsync(n4Action.PrivateData)
						: n4Action.PrivateData,
					PublicData = n4Action.CompressedPublicBlob
						? CompressionHelper.DecompressAsync(n4Action.PublicData)
						: n4Action.PublicData
				};
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<int[]> GetUnboundActionIdsAsync(DateTime startDate, DateTime endDate)
	{
		int[] result;
		try
		{
			result = await _dbContext.N4UnboundActions
				.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
				.Select(x => x.Id)
				.ToArrayAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<int[]> GetUnboundActionReferencesAsync(int actionId)
	{
		int[] result = null;
		try
		{
			result = await _dbContext.N4UnboundActionReferences
				.Where(x => x.ActionId == actionId)
				.Select(x => x.Reference)
				.ToArrayAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<int[]> GetUnboundActionReferencesAsync(int[] actionIds)
	{
		int[] result = null;
		try
		{
			var actionReferences = new List<int>();
			foreach (var actionId in actionIds)
			{
				var ids = await _dbContext.N4UnboundActionReferences
					.Where(x => x.ActionId == actionId)
					.Select(x => x.Reference)
					.ToArrayAsync();
				actionReferences.AddRange(ids);
			}

			result = actionReferences.ToArray();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4UnboundAction>> GetUnboundActionsAsync(int[] actionIds)
	{
		List<N4UnboundAction> result = null;
		try
		{
			result = await _dbContext.N4UnboundActions
				.Where(x => actionIds.Contains(x.Id))
				.OrderBy(x => x.CreatedDate)
				.ThenBy(x => x.Id)
				.ToListAsync();

			foreach (var action in result)
			{
				action.PublicData = null;
				action.PrivateData = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4Action>> GetUpdatedActionsAsync(int page, int pageSize, DateTime startTime,
		DateTime? endTime, short[] dataTypes)
	{
		if (page < 1)
			throw new ArgumentOutOfRangeException("page");
		if (pageSize < 1)
			throw new ArgumentOutOfRangeException("pageSize");

		IEnumerable<N4Action> result = null;
		try
		{
			var endtime = endTime ?? DateTime.Now;

			var query = _dbContext.N4Actions
				.Include(x => x.Session)
				.Where(x => x.UpdatedDate >= startTime && x.UpdatedDate <= endtime && x.Session != null)
				.AsQueryable();

			if (dataTypes != null && dataTypes.Length > 0)
				query = query.Where(x => x.DataTypeCode.HasValue && dataTypes.Contains(x.DataTypeCode.Value));

			if (page > 1) query = query.Skip((page - 1) * pageSize);

			query = query.Take(pageSize).OrderBy(x => x.CreatedDate).ThenBy(x => x.Id);
			result = await query.ToListAsync();
			foreach (var entry in result) entry.Session.Actions = null;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4Patient>> GetUpdatedPatientsAsync(int page, int pageSize, DateTime startTime,
		DateTime? endTime)
	{
		if (page < 1)
			throw new ArgumentOutOfRangeException("page");
		if (pageSize < 1)
			throw new ArgumentOutOfRangeException("pageSize");

		IEnumerable<N4Patient> result = null;
		try
		{
			var endtime = endTime ?? DateTime.Now;

			var query = _dbContext.Patients
				.Include(x => x.Salutation)
				.Include(x => x.Provider)
				.Include(x => x.ReferringPhysician)
				.Include(x => x.PrimaryCarePhysician)
				.Include(x => x.UpdatedByUser)
				.Where(x => x.UpdatedDate >= startTime && x.UpdatedDate <= endtime && !x.Inactive)
				.AsQueryable();

			if (page > 1) query = query.Skip((page - 1) * pageSize);

			query = query.Take(pageSize).OrderBy(x => x.CreatedDate).ThenBy(x => x.Id);

			var patientQuery = query
				.Select(x => new N4Patient
				{
					Id = x.Id,
					PatientGuid = x.Guid,
					ActivePatient = (short)(x.Inactive ? 0 : 1),
					PatientNo = x.Id.ToString(),
					DateCreated = x.CreatedDate,
					FirstName = x.FirstName,
					MiddleName = x.Initial ?? string.Empty,
					LastName = x.LastName,
					Gender = (short)(x.Sex == "M" ? 1 : x.Sex == "F" ? 2 : 0),
					Birthdate = x.BirthDate ?? SqlDateTime.MinValue.Value,
					Salutation = x.Salutation == null ? string.Empty : x.Salutation.Name,
					Title = string.Empty,
					Address1 = x.Address1 ?? string.Empty,
					Address2 = x.Address2 ?? string.Empty,
					State = x.State ?? string.Empty,
					City = x.City ?? string.Empty,
					Zip = x.ZipCode ?? string.Empty,
					HomePhone = x.HomePhone ?? string.Empty,
					WorkPhone = x.WorkPhone ?? string.Empty,
					MobilePhone = x.MobilePhone ?? string.Empty,
					Email = x.Email ?? string.Empty,
					Ssn = x.Ssn ?? string.Empty,
					Other1 = x.CustomText1 ?? string.Empty,
					Other2 = x.CustomText2 ?? string.Empty,
					Physician = x.Provider == null ? string.Empty : x.Provider.FullName,
					Referral = x.ReferringPhysician.Name,
					CreatedBy = x.UpdatedByUser == null ? string.Empty : x.UpdatedByUser.Name,
					SessionCount = 0
				});

			var patients = await patientQuery.ToListAsync();

			foreach (var patient in patients)
				patient.SessionCount = _dbContext.N4Sessions.Count(x => x.PatientId == patient.Id);

			result = patients;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task<N4User> GetUserAsync(int userId)
	{
		N4User result = null;
		try
		{
			result = await _dbContext.Users
				.Where(x => x.Id == userId)
				.Select(x => new N4User
				{
					Id = x.Id,
					CreatedDate = x.UpdatedDate,
					Name = x.Name,
					Initials = x.Initials
				})
				.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<N4User> GetUserAsync(string username)
	{
		N4User result = null;
		try
		{
			result = await _dbContext.Users
				.Where(x => x.Name == username)
				.Select(x => new N4User
				{
					Id = x.Id,
					CreatedDate = x.UpdatedDate,
					Name = x.Name,
					Initials = x.Initials
				})
				.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<int> GetUserPrivilegeAsync(int userId)
	{
		var result = 0;
		try
		{
			result = await _dbContext.Users
				.Where(x => x.Id == userId)
				.Select(x => x.NoahPermissions)
				.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<IEnumerable<N4User>> GetUsersAsync()
	{
		List<N4User> result = null;
		try
		{
			result = await _dbContext.Users
				.Where(x => !x.Inactive)
				.Select(x => new N4User
				{
					Id = x.Id,
					CreatedDate = x.UpdatedDate,
					Name = x.Name,
					Initials = x.Initials
				})
				.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<N4UserSetup> GetUserSetupAsync(int userId, int moduleId)
	{
		N4UserSetup result = null;
		try
		{
			result = await _dbContext.N4UserSetups
				.FirstOrDefaultAsync(x => x.UserId == userId && x.ModuleId == moduleId);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		return result;
	}

	public async Task PutActionArchiveAsync(N4ActionArchive actionArchive)
	{
		try
		{
			await _dbContext.N4ActionArchives.AddAsync(actionArchive);
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutActionAsync(N4Action action, bool archive)
	{
		try
		{
			N4Action dbAction = null;
			if (action.Id != 0)
				dbAction = await _dbContext.N4Actions.FirstOrDefaultAsync(x => x.Id == action.Id);
			else if (action.ActionGuid != Guid.Empty)
				dbAction = await _dbContext.N4Actions.FirstOrDefaultAsync(x => x.ActionGuid == action.ActionGuid);

			try
			{
				if (action.CreatedDate < SqlDateTime.MinValue)
					action.CreatedDate = SqlDateTime.MinValue.Value;
			}
			catch (Exception)
			{
				action.CreatedDate = SqlDateTime.MinValue.Value;
			}

			try
			{
				if (!action.UpdatedDate.HasValue || action.UpdatedDate.Value < SqlDateTime.MinValue.Value)
					action.UpdatedDate = SqlDateTime.MinValue.Value;
			}
			catch (Exception)
			{
				action.UpdatedDate = SqlDateTime.MinValue.Value;
			}

			try
			{
				if (action.ActionGroup < SqlDateTime.MinValue.Value)
					action.ActionGroup = SqlDateTime.MinValue.Value;
			}
			catch (Exception)
			{
				action.ActionGroup = SqlDateTime.MinValue.Value;
			}

			action.ActionGuid = action.ActionGuid == Guid.Empty ? Guid.NewGuid() : action.ActionGuid;
			action.IsArchived = archive;
			if (action.PublicData == null)
			{
				action.PublicData = dbAction?.PublicData;
			}
			else
			{
				if (action.PublicData.Length > 0)
					action.PublicData = CompressionHelper.CompressAsync(action.PublicData);
				else
					action.PublicData = null;
			}

			if (action.PrivateData == null)
			{
				action.PrivateData = dbAction?.PrivateData;
			}
			else
			{
				if (action.PrivateData.Length > 0)
					action.PrivateData = CompressionHelper.CompressAsync(action.PrivateData);
				else
					action.PrivateData = null;
			}

			action.CompressedPublicBlob = action.PublicData != null && action.PublicData.Length > 0;
			action.CompressedPrivateBlob = action.PrivateData != null && action.PrivateData.Length > 0;

			if (dbAction == null)
			{
				//if(action.Session == null)
				//{
				//    action.SessionId = 0;
				//    action.Session = new N4Session
				//    {
				//        CreateDate = DateTime.Now,
				//        PatientId
				//    }
				//}
				_dbContext.N4Actions.Add(action);
			}
			else
			{
				dbAction.CreatedDate = action.CreatedDate;
				dbAction.ActionGroup = action.ActionGroup;
				dbAction.CompressedPrivateBlob = action.CompressedPrivateBlob;
				dbAction.CompressedPublicBlob = action.CompressedPublicBlob;
				dbAction.UpdatedDate = action.UpdatedDate;
				dbAction.UpdatedUserId = action.UpdatedUserId;
				dbAction.IsArchived = action.IsArchived;
				dbAction.PublicData = action.PublicData;
				dbAction.PrivateData = action.PrivateData;
			}

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutActionReferencesAsync(int actionId, int[] actionReferences)
	{
		try
		{
			if (actionReferences != null)
			{
				if (actionReferences.Length == 0)
				{
					await DeleteActionReferencesAsync(actionId);
				}
				else
				{
					foreach (var reference in actionReferences)
					{
						var n4ActionReference = new N4ActionReference
						{
							ActionId = actionId,
							Reference = reference
						};
						_dbContext.N4ActionReferences.Add(n4ActionReference);
					}

					await _dbContext.SaveChangesAsync();
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutAppPermissionsAsync(N4AppPermission[] appPermissions)
	{
		try
		{
			foreach (var appPermission in appPermissions)
				if (appPermission.Id == 0)
					await _dbContext.N4AppPermissions.AddAsync(appPermission);
				else
					_dbContext.N4AppPermissions.Attach(appPermission).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutDashboardAlertArchiveAsync(N4DashboardAlertArchive dashboardAlert)
	{
		try
		{
			_dbContext.N4DashboardAlertArchives.Add(dashboardAlert);
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutDashboardAlertAsync(N4DashboardAlert dashboardAlert)
	{
		try
		{
			_dbContext.N4DashboardAlerts.Add(dashboardAlert);
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutManufacturerSetupAsync(N4ManufacturerSetup[] manufacturerSetups)
	{
		try
		{
			foreach (var manufacturerSetup in manufacturerSetups)
			{
				if (manufacturerSetup.SetupData == null) continue;

				var setup = _dbContext.N4ManufacturerSetups.FirstOrDefault(x =>
					x.ManufacturerId == manufacturerSetup.ManufacturerId && x.Key == manufacturerSetup.Key);

				if (setup == null && manufacturerSetup.SetupData.Length != 0)
					_dbContext.N4ManufacturerSetups.Add(manufacturerSetup);
				else if (setup != null && manufacturerSetup.SetupData.Length == 0)
					_dbContext.N4ManufacturerSetups.Remove(setup);
				else if (setup != null) setup.SetupData = manufacturerSetup.SetupData;
			}

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutMobileAppPermissionsAsync(int moduleId, int[] permissionIds)
	{
		try
		{
			var oldPermissions =
				await _dbContext.N4MobileAppPermissions.Where(x => x.ModuleId == moduleId).ToListAsync();
			_dbContext.N4MobileAppPermissions.RemoveRange(oldPermissions);
			await _dbContext.SaveChangesAsync();
			foreach (var permissionId in permissionIds)
				_dbContext.N4MobileAppPermissions.Add(new N4MobileAppPermission
				{
					ModuleId = moduleId,
					PermissionId = permissionId
				});
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutPatientIdentificationAsync(N4PatientIdentification patientIdentification)
	{
		try
		{
			if (patientIdentification.Id == 0)
				_dbContext.N4PatientIdentifications.Add(patientIdentification);
			else
				_dbContext.N4PatientIdentifications.Attach(patientIdentification).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutPatientSetupAsync(N4PatientSetup patientSetup)
	{
		try
		{
			var n4PatientSetup = await _dbContext.N4PatientSetups
				.FirstOrDefaultAsync(x => x.PatientId == patientSetup.PatientId && x.ModuleId == patientSetup.ModuleId);

			if (n4PatientSetup != null)
				n4PatientSetup.SetupData = patientSetup.SetupData;
			else
				_dbContext.N4PatientSetups.Add(patientSetup);
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutPreferenceAsync(int preferenceId, byte[] preference)
	{
		try
		{
			var n4Preference = await _dbContext.N4Preferences.FirstOrDefaultAsync(x => x.Id == preferenceId);

			if (n4Preference == null)
			{
				n4Preference = new N4Preference
				{
					Id = preferenceId,
					Preference = preference
				};
				_dbContext.N4Preferences.Add(n4Preference);
			}
			else
			{
				n4Preference.Preference = preference;
			}

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutSessionAsync(N4Session session)
	{
		try
		{
			if (session.Id == 0)
				await _dbContext.N4Sessions.AddAsync(session);
			else
				_dbContext.N4Sessions.Attach(session).State = EntityState.Modified;

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutUnboundActionArchiveAsync(N4UnboundActionArchive actionArchive)
	{
		try
		{
			await _dbContext.N4UnboundActionArchives.AddAsync(actionArchive);
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutUnboundActionAsync(N4UnboundAction unboundAction)
	{
		try
		{
			unboundAction.ActionGuid =
				unboundAction.ActionGuid == Guid.Empty ? Guid.NewGuid() : unboundAction.ActionGuid;
			unboundAction.IsArchived = false;
			unboundAction.PublicData = CompressionHelper.CompressAsync(unboundAction.PublicData);
			unboundAction.CompressedPublicBlob =
				unboundAction.PublicData != null && unboundAction.PublicData.Length > 0;
			unboundAction.PrivateData = CompressionHelper.CompressAsync(unboundAction.PrivateData);
			unboundAction.CompressedPrivateBlob =
				unboundAction.PrivateData != null && unboundAction.PrivateData.Length > 0;

			unboundAction.UpdatedDate = DateTime.Now;

			if (unboundAction.CreatedDate < SqlDateTime.MinValue.Value)
				unboundAction.CreatedDate = SqlDateTime.MinValue.Value;
			if (unboundAction.ActionGroup < SqlDateTime.MinValue.Value)
				unboundAction.ActionGroup = SqlDateTime.MinValue.Value;

			if (unboundAction.CreatedDate > SqlDateTime.MaxValue.Value)
				unboundAction.CreatedDate = SqlDateTime.MaxValue.Value;

			if (unboundAction.ActionGroup > SqlDateTime.MaxValue.Value)
				unboundAction.ActionGroup = SqlDateTime.MaxValue.Value;
			if (unboundAction.Id == 0)
			{
				_dbContext.N4UnboundActions.Add(unboundAction);
			}
			else
			{
				// _dbContext.N4UnboundActions.Attach(unboundAction).State = EntityState.Modified;
				var existing = await _dbContext.N4UnboundActions.FirstOrDefaultAsync(x => x.Id == unboundAction.Id);
				if (existing != null)
				{
					existing.ActionGuid = unboundAction.ActionGuid;
					existing.ActionGroup = unboundAction.ActionGroup;
					existing.CreatedDate = unboundAction.CreatedDate;
					existing.UpdatedDate = unboundAction.UpdatedDate;
					existing.UpdatedUserId = unboundAction.UpdatedUserId;
					existing.ModuleId = unboundAction.ModuleId;
					existing.Removed = unboundAction.Removed;
					existing.DataFmtCodePriv = unboundAction.DataFmtCodePriv;
					existing.DataFmtCodeStd = unboundAction.DataFmtCodeStd;
					existing.DataTypeCode = unboundAction.DataTypeCode;
					existing.Description = unboundAction.Description;
					existing.DevTypeCode = unboundAction.DevTypeCode;
					existing.Hidden = unboundAction.Hidden;
					existing.VersionNo = unboundAction.VersionNo;
					existing.IsArchived = unboundAction.IsArchived;
					if (unboundAction.PublicData != null)
					{
						existing.PublicData = unboundAction.PublicData;
						existing.CompressedPublicBlob = unboundAction.CompressedPublicBlob;
					}

					if (unboundAction.PrivateData != null)
					{
						existing.PrivateData = unboundAction.PrivateData;
						existing.CompressedPrivateBlob = unboundAction.CompressedPrivateBlob;
					}
				}
			}

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutUnboundActionReferencesAsync(int unboundActionId, int[] unboundActionReferences)
	{
		try
		{
			if (unboundActionReferences != null)
			{
				if (unboundActionReferences.Length == 0)
				{
					await DeleteUnboundActionReferencesAsync(unboundActionId);
				}
				else
				{
					foreach (var reference in unboundActionReferences)
					{
						var n4UnboundActionReference = new N4UnboundActionReference
						{
							ActionId = unboundActionId,
							Reference = reference
						};
						_dbContext.N4UnboundActionReferences.Add(n4UnboundActionReference);
					}

					await _dbContext.SaveChangesAsync();
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutUserSetupAsync(N4UserSetup userSetup)
	{
		try
		{
			var n4UserSetup = await _dbContext.N4UserSetups.FirstOrDefaultAsync(x =>
				x.UserId == userSetup.UserId && x.ModuleId == userSetup.ModuleId);
			if (userSetup.SetupData == null)
			{
				if (n4UserSetup != null) _dbContext.N4UserSetups.Remove(n4UserSetup);
			}
			else
			{
				if (n4UserSetup == null)
					_dbContext.N4UserSetups.Add(userSetup);
				else
					n4UserSetup.SetupData = userSetup.SetupData;
			}

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task RegisterMobileAppAsync(N4MobileApp mobileApp)
	{
		try
		{
			var existingApp = await _dbContext.N4MobileApps.FirstOrDefaultAsync(x => x.ModuleId == mobileApp.ModuleId);
			if (existingApp == null)
			{
				_dbContext.N4MobileApps.Add(mobileApp);
			}
			else
			{
				existingApp.Name = mobileApp.Name;
				existingApp.Version = mobileApp.Version;
				existingApp.MobileAppType = mobileApp.MobileAppType;
				existingApp.AcceptState = mobileApp.AcceptState;
			}

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task<IEnumerable<N4Action>> SearchActionsAsync(int patientId, DateTime? actionGroup, bool returnLatest,
		int[] dataTypes, int[] moduleIds)
	{
		List<N4Action> result = null;
		try
		{
			var sessionIds = await _dbContext.N4Sessions
				.Where(x => x.PatientId == patientId)
				.Select(x => x.Id)
				.ToListAsync();

			var query = _dbContext.N4Actions.Where(x => sessionIds.Contains(x.SessionId) &&
			                                            !x.IsArchived &&
			                                            (x.Removed == null || !x.Removed.Value));
			if (actionGroup.HasValue)
			{
				var actionGroupDate = actionGroup.Value.ToLocalTime();
				var error = false;
				try
				{
					if (actionGroupDate < SqlDateTime.MinValue.Value)
						actionGroupDate = SqlDateTime.MinValue.Value;
					if (actionGroupDate > SqlDateTime.MaxValue.Value)
						actionGroupDate = SqlDateTime.MaxValue.Value;
				}
				catch (Exception)
				{
					error = true;
				}

				if (!error)
					query = query.Where(x => x.ActionGroup == actionGroupDate);
			}

			if (dataTypes != null && dataTypes.Length > 0)
				query = query.Where(x => x.DataTypeCode.HasValue && dataTypes.Contains(x.DataTypeCode.Value));

			if (moduleIds != null && moduleIds.Length > 0) query = query.Where(x => moduleIds.Contains(x.ModuleId));

			query = query.OrderByDescending(x => x.CreatedDate);

			if (returnLatest)
				query = query.Take(1);

			result = await query.ToListAsync();

			foreach (var action in result)
			{
				action.PublicData = null;
				action.PrivateData = null;
				action.FastViewData = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task UnregisterMobileAppAsync(int moduleId)
	{
		var mobileApp = await GetMobileAppAsync(moduleId);
		var mobileAppPermissions = await GetMobileAppPermissionsAsync(null, moduleId);
		_dbContext.N4MobileAppPermissions.RemoveRange(mobileAppPermissions);
		_dbContext.N4MobileApps.Remove(mobileApp);
		await _dbContext.SaveChangesAsync();
	}

	public async Task UpdateDashboardAlertArchiveAsync(N4DashboardAlertArchive dashboardAlert)
	{
		try
		{
			_dbContext.N4DashboardAlertArchives.Attach(dashboardAlert).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task UpdateDashboardAlertAsync(N4DashboardAlert dashboardAlert)
	{
		try
		{
			_dbContext.N4DashboardAlerts.Attach(dashboardAlert).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task<N4LoginResult> ValidateLoginAsync(string username, string password)
	{
		var loginResult = new N4LoginResult();
		var userData = await _dbContext.Users
			.Where(x => x.Name == username)
			.Select(x => new
			{
				UserId = x.Id,
				x.SiteId,
				x.Password
			})
			.FirstOrDefaultAsync();

		if (userData != null && BCryptHelper.CheckPassword(password, userData.Password))
		{
			loginResult.UserId = userData.UserId;
			loginResult.SiteId = userData.SiteId;
			var conn = _dbContext.Database.GetDbConnection();

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_appSettings.Keys.JwtSecret);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new(StringConstants.User, loginResult.UserId.ToString()),
					new(StringConstants.Site, loginResult.SiteId.ToString()),
					new(StringConstants.Server, conn.DataSource),
					new(StringConstants.Database, conn.Database)
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			loginResult.JwtToken = tokenHandler.WriteToken(token);
		}
		else
		{
			loginResult.UserId = -1;
			loginResult.JwtToken = null;
		}

		return loginResult;
	}

	#endregion NoahQuery Members

	#region Fields

	private readonly AppSettings _appSettings;
	private readonly NoahDbContext _dbContext;

	#endregion Fields
}