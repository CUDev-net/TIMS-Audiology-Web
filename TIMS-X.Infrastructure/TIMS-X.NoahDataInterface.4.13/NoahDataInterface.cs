using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using NoahDataInterfaces;
using NoahDataObjects;
using NOAHErrorLib;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Core.Models;
using TIMS_X.NoahDataInterface413.Models;
using TIMS_X.NoahDataInterface413.Services;
using Action = NoahDataObjects.Action;

namespace TIMS_X.NoahDataInterface413
{
	public class NoahDataInterface : INoah4Database, INoah4UnBoundActionDatabase
	{
		#region Constructors

		public NoahDataInterface()
		{
			//_noahVersion = _GetNoahVersionFromRegistry();
			_dataService = new NoahDataService();
		}

		#endregion Constructors


		#region NoahDataInterface Members

		public int[] ActionReferencedBy(int actionId, object callContext)
		{
			_dataService.Log($"Entering ActionReferencedBy(actionId: {actionId})");

			var actionIds = _dataService.GetActionReferencedBy(actionId);
			_dataService.Log($"ActionReferencedBy(actionId: {actionId}) returned int[{actionIds.Length}]");
			if (actionIds.Length == 0) actionIds = null;
			return actionIds;
		}

		public void ArchiveAction(int actionId, object callContext)
		{
			_dataService.Log($"Entering ArchiveAction(actionId: {actionId})");

			_dataService.PutArchiveAction(actionId);
		}

		public void ArchiveUnBoundAction(int actionId, object callContext)
		{
			_dataService.Log($"Entering ArchiveUnBoundAction(actionId: {actionId})");

			_dataService.PutArchiveUnboundAction(actionId);
		}

		public void CreateArchivedDashboardAlert(DashboardAlert alert, object callContext)
		{
			_dataService.Log("Entering CreateDashboardAlert(alert, callContext)");

			var n4Alert = new N4DashboardAlertArchive
			{
				ActionId = alert.ActionID,
				AlertGuid = alert.AlertGUID,
				AppModuleId = alert.AppModuleID,
				AssigneeUserId = alert.AssigneeUserID,
				Category = alert.Category,
				Description = alert.Description,
				Group = alert.Group,
				IconUrl = alert.IconUrl,
				IsRead = alert.IsRead,
				LastModifiedUtc = alert.LastModifiedUtc,
				ModuleId = alert.ModuleID,
				ModuleParameter = alert.ModuleParameter,
				NotificationActionId = alert.NotificationActionID,
				PatientGuid = alert.PatientGUID,
				Priority = (int)alert.Priority,
				ReceivedUtc = alert.ReceivedUtc,
				Url = alert.Url
			};

			_dataService.PutDashboardAlertArchive(n4Alert);
		}

		public void CreateDashboardAlert(DashboardAlert alert, object callContext)
		{
			_dataService.Log("Entering CreateDashboardAlert(alert, callContext)");

			var n4Alert = new N4DashboardAlert
			{
				ActionId = alert.ActionID,
				AlertGuid = alert.AlertGUID,
				AppModuleId = alert.AppModuleID,
				AssigneeUserId = alert.AssigneeUserID,
				Category = alert.Category,
				Description = alert.Description,
				Group = alert.Group,
				IconUrl = alert.IconUrl,
				IsRead = alert.IsRead,
				LastModifiedUtc = alert.LastModifiedUtc,
				ModuleId = alert.ModuleID,
				ModuleParameter = alert.ModuleParameter,
				NotificationActionId = alert.NotificationActionID,
				PatientGuid = alert.PatientGUID,
				Priority = (int)alert.Priority,
				ReceivedUtc = alert.ReceivedUtc,
				Url = alert.Url
			};

			_dataService.PutDashboardAlert(n4Alert);
		}

		public object DataAccessNoLogin(object obj)
		{
			throw new NotImplementedException();
		}

		public void DelAppPermissions(AppPermission[] appPermissions, object callContext)
		{
			_dataService.Log("Entering DelAppPermissions(appPermissions)");
			var appPermissionIds = appPermissions.Select(x => x.PermissionID).ToArray();
			_dataService.DeleteAppPermissions(appPermissionIds);
		}

		public void DeleteAction(int actionId, object callContext)
		{
			_dataService.Log($"Entering DeleteAction(actionId: {actionId})");

			_dataService.DeleteAction(actionId);
		}

		public void DeleteActionReferences(int actionId, object callContext)
		{
			_dataService.Log($"Entering DeleteActionReferences(actionId: {actionId})");

			_dataService.DeleteActionReferences(actionId);
		}

		public void DeleteArchivedActions(int actionId, object callContext)
		{
			_dataService.Log($"Entering DeleteArchivedActions(actionId: {actionId})");

			_dataService.DeleteArchivedActions(actionId);
		}

		public void DeleteArchivedDashboardAlert(Guid alertGUID, object callContext)
		{
			_dataService.Log($"Entering DeleteArchivedDashboardAlert(alertGuid: {alertGUID})");

			_dataService.DeleteDashboardAlertArchive(alertGUID);
		}

		public void DeleteArchivedUnBoundActions(int actionId, object callContext)
		{
			_dataService.Log($"Entering DeleteArchivedUnBoundActions(actionId: {actionId})");

			_dataService.DeleteArchivedUnboundActions(actionId);
		}

		public void DeleteDashboardAlert(Guid alertGUID, object callContext)
		{
			_dataService.Log($"Entering DeleteDashboardAlert(alertGuid: {alertGUID})");

			_dataService.DeleteDashboardAlert(alertGUID);
		}

		public void DeleteFastView(int actionId, object callContext)
		{
			_dataService.Log($"Entering DeleteFastView(actionId: {actionId})");

			_dataService.DeleteFastView(actionId);
		}

		/// <summary>
		///     We only allow users to delete patients in TIMS
		/// </summary>
		public void DeletePatient(int patientId, object callContext)
		{
			_dataService.Log($"Entering DeletePatient(patientId: {patientId})");
			throw new NoahDatabaseException(NOAHErrorType.NOAH_E_NOTSUPPORTED);
		}

		public void DeletePatientIdentification(int patientId, int manufacturerId, string identification,
			object callContext)
		{
			_dataService.Log(
				$"Entering DeletePatientIdentification(patientId: {patientId}, manufactureID: {manufacturerId}, identification: {identification})");

			_dataService.DeletePatientIdentification(patientId, manufacturerId);
		}

		public void DeletePatientSetup(int patientId, int moduleId, object callContext)
		{
			_dataService.Log($"Entering DeletePatientSetup(patientId: {patientId}, moduleId: {moduleId})");

			_dataService.DeleteUserSetup(patientId, moduleId);
		}

		public void DeleteSession(int sessionId, object callContext)
		{
			_dataService.Log($"Entering DeleteSession(sessionId: {sessionId})");
			var n4Session = _dataService.GetSession(sessionId);
			_dataService.DeleteSession(sessionId);
		}

		public void DeleteUnBoundAction(int actionId, object callContext)
		{
			_dataService.Log($"Entering DeleteUnBoundAction(actionId: {actionId})");

			_dataService.DeleteUnboundAction(actionId);
		}

		public void DeleteUnBoundActionReferences(int actionId, object callContext)
		{
			_dataService.Log($"Entering DeleteUnBoundActionReferences(actionId: {actionId})");

			_dataService.DeleteUnboundActionReferences(actionId);
		}

		public void DeleteUserSetup(int userId, int moduleId, object callContext)
		{
			_dataService.Log($"Entering DeleteUserSetup(userId: {userId}, moduleId: {moduleId})");
			var user = GetUser(userId, callContext);
			_dataService.DeleteUserSetup(userId, moduleId);
		}

		public void DelMobileAppPermissions(int moduleId, int[] permissionIds, object callContext)
		{
			_dataService.Log($"Entering DelMobileAppPermissions(moduleId: {moduleId})");

			_dataService.DeleteMobileAppPermissions(moduleId, permissionIds);
		}

