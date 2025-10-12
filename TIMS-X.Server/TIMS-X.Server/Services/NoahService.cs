using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Noah;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Models.Noah;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Services;

public class NoahService
{
	private readonly ContextHelper _contextHelper;
	private readonly INoahDataMiningService _ndmService;
	private readonly NoahQuery _noahQuery;
	private readonly PatientQuery _patientQuery;

	#region Constructors

	public NoahService(INoahDataMiningService ndmService, NoahQuery noahQuery, PatientQuery patientQuery,
		ContextHelper contextHelper)
	{
		_ndmService = ndmService;
		_noahQuery = noahQuery;
		_patientQuery = patientQuery;
		_contextHelper = contextHelper;
	}

	#endregion Constructors

	#region NoahService Members

	/// <summary>
	///     Formats a date time to WW3 standaard
	/// </summary>
	/// <param name="dateTime"></param>
	/// <returns></returns>
	private string _FormatDateTimeWW3(DateTime dateTime)
	{
		//return dateTime.ToString("O");
		return string.Format("{0:yyyy-MM-ddTHH:mm:ss}", dateTime);
	}

	private string _FormatDateWW3(DateTime dateTime)
	{
		return string.Format("{0:yyyy-MM-dd}", dateTime);
	}

	/// <summary>
	///     returns a substring of the input string that is no longer than the length provided
	/// </summary>
	/// <param name="input"></param>
	/// <param name="length"></param>
	/// <returns></returns>
	private static string _LimitLength(string input, int length)
	{
		if (string.IsNullOrEmpty(input)) return input;

		if (input.Length <= length) return input;
		return input.Substring(0, length);
	}

	/// <summary>
	///     Creates the patient XML node
	/// </summary>
	/// <param name="patient"></param>
	/// <returns></returns>
	private XElement _CreatePatientNodeAsync(Patient patient)
	{
		var xPatient = new XElement(Constants.XML_PATIENT);
		xPatient.Add(new XAttribute(Constants.XML_PATIENTNO, patient.Id));
		xPatient.Add(new XAttribute(Constants.XML_ACTIVEPATIENT, patient.Inactive ? 0 : 1));
		xPatient.Add(new XAttribute(Constants.XML_PATIENTGUID, patient.Guid));
		xPatient.Add(new XAttribute(Constants.XML_CREATEDATE, _FormatDateTimeWW3(patient.UpdatedDate)));
		xPatient.Add(new XAttribute(Constants.XML_FIRSTNAME, patient.FirstName));
		xPatient.Add(new XAttribute(Constants.XML_MIDDLENAME, patient.Initial ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_USERINITIALS, _contextHelper.CurrentUser.Initials));
		xPatient.Add(new XAttribute(Constants.XML_LASTNAME, patient.LastName));
		xPatient.Add(new XAttribute(Constants.XML_BIRTHDATE,
			patient.BirthDate.HasValue
				? _FormatDateWW3(patient.BirthDate.Value)
				: _FormatDateWW3(SqlDateTime.MinValue.Value)));
		xPatient.Add(new XAttribute(Constants.XML_ADDRESS1, patient.Address1 ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_ADDRESS2, patient.Address2 ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_CITY, patient.City ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_STATE, patient.State ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_ZIP, patient.ZipCode ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_HOMEPHONE, patient.HomePhone ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_WORKPHONE, patient.WorkPhone ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_MOBILEPHONE, patient.MobilePhone ?? string.Empty));
		xPatient.Add(new XAttribute(Constants.XML_EMAIL, _LimitLength(patient.Email, 30) ?? string.Empty));
		var sex = 0;
		if (patient.Sex != null)
		{
			if (patient.Sex == "M")
				sex = 1;
			else if (patient.Sex == "F")
				sex = 2;
		}

		xPatient.Add(new XAttribute(Constants.XML_GENDER, sex));
		return xPatient;
	}