		public Action GetAction(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetAction(actionId: {actionId})");

			var n4Action = _dataService.GetAction(actionId);
			var action = new Action
			{
				ID = n4Action.Id,
				SessionID = n4Action.SessionId,
				CreateDate = n4Action.CreatedDate,
				LastModifiedDate = n4Action.UpdatedDate.HasValue
					? n4Action.UpdatedDate == SqlDateTime.MinValue.Value
						? DateTime.MinValue
						: n4Action.UpdatedDate.Value
					: SqlDateTime.MinValue.Value,
				UserID = n4Action.UpdatedUserId ?? 0,
				ModuleID = n4Action.ModuleId,
				DeviceType = n4Action.DevTypeCode.HasValue ? n4Action.DevTypeCode.Value : (short)0,
				DataTypeCode = n4Action.DataTypeCode.HasValue ? n4Action.DataTypeCode.Value : (short)0,
				DataFmtStd = n4Action.DataFmtCodeStd.HasValue ? n4Action.DataFmtCodeStd.Value : (short)0,
				DataFmtExt = n4Action.DataFmtCodePriv.HasValue ? n4Action.DataFmtCodePriv.Value : (short)0,
				Description = n4Action.Description,
				Removed = n4Action.Removed ?? false,
				Hidden = n4Action.Hidden ?? false,
				ActionGroup = n4Action.ActionGroup ?? SqlDateTime.MinValue.Value,
				ActionGUID = n4Action.ActionGuid
			};

			_dataService.Log($"GetAction(actionId: {actionId}) returned action object");

			return action;
		}

		public Action GetAction(Guid actionGuid, object callContext)
		{
			_dataService.Log($"Entering GetAction(actionGuid: {actionGuid})");

			var n4Action = _dataService.GetAction(actionGuid);
			if (n4Action == null)
				return null;
			var action = new Action
			{
				ID = n4Action.Id,
				SessionID = n4Action.SessionId,
				CreateDate = n4Action.CreatedDate,
				LastModifiedDate = n4Action.UpdatedDate.HasValue
					? n4Action.UpdatedDate == SqlDateTime.MinValue.Value
						? DateTime.MinValue
						: n4Action.UpdatedDate.Value
					: SqlDateTime.MinValue.Value,
				UserID = n4Action.UpdatedUserId ?? 0,
				ModuleID = n4Action.ModuleId,
				DeviceType = n4Action.DevTypeCode.HasValue ? n4Action.DevTypeCode.Value : (short)0,
				DataTypeCode = n4Action.DataTypeCode.HasValue ? n4Action.DataTypeCode.Value : (short)0,
				DataFmtStd = n4Action.DataFmtCodeStd.HasValue ? n4Action.DataFmtCodeStd.Value : (short)0,
				DataFmtExt = n4Action.DataFmtCodePriv.HasValue ? n4Action.DataFmtCodePriv.Value : (short)0,
				Description = n4Action.Description,
				Removed = n4Action.Removed ?? false,
				Hidden = n4Action.Hidden ?? false,
				ActionGroup = n4Action.ActionGroup ?? SqlDateTime.MinValue.Value,
				ActionGUID = n4Action.ActionGuid
			};

			_dataService.Log($"GetAction(actionId: {actionGuid}) returned action object");

			return action;
		}

		public Action[] GetActionCollection(int moduleId, object callContext)
		{
			_dataService.Log($"Entering GetActionCollection(moduleId: {moduleId})");

			var n4Actions = _dataService.GetActionCollection(moduleId);

			var actions = n4Actions.Select(action => new Action
			{
				ID = action.Id,
				SessionID = action.SessionId,
				CreateDate = action.CreatedDate,
				LastModifiedDate = action.UpdatedDate.HasValue
					? action.UpdatedDate == SqlDateTime.MinValue.Value ? DateTime.MinValue : action.UpdatedDate.Value
					: SqlDateTime.MinValue.Value,
				UserID = action.UpdatedUserId ?? 0,
				ModuleID = action.ModuleId,
				DeviceType = action.DevTypeCode.HasValue ? action.DevTypeCode.Value : (short)0,
				DataTypeCode = action.DataTypeCode.HasValue ? action.DataTypeCode.Value : (short)0,
				DataFmtStd = action.DataFmtCodeStd.HasValue ? action.DataFmtCodeStd.Value : (short)0,
				DataFmtExt = action.DataFmtCodePriv.HasValue ? action.DataFmtCodePriv.Value : (short)0,
				Description = action.Description,
				Removed = action.Removed ?? false,
				Hidden = action.Hidden ?? false,
				ActionGroup = action.ActionGroup ?? SqlDateTime.MinValue.Value,
				ActionGUID = action.ActionGuid
			}).ToArray();

			_dataService.Log($"GetActionCollection(moduleId: {moduleId}) returned Action[{actions.Length}]");

			return actions;
		}

		public int GetActionCollectionCount(int moduleId, object callContext)
		{
			_dataService.Log($"Entering GetActionCollectionCount(moduleId: {moduleId})");

			var count = _dataService.GetActionCollectionCount(moduleId);

			_dataService.Log($"GetActionCollectionCount(moduleId: {moduleId}) returned {count}");

			return count;
		}

		public void GetActionData(int actionId, out byte[] publicData, out byte[] privateData, object callContext)
		{
			_dataService.Log($"Entering GetActionData(actionId: {actionId})");

			var n4ActionData = _dataService.GetActionData(actionId);
			publicData = n4ActionData?.PublicData;
			privateData = n4ActionData?.PrivateData;

			_dataService.Log($"GetActionData(actionId: {actionId}) returned action data object");
		}

		public int[] GetActionIDsFromDashboardGroup(Guid group, object callContext)
		{
			_dataService.Log($"Entering GetActionIDsFromDashboardGroup(group: {group})");
			if (group == Guid.Empty) return new int[0];

			var ids = _dataService.GetActionIdsFromDashboardGroup(group);
			return ids;
		}

		public int[] GetActionReferences(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetActionReferences(actionId: {actionId})");
			var timer = new Stopwatch();
			timer.Start();
			var n4ActionReferences = _dataService.GetActionReferences(actionId);
			timer.Stop();
			_dataService.Log(
				$"GetActionReferences(actionId: {actionId}) returned int[{n4ActionReferences?.Length ?? 0}]. {timer.Elapsed.TotalSeconds} secs");

			return n4ActionReferences;
		}

		public Action[] GetActions(int sessionId, object callContext)
		{
			_dataService.Log($"Entering GetActions(sessionId: {sessionId})");
			var timer = new Stopwatch();
			timer.Start();
			var n4Actions = _dataService.GetActions(sessionId);
			var actionList = new List<Action>();
			if (n4Actions != null)
				foreach (var action in n4Actions)
				{
					var newAction = new Action
					{
						ID = action.Id,
						SessionID = action.SessionId,
						CreateDate = action.CreatedDate,
						LastModifiedDate = action.UpdatedDate.HasValue
							? action.UpdatedDate == SqlDateTime.MinValue.Value
								? DateTime.MinValue
								: action.UpdatedDate.Value
							: SqlDateTime.MinValue.Value,
						UserID = action.UpdatedUserId ?? 0,
						ModuleID = action.ModuleId,
						DeviceType = action.DevTypeCode.HasValue ? action.DevTypeCode.Value : (short)0,
						DataTypeCode = action.DataTypeCode.HasValue ? action.DataTypeCode.Value : (short)0,
						DataFmtStd = action.DataFmtCodeStd.HasValue ? action.DataFmtCodeStd.Value : (short)0,
						DataFmtExt = action.DataFmtCodePriv.HasValue ? action.DataFmtCodePriv.Value : (short)0,
						Description = action.Description,
						Removed = action.Removed ?? false,
						Hidden = action.Hidden ?? false,
						ActionGroup = action.ActionGroup ?? SqlDateTime.MinValue.Value,
						ActionGUID = action.ActionGuid,
						UserInitials = "???"
					};
					var user = GetUserOrNull(action.UpdatedUserId ?? 0, callContext);
					if (user != null) newAction.UserInitials = user.UserInitials;

					actionList.Add(newAction);
				}

			var actions = actionList.ToArray();
			timer.Stop();
			_dataService.Log(
				$"GetActions(sessionId: {sessionId}) and returned Action[{actions?.Length ?? 0}]. {timer.Elapsed.TotalSeconds} secs");

			return actions;
		}

		public Action[] GetActions(int patientId, ActionFilter actionFilter, object callContext)
		{
			_dataService.Log($"Entering GetActions(patientId: {patientId}, actionFilter)");

			var n4Actions = _dataService.GetActions(patientId, actionFilter.ActionGroup,
				actionFilter.ActionsToReturn == ActionsToReturn.Latest, actionFilter.DataTypes, actionFilter.ModuleIDs);
			if (n4Actions == null || n4Actions.Count() == 0)
				return null;
			var actions = n4Actions.Select(action => new Action
			{
				ID = action.Id,
				SessionID = action.SessionId,
				CreateDate = action.CreatedDate == SqlDateTime.MinValue.Value ? DateTime.MinValue : action.CreatedDate,
				LastModifiedDate = action.UpdatedDate.HasValue
					? action.UpdatedDate == SqlDateTime.MinValue.Value ? DateTime.MinValue : action.UpdatedDate.Value
					: SqlDateTime.MinValue.Value,
				UserID = action.UpdatedUserId ?? 0,
				ModuleID = action.ModuleId,
				DeviceType = action.DevTypeCode.HasValue ? action.DevTypeCode.Value : (short)0,
				DataTypeCode = action.DataTypeCode.HasValue ? action.DataTypeCode.Value : (short)0,
				DataFmtStd = action.DataFmtCodeStd.HasValue ? action.DataFmtCodeStd.Value : (short)0,
				DataFmtExt = action.DataFmtCodePriv.HasValue ? action.DataFmtCodePriv.Value : (short)0,
				Description = action.Description,
				Removed = action.Removed ?? false,
				Hidden = action.Hidden ?? false,
				ActionGroup = action.ActionGroup ?? SqlDateTime.MinValue.Value,
				ActionGUID = action.ActionGuid,
				UserInitials = "???"
			}).ToArray();

			if (actionFilter.ActionsToReturn == ActionsToReturn.Latest && actions.Length > 1)
				actions = new[] { actions.First() };

			foreach (var action in actions)
			{
				var user = GetUserOrNull(action.UserID, callContext);
				if (user != null) action.UserInitials = user.UserInitials;
			}

			_dataService.Log($"GetActions(patientId: {patientId}, actionFilter) returned Action[{actions.Length}]");

			return actions;
		}

		public AppPermission[] GetAppPermissions(object callContext)
		{
			_dataService.Log("Entering GetAppPermissions()");

			var appPermissions = _dataService.GetAppPermissions().Select(x => new AppPermission
			{
				PermissionID = x.Id,
				Default = x.Default,
				MaxLength = x.MaxLength,
				MinLength = x.MinLength,
				Name = x.Name,
				ReadOnly = x.ReadOnly,
				Required = x.Required,
				Type = (DataType)x.Type
			}).ToArray();

			_dataService.Log($"GetAppPermissions() returned AppPermission[{appPermissions.Length}]");

			return appPermissions;
		}

		public Action[] GetArchivedActions(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetArchivedActions(actionId: {actionId})");

			var n4Actions = _dataService.GetArchivedActions(actionId);
			var actionList = new List<Action>();
			foreach (var action in n4Actions)
				actionList.Add(new Action
				{
					ID = action.Id,
					SessionID = action.SessionId,
					CreateDate = action.CreatedDate,
					LastModifiedDate = action.UpdatedDate.HasValue
						? action.UpdatedDate == SqlDateTime.MinValue.Value
							? DateTime.MinValue
							: action.UpdatedDate.Value
						: SqlDateTime.MinValue.Value,
					UserID = action.UpdatedUserId ?? 0,
					ModuleID = action.ModuleId,
					DeviceType = action.DevTypeCode.HasValue ? action.DevTypeCode.Value : (short)0,
					DataTypeCode = action.DataTypeCode.HasValue ? action.DataTypeCode.Value : (short)0,
					DataFmtStd = action.DataFmtCodeStd.HasValue ? action.DataFmtCodeStd.Value : (short)0,
					DataFmtExt = action.DataFmtCodePriv.HasValue ? action.DataFmtCodePriv.Value : (short)0,
					Description = action.Description,
					Removed = action.Removed ?? false,
					Hidden = action.Hidden ?? false,
					ActionGroup = action.ActionGroup ?? SqlDateTime.MinValue.Value,
					ActionGUID = action.ActionGuid
				});

			var actions = actionList.ToArray();
			_dataService.Log($"GetArchivedActions(actionId: {actionId}) returned Action[{actions?.Length ?? 0}]");

			return actions;
		}

		public int GetArchivedActionsCount(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetArchivedActionsCount(actionId: {actionId})");

			var archivedActionsCount = _dataService.GetArchivedActionsCount(actionId);

			_dataService.Log($"GetArchivedActionsCount(actionId: {actionId}) returned {archivedActionsCount}");

			return archivedActionsCount;
		}

		public UnBoundAction[] GetArchivedUnBoundActions(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetArchivedUnBoundActions(actionId: {actionId})");

			var n4UnboundActions = _dataService.GetArchivedUnboundActions(actionId);
			var actionList = new List<UnBoundAction>();
			foreach (var action in n4UnboundActions)
				actionList.Add(new UnBoundAction
				{
					ID = action.Id,
					CreateDate = action.CreatedDate,
					LastModifiedDate = action.UpdatedDate,
					UserID = action.UpdatedUserId ?? 0,
					ModuleID = action.ModuleId,
					DeviceType = action.DevTypeCode.HasValue ? action.DevTypeCode.Value : (short)0,
					DataTypeCode = action.DataTypeCode.HasValue ? action.DataTypeCode.Value : (short)0,
					DataFmtStd = action.DataFmtCodeStd.HasValue ? action.DataFmtCodeStd.Value : (short)0,
					DataFmtExt = action.DataFmtCodePriv.HasValue ? action.DataFmtCodePriv.Value : (short)0,
					Description = action.Description,
					Removed = action.Removed,
					Hidden = action.Hidden,
					ActionGroup = action.ActionGroup,
					ActionGUID = action.ActionGuid
				});

			var actions = actionList.ToArray();
			_dataService.Log(
				$"GetArchivedUnBoundActions(actionId: {actionId}) returned Action[{actions?.Length ?? 0}]");

			return actions;
		}

		public int GetArchivedUnBoundActionsCount(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetArchivedUnBoundActionsCount(actionId: {actionId})");

			var count = _dataService.GetArchivedUnboundActionsCount(actionId);

			_dataService.Log($"GetArchivedUnBoundActionsCount(actionId: {actionId}) returned {count}");

			return count;
		}

		public DashboardAlert[] GetDashboardAlerts(DashboardAlertSearchOptions searchOptions, object callContext)
		{
			_dataService.Log("Entering GetDashboardAlerts(searchOptions)");

			var alerts = _dataService.GetDashboardAlerts(searchOptions);
			_dataService.Log($"GetDashboardAlerts(searchOptions) returned DashboardAlert[{alerts?.Length ?? 0}");

			var result = alerts?.Select(alert => new DashboardAlert
			{
				ActionID = alert.ActionId,
				AlertGUID = alert.AlertGuid,
				AppModuleID = alert.AppModuleId,
				AssigneeUserID = alert.AssigneeUserId,
				Category = alert.Category,
				Description = alert.Description,
				Group = alert.Group,
				IconUrl = alert.IconUrl,
				IsRead = alert.IsRead,
				LastModifiedUtc = alert.LastModifiedUtc,
				ModuleID = alert.ModuleId,
				ModuleParameter = alert.ModuleParameter,
				NotificationActionID = alert.NotificationActionId,
				PatientGUID = alert.PatientGuid,
				Priority = (Priority)alert.Priority,
				ReceivedUtc = alert.ReceivedUtc,
				Url = alert.Url
			}).ToArray() ?? new DashboardAlert[] { };

			return result;
		}

		public void GetFastView(int actionId, out FastView fastView, object callContext)
		{
			_dataService.Log($"Entering GetFastView(actionId: {actionId})");

			var n4FastView = _dataService.GetFastView(actionId);

			if (n4FastView == null)
			{
				_dataService.Log($"GetFastView(actionId: {actionId}) returned null object");
				fastView = null;
				return;
			}

			fastView = new FastView
			{
				Version = n4FastView.Version,
				CreateDate = n4FastView.CreatedDate,
				LastModifiedDate = n4FastView.UpdatedDate,
				Data = n4FastView.Data,
				Format = (FastView.DataFormat)n4FastView.Format
			};

			_dataService.Log($"GetFastView(actionId: {actionId}) returned FastView object");
		}