	/// <summary>
	///     Creates the patient directory document
	/// </summary>
	/// <param name="patient"></param>
	/// <param name="offset"></param>
	/// <returns></returns>
	private XDocument _CreatePatientDirectoryDocumentAsync(int offset, Patient patient)
	{
		var xPatient = _CreatePatientNodeAsync(patient);
		xPatient.Add(new XAttribute(Constants.XML_PATIENTRECORDOFFSET, offset));
		var patientContainer = new XElement(Constants.XML_PATIENTS);
		patientContainer.Add(xPatient);
		return new XDocument(patientContainer);
	}

	/// <summary>
	///     Creates the Users XML document
	/// </summary>
	/// <returns></returns>
	private XDocument _CreateUsersDocument()
	{
		var xUser = new XElement(Constants.XML_USER);

		xUser.Add(new XElement(Constants.XML_INITIALS, _contextHelper.CurrentUser.Initials));
		xUser.Add(new XElement(Constants.XML_NAME, _contextHelper.CurrentUser.Name));
		xUser.Add(new XElement(Constants.XML_USERNAME, _contextHelper.CurrentUser.Name));
		var xUsers = new XElement(Constants.XML_USERS);
		xUsers.Add(xUser);
		return new XDocument(xUsers);
	}

	/// <summary>
	///     Creates the patient document
	/// </summary>
	/// <param name="patient"></param>
	/// <returns></returns>
	private async Task<XDocument> _CreatePatientDocumentAsync(Patient patient)
	{
		var xPatient = _CreatePatientNodeAsync(patient);
		var xSessions = new XElement(Constants.XML_SESSIONS);

		var sessions = await _noahQuery.GetSessionsAsync(patient.Id);
		DateTime tempDate;
		foreach (var session in sessions)
		{
			var xSession = new XElement(Constants.XML_SESSION);
			tempDate = session.CreateDate.HasValue ? session.CreateDate.Value : DateTime.MinValue;
			xSession.Add(new XAttribute(Constants.XML_CREATEDATE, string.Format("{0:yyyy-MM-dd}", tempDate)));
			var actions = await _noahQuery.GetActionsAsync(session.Id);
			foreach (var action in actions)
			{
				var xAction = new XElement(Constants.XML_ACTION);
				xAction.Add(new XAttribute(Constants.XML_UNIQUEID, action.Id));
				xAction.Add(new XAttribute(Constants.XML_ACTIONGUID, action.ActionGuid));
				xAction.Add(new XAttribute(Constants.XML_USERINITIALS, _contextHelper.CurrentUser.Initials));
				xAction.Add(new XAttribute(Constants.XML_CREATEDATE, _FormatDateTimeWW3(action.CreatedDate)));
				if (action.ModuleId > 0) xAction.Add(new XAttribute(Constants.XML_MODULEID, action.ModuleId));

				// Trim for control characters from OT Suite
				if (string.IsNullOrEmpty(action.Description))
					xAction.Add(new XAttribute(Constants.XML_DESCRIPTION,
						Regex.Replace(action.Description, "[^A-Za-z0-9 _]", " ").Trim()));
				else
					xAction.Add(new XAttribute(Constants.XML_DESCRIPTION, string.Empty));
				if (action.UpdatedDate.HasValue)
					xAction.Add(new XAttribute(Constants.XML_LASTMODIFIEDDATE,
						_FormatDateTimeWW3(action.UpdatedDate.Value)));
				xAction.Add(new XAttribute(Constants.XML_DATATYPECODE, action.DataTypeCode));
				xAction.Add(new XAttribute(Constants.XML_DATAFMTCODESTD, action.DataFmtCodeStd));
				xAction.Add(new XAttribute(Constants.XML_DATAFMTCODEPRIV, action.DataFmtCodePriv));
				if (action.DevTypeCode > 0) xAction.Add(new XAttribute(Constants.XML_DEVTYPECODE, action.DevTypeCode));
				if (action.ActionGroup.HasValue)
					xAction.Add(new XAttribute(Constants.XML_ACTIONGROUP,
						_FormatDateTimeWW3(action.ActionGroup.Value)));
				xAction.Add(new XAttribute(Constants.XML_HIDDEN,
					action.Hidden ?? false ? Constants.XML_TRUE : Constants.XML_FALSE));
				xAction.Add(new XAttribute(Constants.XML_REMOVED,
					action.Removed ?? false ? Constants.XML_TRUE : Constants.XML_FALSE));
				xAction.Add(new XAttribute(Constants.XML_ISARCHIVED,
					action.IsArchived ? Constants.XML_TRUE : Constants.XML_FALSE));

				var actionData = await GetActionDataAsync(action.Id);

				if (actionData.PublicData != null)
				{
					var xPublic = new XElement(Constants.XML_PUBLICDATA);
					var publicData = actionData.PublicData.ToArray();

					xPublic.Add(Convert.ToBase64String(publicData));
					xAction.Add(xPublic);
				}

				if (actionData.PrivateData != null)
				{
					var xPrivate = new XElement(Constants.XML_PRIVATEDATA);
					var privateData = actionData.PrivateData.ToArray();
					xPrivate.Add(Convert.ToBase64String(privateData));
					xAction.Add(xPrivate);
				}

				var actionRefs = await _noahQuery.GetActionReferencesAsync(action.Id);
				foreach (var actionReference in actionRefs)
				{
					var xUniqueID = new XElement(Constants.XML_UNIQUEIDREF, actionReference);
					xAction.Add(xUniqueID);
				}

				// Add the action
				xSession.Add(xAction);
			}

			// Add the session
			xSessions.Add(xSession);
		}

		xPatient.Add(xSessions);
		return new XDocument(xPatient);
	}