		public ManufacturerSetup[] GetManufacturerSetup(int manufacturerId, string[] keys, object callContext)
		{
			_dataService.Log($"Entering GetManufacturerSetup(manufacturerId: {manufacturerId})");

			var manufacturerSetups = _dataService.GetManufacturerSetups(manufacturerId, keys).ToList();

			_dataService.Log(
				$"GetManufacturerSetup(manufacturerId: {manufacturerId}) returned ManufacturerSetup[{manufacturerSetups.Count()}]");

			return manufacturerSetups.Select(x => new ManufacturerSetup
			{
				ManufacturerID = x.ManufacturerId,
				Key = x.Key,
				Data = x.SetupData
			}).ToArray();
		}

		public string[] GetManufacturerSetupKeys(int manufacturerId, object callContext)
		{
			_dataService.Log($"Entering GetManufacturerSetupKeys(manufacturerId: {manufacturerId})");

			var setupKeys = _dataService.GetManufacturerSetupKeys(manufacturerId);

			_dataService.Log(
				$"GetManufacturerSetupKeys(manufacturerId: {manufacturerId}) returned string[{setupKeys.Count()}]");

			return setupKeys;
		}

		public int[] GetMobileAppPermissions(int moduleId, object callContext)
		{
			_dataService.Log($"Entering GetMobileAppPermissions(moduleId: {moduleId})");

			var mobileAppPermissions = _dataService.GetMobileAppPermissions(moduleId);
			_dataService.Log(
				$"GetMobileAppPermissions(moduleId: {moduleId}) returned int[{mobileAppPermissions.Count()}]");

			return mobileAppPermissions;
		}

		private AppDataRequirements _DeserializeAppDataRequirements(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
				return null;
			var xmlSerializer = new XmlSerializer(typeof(AppDataRequirements));
			var reader = new StringReader(xml);
			var adr = (AppDataRequirements)xmlSerializer.Deserialize(reader);
			return adr;
		}

		private string _SerializeAppDataRequirements(AppDataRequirements adr)
		{
			if (adr == null)
				return null;
			var xmlSerializer = new XmlSerializer(typeof(AppDataRequirements));
			var writer = new StringWriter();
			xmlSerializer.Serialize(writer, adr);
			return writer.ToString();
		}


		public MobileApp[] GetMobileApps(object callContext)
		{
			_dataService.Log("Entering GetMobileApps()");
			var n4MobileApps = _dataService.GetMobileApps();
			//if (n4MobileApps == null || n4MobileApps.Count() == 0)
			//	return null;
			var mobileApps = n4MobileApps.Select(x => new MobileApp
			{
				ModuleID = x.ModuleId,
				AcceptState = (AcceptState)x.AcceptState,
				MobileAppType = (AppType)x.MobileAppType,
				Name = x.Name,
				Version = x.Version,
				ActiveRequirements = _DeserializeAppDataRequirements(x.ActiveRequirements),
				PendingRequirements = _DeserializeAppDataRequirements(x.PendingRequirements)
			}).ToArray();

			_dataService.Log($"GetMobileApps() returned MobileApp[{mobileApps.Length}]");

			return mobileApps;
		}

		public Patient GetPatient(Guid patientGuid, object callContext)
		{
			_dataService.Log($"Entering GetPatient(patientGuid: {patientGuid})");
			var timer = new Stopwatch();
			timer.Start();
			var n4Patient = _dataService.GetPatient(patientGuid);

			if (n4Patient == null) throw new NoahDatabaseException(NOAHErrorType.NOAH_E_DB_CLIENT_NOT_FOUND);

			var patient = new Patient
			{
				ID = n4Patient.Id,
				PatientGUID = n4Patient.PatientGuid,
				ActivePatient = n4Patient.ActivePatient,
				PatientNo = n4Patient.PatientNo,
				CreateDate = n4Patient.DateCreated,
				FirstName = n4Patient.FirstName,
				MiddleName = n4Patient.MiddleName,
				LastName = n4Patient.LastName,
				Gender = n4Patient.Gender,
				BirthDate = n4Patient.Birthdate,
				Salutation = n4Patient.Salutation,
				Title = n4Patient.Title,
				Address1 = n4Patient.Address1,
				Address2 = n4Patient.Address2,
				Address3 = string.Empty,
				Province = n4Patient.State,
				City = n4Patient.City,
				Zip = n4Patient.Zip,
				Country = string.Empty,
				HomePhone = n4Patient.HomePhone,
				WorkPhone = n4Patient.WorkPhone,
				MobilePhone = n4Patient.MobilePhone,
				EMail = n4Patient.Email,
				SSNumber = n4Patient.Ssn,
				Insurance1 = n4Patient.Insurance1,
				Insurance2 = n4Patient.Insurance2,
				Other1 = n4Patient.Other1,
				Other2 = n4Patient.Other2,
				Physician = n4Patient.Physician,
				Referral = n4Patient.Referral,
				CreatedBy = n4Patient.CreatedBy
			};

			timer.Stop();
			_dataService.Log(
				$"GetPatient(patientGuid: {patientGuid}) returned Patient '{patient.LastName}, {patient.FirstName}'. {timer.Elapsed.TotalSeconds} secs");

			return patient;
		}

		public Patient GetPatient(int patientId, object callContext)
		{
			_dataService.Log($"Entering GetPatient(patientId: {patientId})");
			var timer = new Stopwatch();
			timer.Start();
			var n4Patient = _dataService.GetPatient(patientId);

			if (n4Patient == null) throw new NoahDatabaseException(NOAHErrorType.NOAH_E_DB_CLIENT_NOT_FOUND);

			var patient = new Patient
			{
				ID = patientId,
				PatientGUID = n4Patient.PatientGuid,
				ActivePatient = n4Patient.ActivePatient,
				PatientNo = n4Patient.PatientNo,
				CreateDate = n4Patient.DateCreated,
				FirstName = n4Patient.FirstName,
				MiddleName = n4Patient.MiddleName,
				LastName = n4Patient.LastName,
				Gender = n4Patient.Gender,
				BirthDate = n4Patient.Birthdate,
				Salutation = n4Patient.Salutation,
				Title = n4Patient.Title,
				Address1 = n4Patient.Address1,
				Address2 = n4Patient.Address2,
				Address3 = string.Empty,
				Province = n4Patient.State,
				City = n4Patient.City,
				Zip = n4Patient.Zip,
				Country = string.Empty,
				HomePhone = n4Patient.HomePhone,
				WorkPhone = n4Patient.WorkPhone,
				MobilePhone = n4Patient.MobilePhone,
				EMail = n4Patient.Email,
				SSNumber = n4Patient.Ssn,
				Insurance1 = n4Patient.Insurance1,
				Insurance2 = n4Patient.Insurance2,
				Other1 = n4Patient.Other1,
				Other2 = n4Patient.Other2,
				Physician = n4Patient.Physician,
				Referral = n4Patient.Referral,
				CreatedBy = n4Patient.CreatedBy
			};

			timer.Stop();
			_dataService.Log(
				$"GetPatient(patientId: {patientId}) returned Patient '{patient.LastName}, {patient.FirstName}'. {timer.Elapsed.TotalSeconds} secs");

			return patient;
		}

		public int[] GetPatientFromIdentification(string identification, int manufacturerId, object callContext)
		{
			_dataService.Log(
				$"Entering GetPatientFromIdentification(identification: {identification}, manufactureID: {manufacturerId})");

			var patientIds = _dataService.GetPatientIdsFromIdentification(identification, manufacturerId);

			_dataService.Log(
				$"GetPatientFromIdentification(identification: {identification}, manufactureID: {manufacturerId}) returned int[{patientIds.Length}]");

			return patientIds;
		}

		public Patient[] GetPatients(string searchText, int page, int pageSize, ref int? queryTotalNoOfPatients,
			object callContext)
		{
			searchText = searchText == null ? string.Empty : searchText;
			_dataService.Log($"Entering GetPatients(searchText: {searchText}, page: {page}, pageSize: {pageSize})");

			var n4Patients = _dataService.GetPatients(searchText, page, pageSize);
			if (n4Patients == null) return null;
			var patients = n4Patients.Select(p => new Patient
			{
				ID = p.Id,
				PatientGUID = p.PatientGuid,
				ActivePatient = p.ActivePatient,
				PatientNo = p.PatientNo,
				CreateDate = p.DateCreated,
				FirstName = p.FirstName,
				MiddleName = p.MiddleName,
				LastName = p.LastName,
				Gender = p.Gender,
				BirthDate = p.Birthdate,
				Salutation = p.Salutation,
				Title = p.Title,
				Address1 = p.Address1,
				Address2 = p.Address2,
				Address3 = string.Empty,
				Province = p.State,
				City = p.City,
				Zip = p.Zip,
				Country = string.Empty,
				HomePhone = p.HomePhone,
				WorkPhone = p.WorkPhone,
				MobilePhone = p.MobilePhone,
				EMail = p.Email,
				SSNumber = p.Ssn,
				Insurance1 = p.Insurance1,
				Insurance2 = p.Insurance2,
				Other1 = p.Other1,
				Other2 = p.Other2,
				Physician = p.Physician,
				Referral = p.Referral,
				CreatedBy = p.CreatedBy
			}).ToArray();

			if (queryTotalNoOfPatients.HasValue && queryTotalNoOfPatients.Value == 0)
				queryTotalNoOfPatients = patients.Length;

			_dataService.Log(
				$"GetPatients(searchText: {searchText}, page: {page}, pageSize: {pageSize}) returned Patient[{patients.Length}]");

			return patients;
		}

		public byte[] GetPatientSetup(int patientId, int moduleId, object callContext)
		{
			_dataService.Log($"Entering GetPatientSetup(patientId: {patientId}, moduleId: {moduleId})");
			var patient = GetPatient(patientId, callContext);
			byte[] patientSetupData = null;
			var n4PatientSetup = _dataService.GetPatientSetup(patientId, moduleId);
			if (n4PatientSetup != null) patientSetupData = n4PatientSetup.SetupData;

			_dataService.Log(
				$"GetPatientSetup(patientId: {patientId}, moduleId: {moduleId}) returned byte[{patientSetupData?.Length ?? 0}]");

			return patientSetupData;
		}

		public PatientSetup[] GetPatientSetupForCompanyModules(int moduleId, object callContext)
		{
			_dataService.Log($"Entering GetPatientSetupForCompanyModules(moduleId: {moduleId})");

			var n4PatientSetups = _dataService.GetPatientSetupsForModule(moduleId);
			var patientSetupList = new List<PatientSetup>();
			foreach (var n4PatientSetup in n4PatientSetups)
			{
				var patientSetup = new PatientSetup
				{
					PatientId = n4PatientSetup.PatientId,
					ModuleId = n4PatientSetup.ModuleId,
					Setup = n4PatientSetup.SetupData
				};
				patientSetupList.Add(patientSetup);
			}

			var patientSetups = patientSetupList.ToArray();
			_dataService.Log(
				$"GetPatientSetupForCompanyModules(moduleId: {moduleId}) returned PatientSetup[{patientSetups?.Length ?? 0}]");

			return patientSetups;
		}

		public PatientSetup[] GetPatientsSetup(int patientId, int moduleId, object callContext)
		{
			_dataService.Log($"Entering GetPatientsSetup(patientId: {patientId}, moduleId: {moduleId})");

			var n4PatientSetups = _dataService.GetPatientSetups(patientId, moduleId);
			var patientSetupList = new List<PatientSetup>();
			foreach (var n4PatientSetup in n4PatientSetups)
			{
				var patientSetup = new PatientSetup
				{
					PatientId = n4PatientSetup.PatientId,
					ModuleId = n4PatientSetup.ModuleId,
					Setup = n4PatientSetup.SetupData
				};
				patientSetupList.Add(patientSetup);
			}

			var patientSetups = patientSetupList.ToArray();
			_dataService.Log(
				$"GetPatientsSetup(patientId: {patientId}, moduleId: {moduleId}) returned PatientSetup[{patientSetups?.Length ?? 0}]");

			return patientSetups;
		}

		public byte[] GetPreference(int id, object callContext)
		{
			_dataService.Log($"Entering GetPreference(id: {id})");

			var preference = _dataService.GetPreference(id);

			if (preference == null) throw new NoahDatabaseException(NOAHErrorType.NOAH_E_DB_PREFERENCE_NOT_FOUND);
			_dataService.Log($"GetPreference(id: {id}) returned byte[{preference.Preference?.Length ?? 0}");

			return preference.Preference;
		}

		public User GetServiceAppUser(int ModuleID, object callContext)
		{
			throw new NotImplementedException();
		}

		public Session GetSession(int sessionId, object callContext)
		{
			_dataService.Log($"Entering GetSession(sessionId: {sessionId})");
			var timer = new Stopwatch();
			timer.Start();
			var n4Session = _dataService.GetSession(sessionId);
			var session = new Session
			{
				ID = n4Session.Id,
				PatientID = n4Session.PatientId,
				CreateDate = n4Session.CreateDate ?? DateTime.Now
			};
			timer.Stop();
			_dataService.Log(
				$"GetSession(sessionId: {sessionId}) returned session object. {timer.Elapsed.TotalSeconds} secs");

			return session;
		}

		public Session[] GetSessions(int patientId, object callContext)
		{
			_dataService.Log($"Entering GetSessions(patientId: {patientId})");
			var timer = new Stopwatch();
			timer.Start();
			var n4Sessions = _dataService.GetSessions(patientId);
			var sessionList = new List<Session>();
			foreach (var session in n4Sessions)
				sessionList.Add(new Session
				{
					ID = session.Id,
					PatientID = session.PatientId,
					CreateDate = session.CreateDate ?? DateTime.Now
				});

			var sessions = sessionList.ToArray();
			timer.Stop();
			_dataService.Log(
				$"GetSessions(patientId: {patientId}) returned Session[{sessions?.Length ?? 0}]. {timer.Elapsed.TotalSeconds} secs");

			return sessions;
		}


		public UnBoundAction GetUnBoundAction(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetUnBoundAction(actionId: {actionId})");

			var n4UnboundAction = _dataService.GetUnboundAction(actionId);
			var unboundAction = new UnBoundAction
			{
				ID = n4UnboundAction.Id,
				CreateDate = n4UnboundAction.CreatedDate,
				LastModifiedDate = n4UnboundAction.UpdatedDate,
				UserID = n4UnboundAction.UpdatedUserId ?? 0,
				ModuleID = n4UnboundAction.ModuleId,
				DeviceType = n4UnboundAction.DevTypeCode.HasValue ? n4UnboundAction.DevTypeCode.Value : (short)0,
				DataTypeCode = n4UnboundAction.DataTypeCode.HasValue ? n4UnboundAction.DataTypeCode.Value : (short)0,
				DataFmtStd = n4UnboundAction.DataFmtCodeStd.HasValue ? n4UnboundAction.DataFmtCodeStd.Value : (short)0,
				DataFmtExt =
					n4UnboundAction.DataFmtCodePriv.HasValue ? n4UnboundAction.DataFmtCodePriv.Value : (short)0,
				Description = n4UnboundAction.Description,
				Removed = n4UnboundAction.Removed,
				Hidden = n4UnboundAction.Hidden,
				ActionGroup = n4UnboundAction.ActionGroup,
				ActionGUID = n4UnboundAction.ActionGuid
			};

			_dataService.Log($"GetUnBoundAction(actionId: {actionId}) returned action object");

			return unboundAction;
		}

		public void GetUnBoundActionData(int actionId, out byte[] publicData, out byte[] privateData,
			object callContext)
		{
			_dataService.Log($"Entering GetUnBoundActionData(actionId: {actionId})");

			var n4UnboundActionData = _dataService.GetUnboundActionData(actionId);
			publicData = n4UnboundActionData?.PublicData;
			privateData = n4UnboundActionData?.PrivateData;
			if (publicData != null && publicData.Length == 0) publicData = null;
			if (privateData != null && privateData.Length == 0) privateData = null;

			_dataService.Log($"GetActionData(actionId: {actionId}) returned action data object");
		}