	/// <summary>
	///     Converts an xml documen to a byte array
	/// </summary>
	/// <param name="xDocument"></param>
	/// <returns></returns>
	private byte[] _ConvertXMLToByteArray(XDocument xDocument)
	{
		var ms = new MemoryStream();
		try
		{
			xDocument.Save(ms);
		}
		catch (Exception)
		{
			return new byte[0];
		}

		return ms.ToArray();
	}

	public async Task<byte[]> GetNHAXAsync(int patientId, string baseUrl)
	{
		var rootxSection = new RootxSection();
		rootxSection.MagicNumber = Constants.MagicNumber;
		rootxSection.MiscOffset = 0;
		var patient = await _patientQuery.GetFullPatientAsync(patientId);
		var xPatientDirectory = new XDocument();
		var xPatientDocument = new XDocument();
		var xUsers = new XDocument();
		try
		{
			xPatientDirectory = _CreatePatientDirectoryDocumentAsync(40, patient);
			xPatientDocument = await _CreatePatientDocumentAsync(patient);
			xUsers = _CreateUsersDocument();
		}
		catch (Exception)
		{
		}

		var rootData = NoahBlobWriter.RawSerialize(rootxSection);
		var patientData = _ConvertXMLToByteArray(xPatientDocument);
		var usersData = _ConvertXMLToByteArray(xUsers);
		var patientDirectoryData = _ConvertXMLToByteArray(xPatientDirectory);

		byte[] noahData = null;

		using (var ms = new MemoryStream())
		{
			var blobWriter = new NoahBlobWriter(ms);
			var position = blobWriter.WriteBlobAsync(rootData, 1);
			rootxSection.UserOffset = blobWriter.WriteBlobAsync(patientData, 1);
			rootxSection.PatientDirectoryOffset = blobWriter.WriteBlobAsync(usersData, 1);
			blobWriter.WriteBlobAsync(patientDirectoryData, 1);

			// Reserialize the root section with the updated offsets
			blobWriter.Position = 0;
			rootData = NoahBlobWriter.RawSerialize(rootxSection);
			blobWriter.WriteBlobAsync(rootData, 1);
			noahData = ms.ToArray();
		}

		return noahData;
	}