		public int[] GetUnBoundActionIDs(DateTime startDate, DateTime endDate, object callContext)
		{
			var actionIds = new int[] { };
			try
			{
				_dataService.Log($"Entering GetUnBoundActionIDs(startDate: {startDate}, endDate: {endDate})");
				actionIds = _dataService.GetUnboundActionIds(startDate, endDate);
				_dataService.Log(
					$"GetUnBoundActionIDs(startDate: {startDate}, endDate: {endDate}) returned int[{actionIds.Length}]");
			}
			catch (NullReferenceException e)
			{
				_dataService.Log(
					$"NullReferenceException occurred in GetUnBoundActionIDs: {e.Message}\r\n\tStack Trace: {e.StackTrace}");
			}
			catch
			{
				// do nothing
			}

			return actionIds;
		}

		public int[] GetUnBoundActionReferences(int actionId, object callContext)
		{
			_dataService.Log($"Entering GetUnBoundActionReferences(actionId: {actionId})");

			var references = _dataService.GetUnboundActionReferences(actionId);

			_dataService.Log($"GetUnBoundActionReferences(actionId: {actionId}) returned int[{references.Length}]");

			return references;
		}

		public UnBoundAction[] GetUnBoundActions(int[] actionIds, object callContext)
		{
			_dataService.Log("Entering GetUnBoundActions(actionIds)");

			var n4UnboundActions = _dataService.GetUnboundActions(actionIds);

			var actions = n4UnboundActions == null
				? new UnBoundAction[0]
				: n4UnboundActions.Select(action => new UnBoundAction
				{
					ID = action.Id,
					CreateDate = action.CreatedDate,
					LastModifiedDate = action.UpdatedDate,
					UserID = action.UpdatedUserId ?? 0,
					ModuleID = action.ModuleId,
					DeviceType = action.DevTypeCode.HasValue ? action.DevTypeCode.Value : (short)0,
					DataTypeCode = action.DataTypeCode.HasValue ? action.DataTypeCode.Value : (short)0,
					DataFmtStd = action.DataFmtCodeStd.HasValue ? action.DataFmtCodeStd.Value : (short)0,
					DataFmtExt = action.DataFmtCodePriv.HasValue ? action.DataFmtCodePriv.Value : (short)0,
					Description = action.Description,
					Removed = action.Removed,
					Hidden = action.Hidden,
					ActionGroup = action.ActionGroup,
					ActionGUID = action.ActionGuid
				}).ToArray();

			_dataService.Log($"GetUnBoundActions(actionIds) returned UnBoundAction[{actions.Length}]");

			return actions;
		}

		public ActionEx[] GetUpdatedActions(int page, int pageSize, DateTime start, DateTime? end, int[] dataTypes,
			object callContext)
		{
			_dataService.Log(
				$"Entering GetUpdatedActions(page: {page}, pageSize: {pageSize}, start: {start}, end: {end}, dataTypes)");

			if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
			if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

			var n4Actions = _dataService.GetUpdatedActions(page, pageSize, start, end, dataTypes);

			var actions = n4Actions.Select(action => new ActionEx
			{
				ID = action.Id,
				SessionID = action.SessionId,
				CreateDate = action.CreatedDate,
				LastModifiedDate = action.UpdatedDate.HasValue
					? action.UpdatedDate == SqlDateTime.MinValue.Value ? DateTime.MinValue : action.UpdatedDate.Value
					: SqlDateTime.MinValue.Value,
				UserID = action.UpdatedUserId ?? 0,
				ModuleID = action.ModuleId,
				DeviceType = action.DevTypeCode.HasValue ? action.DevTypeCode.Value : (short)0,
				DataTypeCode = action.DataTypeCode.HasValue ? action.DataTypeCode.Value : (short)0,
				DataFmtStd = action.DataFmtCodeStd.HasValue ? action.DataFmtCodeStd.Value : (short)0,
				DataFmtExt = action.DataFmtCodePriv.HasValue ? action.DataFmtCodePriv.Value : (short)0,
				Description = action.Description,
				Removed = action.Removed ?? false,
				Hidden = action.Hidden ?? false,
				ActionGroup = action.ActionGroup.HasValue && action.ActionGroup.Value == SqlDateTime.MinValue.Value
					? DateTime.MinValue
					: action.ActionGroup ?? SqlDateTime.MinValue.Value,
				ActionGUID = action.ActionGuid,
				UserInitials = "???"
			}).ToArray();

			foreach (var action in actions)
			{
				var user = GetUserOrNull(action.UserID, callContext);
				if (user != null) action.UserInitials = action.UserID.ToString();
			}

			_dataService.Log(
				$"GetUpdatedActions(page: {page}, pageSize: {pageSize}, start: {start}, end: {end}, dataTypes) returned Action[{actions.Length}]");

			return actions;
		}

		public Patient[] GetUpdatedPatients(int page, int pageSize, DateTime start, DateTime? end, object callContext)
		{
			_dataService.Log(
				$"Entering GetUpdatedPatients(page: {page}, pageSize: {pageSize}, start: {start}, end: {end})");

			if (page < 1 || pageSize < 1) throw new ArgumentOutOfRangeException();

			var n4Patients = _dataService.GetUpdatedPatients(page, pageSize, start, end);

			Patient[] patients = { };
			if (n4Patients != null)
				patients = n4Patients?.Select(p => new Patient
				{
					ID = p.Id,
					PatientGUID = p.PatientGuid,
					ActivePatient = p.ActivePatient,
					PatientNo = p.PatientNo,
					CreateDate = p.DateCreated,
					FirstName = p.FirstName,
					MiddleName = p.MiddleName,
					LastName = p.LastName,
					Gender = p.Gender,
					BirthDate = p.Birthdate,
					Salutation = p.Salutation,
					Title = p.Title,
					Address1 = p.Address1,
					Address2 = p.Address2,
					Address3 = string.Empty,
					Province = p.State,
					City = p.City,
					Zip = p.Zip,
					Country = string.Empty,
					HomePhone = p.HomePhone,
					WorkPhone = p.WorkPhone,
					MobilePhone = p.MobilePhone,
					EMail = p.Email,
					SSNumber = p.Ssn,
					Insurance1 = p.Insurance1,
					Insurance2 = p.Insurance2,
					Other1 = p.Other1,
					Other2 = p.Other2,
					Physician = p.Physician,
					Referral = p.Referral,
					CreatedBy = p.CreatedBy
				}).ToArray();

			_dataService.Log(
				$"GetUpdatedPatients(page: {page}, pageSize: {pageSize}, start: {start}, end: {end}) returned Patient[{patients.Length}]");

			return patients;
		}

		public User GetUser(int userId, object callContext)
		{
			_dataService.Log($"Entering GetUser(userId: {userId})");
			var timer = new Stopwatch();
			timer.Start();
			var n4User = _dataService.GetUser(userId);

			if (n4User == null) throw new NoahDatabaseException(NOAHErrorType.NOAH_E_DB_USER_NOT_FOUND);

			var user = new User
			{
				ID = n4User.Id,
				CreateDate = n4User.CreatedDate,
				LoginName = n4User.Name,
				UserName = n4User.Name,
				UserInitials = n4User.Initials
			};

			timer.Stop();
			_dataService.Log(
				$"GetUser(userId: {userId}) returned User '{user.UserName}' {timer.Elapsed.TotalSeconds} secs");

			return user;
		}

		public User GetUser(string username, object callContext)
		{
			_dataService.Log($"Entering GetUser(username: {username})");

			var n4User = _dataService.GetUser(username);

			if (n4User == null) throw new NoahDatabaseException(NOAHErrorType.NOAH_E_DB_USER_NOT_FOUND);

			var user = new User
			{
				ID = n4User.Id,
				CreateDate = n4User.CreatedDate,
				LoginName = n4User.Name,
				UserName = n4User.Name,
				UserInitials = n4User.Initials
			};
			_dataService.Log($"GetUser(username: {username}) returned User with id {user.ID}");

			return user;
		}

		public User GetUserOrNull(int userId, object callContext)
		{
			_dataService.Log($"Entering GetUser(userId: {userId})");
			var n4User = _dataService.GetUser(userId);

			if (n4User == null) return null;

			var user = new User
			{
				ID = n4User.Id,
				CreateDate = n4User.CreatedDate,
				LoginName = n4User.Name,
				UserName = n4User.Name,
				UserInitials = n4User.Initials
			};
			_dataService.Log($"GetUser(userId: {userId}) returned User '{user.UserName}'");

			return user;
		}

		public UserPrivilege GetUserPrivilege(int userId, object callContext)
		{
			_dataService.Log($"Entering GetUserPrivilege(userId: {userId})");

			var appRegister = _dataService.GetUserPrivilege(userId);
			var userPrivilege = new UserPrivilege
			{
				AppRegister = appRegister
			};
			_dataService.Log($"GetUserPrivilege(userId: {userId}) returned '{appRegister}'");

			return userPrivilege;
		}

		public byte[] GetUserSetup(int userId, int moduleId, object callContext)
		{
			_dataService.Log($"Entering GetUserSetup(userId: {userId}, moduleId: {moduleId})");

			var user = GetUser(userId, callContext);

			byte[] userSetupData = null;

			var n4UserSetup = _dataService.GetUserSetup(userId, moduleId);

			if (n4UserSetup != null) userSetupData = n4UserSetup.SetupData;

			_dataService.Log(
				$"GetUserSetup(userId: {userId}, moduleId: {moduleId}) returned byte[{userSetupData?.Length ?? 0}]");

			return userSetupData;
		}

		public DatabaseNotification HandleNoahDataObjectWrapper(ref object noahDataObjectWrapper)
		{
			_dataService.Log("Entering HandleNoahDataObjectWrapper");
			var cmd = noahDataObjectWrapper as Noah4SystemCommand;
			DatabaseNotification result = null;

			if (cmd != null && cmd.Command == Noah4SystemCommandEnum.SendEventPatientUpdated)
			{
				_dataService.Log("HandleNoahDataObjectWrapper:Command = SendEventPatientUpdated. Executing request...");
				var patientId = (int)cmd.Data;
				result = new DatabaseNotification
				{
					SendTo = DatabaseNotification.SendOptions.all,
					NotificationData = new Noah4DbNotification
					{
						NotificationType = Noah4DbNotificationTypeEnum.UpdatePatientList,
						UserID = 0,
						PatientID = patientId,
						SessionID = 0,
						ActionID = 0
					}
				};
			}

			return result;
		}

		public void PutAction(ref Action action, byte[] publicData, byte[] privateData, FastView fastView,
			object callContext)
		{
			_dataService.Log($"Entering PutAction(action({action.ID}))");

			var n4Action = new N4Action
			{
				Id = action.ID,
				SessionId = action.SessionID,
				CreatedDate = action.CreateDate,
				UpdatedDate = action.LastModifiedDate,
				UpdatedUserId = action.UserID,
				ModuleId = action.ModuleID,
				DevTypeCode = action.DeviceType,
				DataTypeCode = action.DataTypeCode,
				DataFmtCodeStd = action.DataFmtStd,
				DataFmtCodePriv = action.DataFmtExt,
				Description = action.Description,
				Removed = action.Removed,
				Hidden = action.Hidden,
				ActionGroup = action.ActionGroup,
				ActionGuid = action.ActionGUID,
				PublicData = publicData,
				PrivateData = privateData,
				FastViewVersion = fastView?.Version ?? 0,
				FastViewDataType = (int)(fastView?.Format ?? 0),
				FastViewData = fastView?.Data
			};

			var idPair = _dataService.PutAction(n4Action);
			action.ID = idPair.Id;
			action.ActionGUID = idPair.Guid;
		}

		public void PutActionReferences(int actionId, int[] actionReferences, object callContext)
		{
			_dataService.Log($"Entering PutActionReferences(actionId: {actionId})");

			_dataService.PutActionReferences(actionId, actionReferences);
		}

		public void PutAppPermissions(AppPermission[] appPermissions, object callContext)
		{
			_dataService.Log("Entering PutAppPermissions(appPermissions)");
			var n4AppPermissions = appPermissions.Select(x => new N4AppPermission
			{
				Id = x.PermissionID,
				Default = x.Default,
				MaxLength = x.MaxLength,
				MinLength = x.MinLength,
				Name = x.Name,
				ReadOnly = x.ReadOnly,
				Required = x.Required,
				Type = (int)x.Type
			}).ToArray();
			var data = new ArrayWrapper<N4AppPermission> { Values = n4AppPermissions };
			_dataService.PutAppPermissions(data);
		}

		public void PutManufacturerSetup(ManufacturerSetup[] setupData, object callContext)
		{
			_dataService.Log("Entering PutManufacturerSetup(setupData)");

			var manufacturerSetups = setupData.Select(x => new N4ManufacturerSetup
			{
				ManufacturerId = x.ManufacturerID,
				Key = x.Key,
				SetupData = x.Data
			}).ToArray();
			var data = new ArrayWrapper<N4ManufacturerSetup> { Values = manufacturerSetups };
			_dataService.PutManufacturerSetups(data);
		}

		public void PutMobileAppPermissions(int moduleId, int[] permissionIds, object callContext)
		{
			_dataService.Log($"Entering PutMobileAppPermissions(moduleId: {moduleId})");

			_dataService.PutMobileAppPermissions(moduleId, permissionIds);
		}

		/// <summary>
		///     We only allow users to create patients in TIMS
		/// </summary>
		public Patient PutPatient(Patient patient, object callContext)
		{
			_dataService.Log("Entering PutPatient(patient)");
			throw new NoahDatabaseException(NOAHErrorType.NOAH_E_NOTSUPPORTED);
		}

		public void PutPatientIdentification(int patientId, int manufacturerId, string identification,
			object callContext)
		{
			_dataService.Log(
				$"Entering PutPatientIdentification(patientId: {patientId}, manufactureID: {manufacturerId}, identification: {identification})");

			var patientIdentification = new N4PatientIdentification
			{
				PatientId = patientId,
				ManufacturerId = manufacturerId,
				IdentificationData = identification
			};

			_dataService.PutPatientIdentification(patientIdentification);
		}

		public void PutPatientSetup(int patientId, int moduleId, byte[] setupData, object callContext)
		{
			_dataService.Log($"Entering PutPatientSetup(patientId: {patientId}, moduleId: {moduleId})");
			var patient = GetPatient(patientId, callContext);
			var n4PatientSetup = new N4PatientSetup
			{
				PatientId = patientId,
				ModuleId = moduleId,
				SetupData = setupData
			};

			_dataService.PutPatientSetup(n4PatientSetup);
		}

		public void PutSession(ref Session session, object callContext)
		{
			_dataService.Log($"Entering PutSession(session({session.ID}))");
			N4Session n4Session = null;
			if (session.ID != 0)
			{
				n4Session = _dataService.GetSession(session.ID);
			}
			else
			{
				n4Session = new N4Session
				{
					Id = session.ID,
					CreateDate = session.CreateDate,
					PatientId = session.PatientID
				};
				var sessionId = _dataService.PutSession(n4Session);
				session.ID = sessionId;
			}
		}

		public void PutStaticCallContext(string StaticCallContext, object callContext)
		{
			throw new NotImplementedException();
		}

		public void PutUnBoundAction(ref UnBoundAction action, byte[] publicData, byte[] privateData,
			object callContext)
		{
			_dataService.Log("Entering PutUnBoundAction(action, publicData, privateData)");

			var n4UnboundAction = new N4UnboundAction
			{
				Id = action.ID,
				CreatedDate = action.CreateDate,
				UpdatedDate = action.LastModifiedDate,
				UpdatedUserId = action.UserID,
				ModuleId = action.ModuleID,
				DevTypeCode = action.DeviceType,
				DataTypeCode = action.DataTypeCode,
				DataFmtCodeStd = action.DataFmtStd,
				DataFmtCodePriv = action.DataFmtExt,
				Description = action.Description,
				Removed = action.Removed,
				Hidden = action.Hidden,
				ActionGroup = action.ActionGroup,
				ActionGuid = action.ActionGUID,
				PublicData = publicData,
				PrivateData = privateData
			};

			var idPair = _dataService.PutUnboundAction(n4UnboundAction);
			action.ID = idPair.Id;
			action.ActionGUID = idPair.Guid;
		}