	public async Task ArchiveActionAsync(int actionId)
	{
		try
		{
			var action = await GetActionAsync(actionId);
			if (action != null)
			{
				var clone = action.Clone();
				await _noahQuery.PutActionAsync(clone, true);
				var actionArchive = new N4ActionArchive
				{
					OriginalActionId = actionId,
					ModifiedActionId = clone.Id
				};
				await _noahQuery.PutActionArchiveAsync(actionArchive);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task ArchiveUnboundActionAsync(int actionId)
	{
		var clone = (await GetUnboundActionAsync(actionId)).Clone();
		clone.IsArchived = true;

		await _noahQuery.PutUnboundActionAsync(clone);
		var unboundActionArchive = new N4UnboundActionArchive
		{
			OriginalActionId = actionId,
			ModifiedActionId = clone.Id
		};
		await _noahQuery.PutUnboundActionArchiveAsync(unboundActionArchive);
	}

	public async Task DeleteActionAsync(int actionId)
	{
		await _noahQuery.DeleteActionAsync(actionId);
	}

	public async Task DeleteDashboardAlertAsync(Guid alertGuid)
	{
		await _noahQuery.DeleteDashboardAlertAsync(alertGuid);
	}

	public async Task DeleteActionReferencesAsync(int actionId)
	{
		await _noahQuery.DeleteActionReferencesAsync(actionId);
	}

	public async Task<int[]> GetActionIdsFromDashboardGroupAsync(Guid group)
	{
		var result = await _noahQuery.GetActionIdsFromDashboardGroupAsync(group);
		return result;
	}

	public async Task DeleteAppPermissionsAsync(int[] permissionIds)
	{
		foreach (var permissionId in permissionIds)
		{
			var permission = await _noahQuery.GetAppPermissionAsync(permissionId);
			if (permission != null)
			{
				var mobileAppPermissions = await _noahQuery.GetMobileAppPermissionsAsync(permissionId, null);
				await _noahQuery.DeleteMobileAppPermissionsAsync(mobileAppPermissions);
				await _noahQuery.DeleteAppPermissionAsync(permission);
			}
		}
	}

	public async Task DeleteDashboardAlertArchiveAsync(Guid alertGuid)
	{
		await _noahQuery.DeleteDashboardAlertArchiveAsync(alertGuid);
	}

	public async Task DeleteArchivedActionsAsync(int actionId)
	{
		await _noahQuery.DeleteArchivedActionsAsync(actionId);
	}

	public async Task DeleteArchivedUnboundActionsAsync(int actionId)
	{
		await _noahQuery.DeleteArchivedUnboundActionsAsync(actionId);
	}

	public async Task DeleteFastViewAsync(int actionId)
	{
		await _noahQuery.DeleteFastViewAsync(actionId);
	}

	public async Task DeleteMobileAppPermissionsAsync(int moduleId, int[] permissionIds)
	{
		foreach (var permissionId in permissionIds)
		{
			var mobileAppPermissions = await _noahQuery.GetMobileAppPermissionsAsync(permissionId, moduleId);
			await _noahQuery.DeleteMobileAppPermissionsAsync(mobileAppPermissions);
		}
	}

	public async Task DeletePatientIdentificationAsync(int patientId, int manufacturerId)
	{
		await _noahQuery.DeletePatientIdentificationAsync(patientId, manufacturerId);
	}

	public async Task DeletePatientSetupAsync(int patientId, int moduleId)
	{
		await _noahQuery.DeletePatientSetupAsync(patientId, moduleId);
	}

	public async Task DeleteSessionAsync(int sessionId)
	{
		await _noahQuery.DeleteSessionAsync(sessionId);
	}

	public async Task DeleteUnboundActionAsync(int unboundActionId)
	{
		await _noahQuery.DeleteUnboundActionAsync(unboundActionId);
	}

	public async Task DeleteUnboundActionReferencesAsync(int actionId)
	{
		await _noahQuery.DeleteUnboundActionReferencesAsync(actionId);
	}

	public async Task DeleteUserSetupAsync(int userId, int moduleId)
	{
		await _noahQuery.DeleteUserSetupAsync(userId, moduleId);
	}

	public async Task<N4Action> GetActionAsync(int actionId)
	{
		return await _noahQuery.GetActionAsync(actionId);
	}

	public async Task<N4Action> GetActionAsync(Guid actionGuid)
	{
		return await _noahQuery.GetActionAsync(actionGuid);
	}

	public async Task<IEnumerable<N4Action>> GetActionCollectionAsync(int moduleId)
	{
		return await _noahQuery.GetActionCollectionAsync(moduleId);
	}

	public async Task<int> GetActionCollectionCountAsync(int moduleId)
	{
		return await _noahQuery.GetActionCollectionCountAsync(moduleId);
	}

	public async Task<N4ActionData> GetActionDataAsync(int actionId)
	{
		return await _noahQuery.GetActionDataAsync(actionId);
	}

	public async Task<int[]> GetActionReferencedByAsync(int actionId)
	{
		return await _noahQuery.GetActionReferencedByAsync(actionId);
	}

	public async Task<int[]> GetActionReferencesAsync(int actionId)
	{
		return await _noahQuery.GetActionReferencesAsync(actionId);
	}

	public async Task<IEnumerable<N4Action>> GetActionsAsync(int sessionId)
	{
		return await _noahQuery.GetActionsAsync(sessionId);
	}

	public async Task<IEnumerable<N4Action>> SearchActionsAsync(int patientId, DateTime? actionGroup, bool returnLatest,
		int[] dataTypes, int[] moduleIds)
	{
		return await _noahQuery.SearchActionsAsync(patientId, actionGroup, returnLatest, dataTypes, moduleIds);
	}

	public async Task<IEnumerable<N4UnboundAction>> GetUnboundActionsAsync(int[] actionIds)
	{
		return await _noahQuery.GetUnboundActionsAsync(actionIds);
	}

	public async Task<IEnumerable<N4AppPermission>> GetAppPermissionsAsync()
	{
		return await _noahQuery.GetAppPermissionsAsync();
	}

	public async Task<IEnumerable<N4Action>> GetArchivedActionsAsync(int actionId)
	{
		return await _noahQuery.GetArchivedActionsAsync(actionId);
	}

	public async Task<int> GetArchivedActionsCountAsync(int actionId)
	{
		return await _noahQuery.GetArchivedActionsCountAsync(actionId);
	}

	public async Task<IEnumerable<N4UnboundAction>> GetArchivedUnboundActionsAsync(int actionId)
	{
		return await _noahQuery.GetArchivedUnboundActionsAsync(actionId);
	}

	public async Task<int> GetArchivedUnboundActionsCountAsync(int actionId)
	{
		return await _noahQuery.GetArchivedUnboundActionsCountAsync(actionId);
	}

	public ConnectionInfo GetConnectionInfo()
	{
		return _noahQuery.GetConnectionInfo();
	}

	public async Task<N4FastView> GetFastViewAsync(int actionId)
	{
		return await _noahQuery.GetFastViewAsync(actionId);
	}

	public async Task<IEnumerable<string>> GetManufacturerSetupKeysAsync(int manufacturerId)
	{
		return await _noahQuery.GetManufacturerSetupKeysAsync(manufacturerId);
	}

	public async Task<IEnumerable<N4ManufacturerSetup>> GetManufacturerSetupsAsync(int manufacturerId, string[] keys)
	{
		return await _noahQuery.GetManufacturerSetupsAsync(manufacturerId, keys);
	}

	public async Task<int[]> GetMobileAppPermissionsAsync(int moduleId)
	{
		var permissions = await _noahQuery.GetMobileAppPermissionsAsync(null, moduleId);
		return permissions.Select(x => x.PermissionId).ToArray();
	}

	public async Task<IEnumerable<N4MobileApp>> GetMobileAppsAsync()
	{
		var mobileApps = await _noahQuery.GetMobileAppsAsync();
		return mobileApps;
	}

	public async Task<N4Patient> GetPatientAsync(int patientId)
	{
		return await _noahQuery.GetPatientAsync(patientId);
	}

	public async Task<int[]> GetPatientIdsFromIdentificationAsync(string identification, int manufacturerId)
	{
		return await _noahQuery.GetPatientIdsFromIdentificationAsync(identification, manufacturerId);
	}

	public async Task<IEnumerable<N4Patient>> GetPatientsAsync(string searchText, int page, int pageSize)
	{
		return await _noahQuery.GetPatientsAsync(searchText, page, pageSize);
	}

	public async Task<int> GetPatientsCountAsync(string searchText)
	{
		return await _noahQuery.GetPatientsCountAsync(searchText);
	}

	public async Task<N4PatientSetup> GetPatientSetupAsync(int patientId, int moduleId)
	{
		return await _noahQuery.GetPatientSetupAsync(patientId, moduleId);
	}

	public async Task<IEnumerable<N4PatientSetup>> GetPatientSetupsAsync(int patientId, int moduleId)
	{
		return await _noahQuery.GetPatientSetupsAsync(patientId, moduleId);
	}

	public async Task<IEnumerable<N4PatientSetup>> GetPatientSetupsForModuleAsync(int moduleId)
	{
		return await _noahQuery.GetPatientSetupsForModuleAsync(moduleId);
	}

	public async Task<N4Preference> GetPreferenceAsync(int preferenceId)
	{
		return await _noahQuery.GetPreferenceAsync(preferenceId);
	}

	public async Task<N4Session> GetSessionAsync(int sessionId)
	{
		return await _noahQuery.GetSessionAsync(sessionId);
	}

	public async Task<IEnumerable<N4Session>> GetSessionsAsync(int patientId)
	{
		return await _noahQuery.GetSessionsAsync(patientId);
	}

	public async Task<N4UnboundAction> GetUnboundActionAsync(int actionId)
	{
		return await _noahQuery.GetUnboundActionAsync(actionId);
	}

	public async Task<N4ActionData> GetUnboundActionDataAsync(int actionId)
	{
		return await _noahQuery.GetUnboundActionDataAsync(actionId);
	}

	public async Task<int[]> GetUnboundActionIdsAsync(DateTime startDate, DateTime endDate)
	{
		// Ensure Date Range values aren't going to cause DB exception
		if (startDate <= SqlDateTime.MinValue.Value) startDate = SqlDateTime.MinValue.Value;

		if (endDate <= SqlDateTime.MaxValue.Value) endDate = SqlDateTime.MaxValue.Value;

		return await _noahQuery.GetUnboundActionIdsAsync(startDate, endDate);
	}

	public async Task<int[]> GetUnboundActionReferencesAsync(int actionId)
	{
		return await _noahQuery.GetUnboundActionReferencesAsync(actionId);
	}

	public async Task<IEnumerable<N4Action>> GetUpdatedActionsAsync(int page, int pageSize, DateTime startTime,
		DateTime? endTime, short[] dataTypes)
	{
		return await _noahQuery.GetUpdatedActionsAsync(page, pageSize, startTime, endTime, dataTypes);
	}

	public async Task<IEnumerable<N4Patient>> GetUpdatedPatientsAsync(int page, int pageSize, DateTime startTime,
		DateTime? endTime)
	{
		return await _noahQuery.GetUpdatedPatientsAsync(page, pageSize, startTime, endTime);
	}

	public async Task<N4User> GetUserAsync(int userId)
	{
		return await _noahQuery.GetUserAsync(userId);
	}

	public async Task DeletePreferenceAsync(int preferenceId)
	{
		await _noahQuery.DeletePreferenceAsync(preferenceId);
	}

	public async Task<N4User> GetUserAsync(string username)
	{
		return await _noahQuery.GetUserAsync(username);
	}

	public async Task<IEnumerable<N4User>> GetUsersAsync()
	{
		return await _noahQuery.GetUsersAsync();
	}

	public async Task<int> GetUserPrivilegeAsync(int userId)
	{
		return await _noahQuery.GetUserPrivilegeAsync(userId);
	}

	public async Task<N4UserSetup> GetUserSetupAsync(int userId, int moduleId)
	{
		return await _noahQuery.GetUserSetupAsync(userId, moduleId);
	}

	public async Task PutActionAsync(N4Action action)
	{
		// NoahQuery.PutActionAsync will compress the public data, since we have further processing,
		// we copy the decompressed public data
		byte[] publicData = null;
		if (action.PublicData != null && action.PublicData.Length > 0)
		{
			publicData = action.PublicData.ToArray();
			;
		}

		await _noahQuery.PutActionAsync(action, false);
		if (action.SessionId > 0)
		{
			var session = await _noahQuery.GetSessionAsync(action.SessionId);
			if (session != null && publicData != null)
				await _ndmService.ProcessNoahActionAsync(action, publicData, session.PatientId);
		}
	}

	public async Task PutActionReferencesAsync(int actionId, int[] actionReferences)
	{
		await _noahQuery.PutActionReferencesAsync(actionId, actionReferences);
	}

	public async Task PutAppPermissionsAsync(N4AppPermission[] appPermissions)
	{
		await _noahQuery.PutAppPermissionsAsync(appPermissions);
	}

	public async Task PutManufacturerSetupsAsync(N4ManufacturerSetup[] manufacturerSetups)
	{
		await _noahQuery.PutManufacturerSetupAsync(manufacturerSetups);
	}

	public async Task PutMobileAppPermissionsAsync(int moduleId, int[] permissionIds)
	{
		await _noahQuery.PutMobileAppPermissionsAsync(moduleId, permissionIds);
	}

	public async Task PutPatientIdentificationAsync(N4PatientIdentification patientIdentification)
	{
		await _noahQuery.PutPatientIdentificationAsync(patientIdentification);
	}

	public async Task PutPatientSetupAsync(N4PatientSetup patientSetup)
	{
		await _noahQuery.PutPatientSetupAsync(patientSetup);
	}

	public async Task PutPreferenceAsync(int preferenceId, byte[] preference)
	{
		await _noahQuery.PutPreferenceAsync(preferenceId, preference);
	}

	public async Task PutSessionAsync(N4Session session)
	{
		await _noahQuery.PutSessionAsync(session);
	}

	public async Task PutUnboundActionAsync(N4UnboundAction unboundAction)
	{
		await _noahQuery.PutUnboundActionAsync(unboundAction);
	}

	public async Task PutUnboundActionReferencesAsync(int unboundActionId, int[] unboundActionReferences)
	{
		await _noahQuery.PutUnboundActionReferencesAsync(unboundActionId, unboundActionReferences);
	}

	public async Task PutUserSetupAsync(N4UserSetup userSetup)
	{
		await _noahQuery.PutUserSetupAsync(userSetup);
	}

	public async Task PutDashboardAlertAsync(N4DashboardAlert dashboardAlert)
	{
		await _noahQuery.PutDashboardAlertAsync(dashboardAlert);
	}

	public async Task PutDashboardAlertArchiveAsync(N4DashboardAlertArchive dashboardAlert)
	{
		await _noahQuery.PutDashboardAlertArchiveAsync(dashboardAlert);
	}

	public async Task UpdateDashboardAlertAsync(N4DashboardAlert dashboardAlert)
	{
		await _noahQuery.UpdateDashboardAlertAsync(dashboardAlert);
	}

	public async Task UpdateDashboardAlertArchiveAsync(N4DashboardAlertArchive dashboardAlert)
	{
		await _noahQuery.UpdateDashboardAlertArchiveAsync(dashboardAlert);
	}

	public async Task<N4DashboardAlert> GetDashboardAlertAsync(Guid alertGuid)
	{
		return await _noahQuery.GetDashboardAlertAsync(alertGuid);
	}

	public async Task<N4DashboardAlert[]> GetDashboardAlertsAsync(int assignee, int? userId, int? patientId,
		bool? isRead)
	{
		return await _noahQuery.GetDashboardAlertsAsync(assignee, userId, patientId, isRead);
	}

	public async Task<N4DashboardAlertArchive> GetDashboardAlertArchiveAsync(Guid alertGuid)
	{
		return await _noahQuery.GetDashboardAlertArchiveAsync(alertGuid);
	}

	public async Task RegisterMobileAppAsync(N4MobileApp mobileApp)
	{
		await _noahQuery.RegisterMobileAppAsync(mobileApp);
	}

	public async Task UnregisterMobileAppAsync(int moduleId)
	{
		await _noahQuery.UnregisterMobileAppAsync(moduleId);
	}

	public async Task<N4LoginResult> ValidateLoginAsync(string username, string password)
	{
		return await _noahQuery.ValidateLoginAsync(username, password);
	}

	public async Task<NoahPayload> GetNoahPayloadAsync(int patientId)
	{
		return await _noahQuery.GetNoahPayloadAsync(patientId);
	}

	public async Task<NoahPayload> GetNoahPayloadAsync(Guid patientGuid)
	{
		return await _noahQuery.GetNoahPayloadAsync(patientGuid);
	}

	#endregion NoahService Members
}