		public void PutUnBoundActionReferences(int actionId, int[] actionReferences, object callContext)
		{
			_dataService.Log($"Entering PutUnBoundActionReferences(actionId: {actionId})");

			_dataService.PutUnboundActionReferences(actionId, actionReferences);
		}

		public void PutUserSetup(int userId, int moduleId, byte[] setupData, object callContext)
		{
			_dataService.Log($"Entering PutUserSetup(userId: {userId}, moduleId: {moduleId})");

			var user = GetUser(userId, callContext);

			var n4UserSetup = new N4UserSetup
			{
				UserId = userId,
				ModuleId = moduleId,
				SetupData = setupData
			};

			_dataService.PutUserSetup(n4UserSetup);
		}

		public void RegisterMobileApp(MobileApp mobileApp, object callContext)
		{
			_dataService.Log("Entering RegisterMobileApp(mobileApp)");
			var n4MobileApp = new N4MobileApp
			{
				ModuleId = mobileApp.ModuleID,
				AcceptState = (int)mobileApp.AcceptState,
				MobileAppType = (int)mobileApp.MobileAppType,
				Name = mobileApp.Name,
				Version = mobileApp.Version,
				PendingRequirements = _SerializeAppDataRequirements(mobileApp.PendingRequirements),
				ActiveRequirements = _SerializeAppDataRequirements(mobileApp.ActiveRequirements)
			};
			_dataService.RegisterMobileApp(n4MobileApp);
		}

		public DashboardAlert RetrieveArchivedDashboardAlert(Guid alertGuid, object callContext)
		{
			_dataService.Log($"Entering RetrieveDashboardAlert(alertGuid: {alertGuid})");
			var alert = _dataService.GetDashboardAlertArchive(alertGuid);
			return new DashboardAlert
			{
				ActionID = alert.ActionId,
				AlertGUID = alert.AlertGuid,
				AppModuleID = alert.AppModuleId,
				AssigneeUserID = alert.AssigneeUserId,
				Category = alert.Category,
				Description = alert.Description,
				Group = alert.Group,
				IconUrl = alert.IconUrl,
				IsRead = alert.IsRead,
				LastModifiedUtc = alert.LastModifiedUtc,
				ModuleID = alert.ModuleId,
				ModuleParameter = alert.ModuleParameter,
				NotificationActionID = alert.NotificationActionId,
				PatientGUID = alert.PatientGuid,
				Priority = (Priority)alert.Priority,
				ReceivedUtc = alert.ReceivedUtc,
				Url = alert.Url
			};
		}

		public DashboardAlert RetrieveDashboardAlert(Guid alertGuid, object callContext)
		{
			_dataService.Log($"Entering RetrieveDashboardAlert(alertGuid: {alertGuid})");
			var alert = _dataService.GetDashboardAlert(alertGuid);
			if (alert == null)
				return null;
			return new DashboardAlert
			{
				ActionID = alert.ActionId,
				AlertGUID = alert.AlertGuid,
				AppModuleID = alert.AppModuleId,
				AssigneeUserID = alert.AssigneeUserId,
				Category = alert.Category,
				Description = alert.Description,
				Group = alert.Group,
				IconUrl = alert.IconUrl,
				IsRead = alert.IsRead,
				LastModifiedUtc = alert.LastModifiedUtc,
				ModuleID = alert.ModuleId,
				ModuleParameter = alert.ModuleParameter,
				NotificationActionID = alert.NotificationActionId,
				PatientGUID = alert.PatientGuid,
				Priority = (Priority)alert.Priority,
				ReceivedUtc = alert.ReceivedUtc,
				Url = alert.Url
			};
		}

		public void SetNotificationCallback(DatabaselayerNotification callback)
		{
			_dataService.Log("Entering SetNotificationCallback(callback)");
			_databaseNotificationCallback = callback;
		}

		public void SetPreference(int id, byte[] preference, bool allowUpdate, object callContext)
		{
			_dataService.Log($"Entering SetPreference(id: {id}, allowUpdate: {allowUpdate})");

			if (preference == null)
			{
				_dataService.DeletePreference(id);
			}
			else
			{
				var existingPreference = _dataService.GetPreference(id);
				if (existingPreference != null && !allowUpdate)
					throw new NoahDatabaseException(NOAHErrorType.NOAH_E_DB_PREFERENCE_ALLREADY_EXISTS);
				_dataService.PutPreference(id, preference);
			}
		}

		public void SetServiceAppEventCallback(ServiceAppEventCallback callback)
		{
			// Do nothing
		}

		public void UnregisterMobileApp(int moduleId, object callContext)
		{
			_dataService.Log($"Entering UnregisterMobileApp(moduleId: {moduleId})");

			_dataService.UnregisterMobileApp(moduleId);
		}

		public void UpdateArchivedDashboardAlert(DashboardAlert alert, object callContext)
		{
			_dataService.Log("Entering CreateDashboardAlert(alert, callContext)");

			var n4Alert = new N4DashboardAlertArchive
			{
				ActionId = alert.ActionID,
				AlertGuid = alert.AlertGUID,
				AppModuleId = alert.AppModuleID,
				AssigneeUserId = alert.AssigneeUserID,
				Category = alert.Category,
				Description = alert.Description,
				Group = alert.Group,
				IconUrl = alert.IconUrl,
				IsRead = alert.IsRead,
				LastModifiedUtc = alert.LastModifiedUtc,
				ModuleId = alert.ModuleID,
				ModuleParameter = alert.ModuleParameter,
				NotificationActionId = alert.NotificationActionID,
				PatientGuid = alert.PatientGUID,
				Priority = (int)alert.Priority,
				ReceivedUtc = alert.ReceivedUtc,
				Url = alert.Url
			};

			_dataService.UpdateDashboardAlertArchive(n4Alert);
		}

		public void UpdateDashboardAlert(DashboardAlert alert, object callContext)
		{
			_dataService.Log("Entering CreateDashboardAlert(alert, callContext)");

			var n4Alert = new N4DashboardAlert
			{
				ActionId = alert.ActionID,
				AlertGuid = alert.AlertGUID,
				AppModuleId = alert.AppModuleID,
				AssigneeUserId = alert.AssigneeUserID,
				Category = alert.Category,
				Description = alert.Description,
				Group = alert.Group,
				IconUrl = alert.IconUrl,
				IsRead = alert.IsRead,
				LastModifiedUtc = alert.LastModifiedUtc,
				ModuleId = alert.ModuleID,
				ModuleParameter = alert.ModuleParameter,
				NotificationActionId = alert.NotificationActionID,
				PatientGuid = alert.PatientGUID,
				Priority = (int)alert.Priority,
				ReceivedUtc = alert.ReceivedUtc,
				Url = alert.Url
			};

			_dataService.UpdateDashboardAlert(n4Alert);
		}

		public bool ValidateLogin(string loginname, object password, out int userid, object callContext)
		{
			callContext = null;

			if (loginname == "timslogin_12")
			{
				userid = (int)password;
				return true;
			}

			bool isValid;
			try
			{
				isValid = ValidateLogin(loginname, (string)password, out userid, callContext);
			}
			catch (NoahDatabaseException ex)
			{
				if (ex.Error == NOAHErrorType.NOAH_E_DB_USER_NOT_FOUND)
				{
					userid = -1;
					isValid = false;
				}
				else
				{
					throw;
				}
			}

			return isValid;
		}

		public bool ValidateLogin(string username, string password, NoahMobileInfo noahMobileInfo, out int userId,
			out object callContext)
		{
			_dataService.Log($"ValidateLogin(username: {username}, password: {password})");
			callContext = null;
			userId = _dataService.ValidateLogin(username, password);
			_dataService.Log($"    returned userId of {userId}");
			return userId != -1;
		}

		#endregion NoahDataInterface Members

		#region Fields

		private DatabaselayerNotification _databaseNotificationCallback;
		private readonly NoahDataService _dataService;

		private readonly string _noahVersion = string.Empty;
		//private readonly string _officeCode;
		//private static readonly string ENV_VAR_API_URL = "TIMS_API_URL";
		//private static readonly string ENV_VAR_LOG_FILE = "TIMS_LOG";
		//private static readonly string ENV_VAR_OFFICE_CODE = "TIMS_OFFICE_CODE";

		#endregion Fields
	}
}