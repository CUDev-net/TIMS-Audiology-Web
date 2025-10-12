using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using NoahDataInterfaces;
using NoahDataObjects;
using NOAHErrorLib;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Noah;
using TIMS_X.Core.Utils;
using N4LoginResult = TIMS_X.NoahDataInterface413.Models.N4LoginResult;

namespace TIMS_X.NoahDataInterface413.Services
{
	public class NoahDataService
	{
		private readonly int _timeout = 50000;

		public NoahDataService()
		{
			RefreshSettings();
			Model = new NoahPayload();
		}

		public NoahPayload Model { get; set; }

		public ClientSettings Settings { get; set; }

		public List<N4User> Users { get; set; }

		private HttpClient _GetHttpClient(bool authenticationRequired)
		{
			var handler = new HttpClientHandler();
//#if DEBUG

			handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true;
//#endif

			var client = new HttpClient(handler);
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			if (authenticationRequired)
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.WebToken);
			else
				client.DefaultRequestHeaders.Add("OfficeCode", Settings.OfficeCode);

			return client;
		}
		
		public bool Delete(string uri)
		{
			try
			{
				RefreshSettings();
				var cts = new CancellationTokenSource();
				cts.CancelAfter(5000);
				using (var client = _GetHttpClient(true))
				{
					var requestPath = $"{Settings.NoahApi}{uri}";
					var response = client.DeleteAsync(requestPath, cts.Token).Result;
					return response.IsSuccessStatusCode;
				}
			}
			catch (Exception ex)
			{
				Log($"Delete<T>(\"{uri}\") error: {ex.Message}");
				return false;
			}
		}

		public void DeleteAction(int actionId)
		{
			Delete($"Action?actionId={actionId}");
			var key = Model.Actions.Where(x => x.Value.Any(p => p.Id == actionId)).Select(x => x.Key).FirstOrDefault();
			if (key > 0) Model.Actions[key].RemoveAll(x => x.Id == actionId);
		}

		public void DeleteActionReferences(int actionId)
		{
			Delete($"ActionReferences?actionId={actionId}");
			if (Model.ActionReferences.TryGetValue(actionId, out var refs)) Model.ActionReferences[actionId] = null;
		}

		public void DeleteAppPermissions(int[] permissionIds)
		{
			var parameters = new ParameterBuilder()
				.Add(nameof(permissionIds), permissionIds)
				.ToString();

			Delete($"AppPermissions?{parameters}");
		}

		public void DeleteArchivedActions(int actionId)
		{
			Delete($"ArchivedActions?actionId={actionId}");
		}

		public void DeleteArchivedUnboundActions(int actionId)
		{
			Delete($"ArchivedUnboundActions?actionId={actionId}");
		}

		public void DeleteDashboardAlert(Guid alertGuid)
		{
			Delete($"DashboardAlert?alertGuid={alertGuid}");
		}

		public void DeleteDashboardAlertArchive(Guid alertGuid)
		{
			Delete($"DashboardAlertArchive?alertGuid={alertGuid}");
		}

		public void DeleteFastView(int actionId)
		{
			Delete($"FastView?actionId={actionId}");
		}

		public void DeleteMobileAppPermissions(int moduleId, int[] permissionIds)
		{
			var parameters = new ParameterBuilder()
				.Add(nameof(permissionIds), permissionIds)
				.ToString();
			Delete($"MobileAppPermissions?moduleId={moduleId}&{parameters}");
		}

		public void DeletePatientIdentification(int patientId, int manufacturerId)
		{
			Delete($"PatientIdentification?patientId={patientId}&manufacturerId={manufacturerId}");
		}

		public void DeletePatientSetup(int patientId, int moduleId)
		{
			Delete($"PatientSetup?patientId={patientId}&moduleId={moduleId}");
		}

		public void DeletePreference(int preferenceId)
		{
			Delete($"Preference?preferenceId={preferenceId}");
		}

		public void DeleteSession(int sessionId)
		{
			Delete($"Session?sessionId={sessionId}");
		}

		public void DeleteUnboundAction(int unboundActionId)
		{
			Delete($"UnboundAction?unboundActionId={unboundActionId}");
		}

		public void DeleteUnboundActionReferences(int actionId)
		{
			Delete($"UnboundActionReferences?actionId={actionId}");
		}

		public void DeleteUserSetup(int userId, int moduleId)
		{
			Delete($"UserSetup?userId={userId}&moduleId={moduleId}");
		}

		public T Get<T>(string path, bool authenticationRequired = true)
		{
			try
			{
				RefreshSettings();
				var cts = new CancellationTokenSource();
				cts.CancelAfter(_timeout);
				using (var client = _GetHttpClient(authenticationRequired))
				{
					var requestPath = $"{Settings.NoahApi}{path}";
					var response = client.GetAsync(requestPath, cts.Token).Result;
					if (response.IsSuccessStatusCode)
					{
						var json = response.Content.ReadAsStringAsync().Result;
						var responseObject = JsonConvert.DeserializeObject<T>(json);
						return responseObject;
					}
				}
			}
			catch (Exception ex)
			{
				Log($"Get<T>(\"{path}\") error: {ex.Message}");
			}

			return default;
		}

		public N4Action GetAction(int actionId)
		{
			return Get<N4Action>($"Action?actionId={actionId}");
		}


		public N4Action GetAction(Guid actionGuid)
		{
			return Get<N4Action>($"Action2?actionGuid={actionGuid}");
		}

		public IEnumerable<N4Action> GetActionCollection(int moduleId)
		{
			return Get<IEnumerable<N4Action>>($"ActionCollection?moduleId={moduleId}");
		}

		public int GetActionCollectionCount(int moduleId)
		{
			return Get<int>($"ActionCollectionCount?moduleId={moduleId}");
		}

		public N4ActionData GetActionData(int actionId)
		{
			return Get<N4ActionData>($"ActionData?actionId={actionId}");
		}

		public int[] GetActionIdsFromDashboardGroup(Guid group)
		{
			return Get<int[]>($"ActionIdsFromDashboardGroup?group={group}");
		}

		public int[] GetActionReferencedBy(int actionId)
		{
			return Get<int[]>($"ActionReferencedBy?actionId={actionId}");
		}

		public int[] GetActionReferences(int actionId)
		{
			if (!Model.ActionReferences.ContainsKey(actionId))
				Model.ActionReferences[actionId] = Get<int[]>($"ActionReferences?actionId={actionId}");
			return Model.ActionReferences[actionId];
		}

		public IEnumerable<N4Action> GetActions(int sessionId)
		{
			try
			{
				if (!Model.Actions.ContainsKey(sessionId))
					Model.Actions[sessionId] = Get<List<N4Action>>($"Actions?sessionId={sessionId}");

				return Model.Actions[sessionId];
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public IEnumerable<N4Action> GetActions(int patientId, DateTime? actionGroup, bool returnLatest,
			int[] dataTypes, int[] moduleIds)
		{
			var parameters = new ParameterBuilder()
				.Add(nameof(patientId), patientId)
				.Add(nameof(actionGroup), actionGroup)
				.Add(nameof(dataTypes), dataTypes)
				.Add(nameof(moduleIds), moduleIds)
				.Add(nameof(returnLatest), returnLatest)
				.ToString();

			var actions = Get<IEnumerable<N4Action>>($"SearchActions?{parameters}");
			return actions;
		}

		public IEnumerable<N4AppPermission> GetAppPermissions()
		{
			return Get<IEnumerable<N4AppPermission>>("AppPermissions");
		}

		public IEnumerable<N4Action> GetArchivedActions(int actionId)
		{
			return Get<IEnumerable<N4Action>>($"ArchivedActions?actionId={actionId}");
		}

		public int GetArchivedActionsCount(int actionId)
		{
			return Get<int>($"ArchivedActionsCount?actionId={actionId}");
		}

		public IEnumerable<N4UnboundAction> GetArchivedUnboundActions(int actionId)
		{
			return Get<IEnumerable<N4UnboundAction>>($"ArchivedUnboundActions?actionId={actionId}");
		}

		public int GetArchivedUnboundActionsCount(int actionId)
		{
			return Get<int>($"ArchivedUnboundActionsCount?actionId={actionId}");
		}

		public ConnectionInfo GetConnectionInfo()
		{
			return Get<ConnectionInfo>(nameof(ConnectionInfo));
		}

		public N4DashboardAlert GetDashboardAlert(Guid alertGuid)
		{
			return Get<N4DashboardAlert>($"DashboardAlert?alertGuid={alertGuid}");
		}

		public N4DashboardAlertArchive GetDashboardAlertArchive(Guid alertGuid)
		{
			return Get<N4DashboardAlertArchive>($"DashboardAlertArchive?alertGuid={alertGuid}");
		}

		public N4DashboardAlert[] GetDashboardAlerts(DashboardAlertSearchOptions searchOptions)
		{
			var parameters = new ParameterBuilder()
				.Add("assignee", (int)searchOptions.Assignee)
				.Add("userId", searchOptions.UserID)
				.Add("patientId", searchOptions.PatientID)
				.Add("isRead", searchOptions.IsRead)
				.ToString();
			return Get<N4DashboardAlert[]>($"DashboardAlerts?{parameters}");
		}

		public N4FastView GetFastView(int actionId)
		{
			return Get<N4FastView>($"FastView?actionId={actionId}");
		}

		public string[] GetManufacturerSetupKeys(int manufacturerId)
		{
			return Get<string[]>($"ManufacturerSetupKeys?manufacturerId={manufacturerId}");
		}

		public IEnumerable<N4ManufacturerSetup> GetManufacturerSetups(int manufacturerId, string[] manufacturerKeys)
		{
			var parameters = new ParameterBuilder()
				.Add(nameof(manufacturerKeys), manufacturerKeys)
				.ToString();
			return Get<IEnumerable<N4ManufacturerSetup>>(
				$"ManufacturerSetups?manufacturerId={manufacturerId}&{parameters}");
		}

		public int[] GetMobileAppPermissions(int moduleId)
		{
			return Get<int[]>($"MobileAppPermissions?moduleId={moduleId}");
		}

		public IEnumerable<N4MobileApp> GetMobileApps()
		{
			return Get<IEnumerable<N4MobileApp>>("MobileApps");
		}

		public N4Patient GetPatient(int patientId)
		{
			LoadPatient(patientId);
			return Model?.Patient;
			//return Get<N4Patient>($"Patient?patientId={patientId}");
		}

		public N4Patient GetPatient(Guid patientGuid)
		{
			LoadPatient(patientGuid);
			return Model?.Patient;
			//return Get<N4Patient>($"Patient?patientId={patientId}");
		}

		public int[] GetPatientIdsFromIdentification(string identification, int manufacturerId)
		{
			return Get<int[]>(
				$"PatientIdsFromIdentification?identification={identification}&manufacturerId={manufacturerId}");
		}

		public IEnumerable<N4Patient> GetPatients(string searchText, int page, int pageSize)
		{
			return Get<IEnumerable<N4Patient>>($"Patients?searchText={searchText}&page={page}&pageSize={pageSize}");
		}

		public int GetPatientsCount(string searchText)
		{
			return Get<int>($"PatientsCount?searchText={searchText}");
		}

		public N4PatientSetup GetPatientSetup(int patientId, int moduleId)
		{
			return Get<N4PatientSetup>($"PatientSetup?patientId={patientId}&moduleId={moduleId}");
		}

		public IEnumerable<N4PatientSetup> GetPatientSetups(int patientId, int moduleId)
		{
			return Get<IEnumerable<N4PatientSetup>>($"PatientSetups?patientId={patientId}&moduleId={moduleId}");
		}

		public IEnumerable<N4PatientSetup> GetPatientSetupsForModule(int moduleId)
		{
			return Get<IEnumerable<N4PatientSetup>>($"PatientSetupsForModule?moduleId={moduleId}");
		}

		public N4Preference GetPreference(int preferenceId)
		{
			return Get<N4Preference>($"Preference?preferenceId={preferenceId}");
		}

		public N4Session GetSession(int sessionId)
		{
			var session = Get<N4Session>($"Session?sessionId={sessionId}");
			if (session == null) throw new NoahDatabaseException(NOAHErrorType.NOAH_E_DB_SESSION_NOT_FOUND);
			return session;
		}

		public IEnumerable<N4Session> GetSessions(int patientId)
		{
			if (Model.Patient != null && Model.Patient.Id == patientId) return Model.Sessions;
			return Get<IEnumerable<N4Session>>($"Sessions?patientId={patientId}");
		}

		public N4UnboundAction GetUnboundAction(int actionId)
		{
			return Get<N4UnboundAction>($"UnboundAction?actionId={actionId}");
		}

		public N4ActionData GetUnboundActionData(int actionId)
		{
			return Get<N4ActionData>($"UnboundActionData?actionId={actionId}");
		}

		public int[] GetUnboundActionIds(DateTime startDate, DateTime endDate)
		{
			return Get<int[]>($"UnboundActionIds?startDate={startDate}&endDate={endDate}");
		}

		public int[] GetUnboundActionReferences(int actionId)
		{
			return Get<int[]>($"UnboundActionReferences?actionId={actionId}");
		}

		public IEnumerable<N4UnboundAction> GetUnboundActions(int[] actionIds)
		{
			var parameters = new ParameterBuilder()
				.Add(nameof(actionIds), actionIds)
				.ToString();
			return Get<IEnumerable<N4UnboundAction>>($"UnboundActions?{parameters}");
		}

		public IEnumerable<N4Action> GetUpdatedActions(int page, int pageSize, DateTime startTime,
			DateTime? endTime, int[] dataTypes)
		{
			var parameters = new ParameterBuilder()
				.Add(nameof(page), page)
				.Add(nameof(pageSize), pageSize)
				.Add(nameof(startTime), startTime)
				.Add(nameof(endTime), endTime)
				.Add(nameof(dataTypes), dataTypes)
				.ToString();

			return Get<IEnumerable<N4Action>>($"UpdatedActions?{parameters}");
		}

		public IEnumerable<N4Patient> GetUpdatedPatients(int page, int pageSize, DateTime startTime, DateTime? endTime)
		{
			var parameters = new ParameterBuilder()
				.Add(nameof(page), page)
				.Add(nameof(pageSize), pageSize)
				.Add(nameof(startTime), startTime)
				.Add(nameof(endTime), endTime)
				.ToString();

			return Get<IEnumerable<N4Patient>>($"UpdatedPatients?{parameters}");
		}

		public N4User GetUser(int userId)
		{
			if (Users == null) Users = GetUsers().ToList();
			//return Get<N4User>($"User?userId={userId}");
			return Users.FirstOrDefault(x => x.Id == userId);
		}

		public N4User GetUser(string username)
		{
			return Get<N4User>($"User?username={username}");
		}

		public bool GetUserPrivilege(int userId)
		{
			return Get<bool>($"UserPrivilege?userId={userId}");
		}

		internal IEnumerable<N4User> GetUsers()
		{
			return Get<IEnumerable<N4User>>("Users");
		}

		public N4UserSetup GetUserSetup(int userId, int moduleId)
		{
			return Get<N4UserSetup>($"UserSetup?userId={userId}&moduleId={moduleId}");
		}

		public void LoadPatient(int patientId)
		{
			Model = Get<NoahPayload>($"Payload?patientId={patientId}");
		}

		public void LoadPatient(Guid patientGuid)
		{
			Model = Get<NoahPayload>($"Payload2?patientGuid={patientGuid}");
		}

		public void Log(string logMessage)
		{
#if DEBUG
			Debug.WriteLine($"\r\n{logMessage}\r\n");
#endif
			if (string.IsNullOrWhiteSpace(Settings.DbInterfaceLogFile))
				return;

			using (var streamWriter = File.AppendText(Settings.DbInterfaceLogFile))
			{
				streamWriter.WriteLine($"[{DateTime.Now:MM/dd/yyyy HH:mm tt}] {logMessage}");
			}
		}

		public T Post<T>(string path, object body, bool authenticationRequired = true)
		{
			try
			{
				RefreshSettings();
				var cts = new CancellationTokenSource();
				cts.CancelAfter(_timeout);
				using (var client = _GetHttpClient(authenticationRequired))
				{
					var requestPath = $"{Settings.NoahApi}{path}";
					var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
						"application/json");
					var response = client.PostAsync(requestPath, content, cts.Token).Result;
					if (response.IsSuccessStatusCode)
					{
						var json = response.Content.ReadAsStringAsync().Result;
						var responseObject = JsonConvert.DeserializeObject<T>(json);
						return responseObject;
					}
				}
			}
			catch (Exception ex)
			{
				Log($"Post<T>(\"{path}\") error: {ex.Message}");
			}

			return default;
		}

		public bool Put<T>(string path, T body, bool authenticationRequired = true)
		{
			try
			{
				RefreshSettings();
				var cts = new CancellationTokenSource();
				cts.CancelAfter(_timeout);
				using (var client = _GetHttpClient(authenticationRequired))
				{
					var requestPath = $"{Settings.NoahApi}{path}";
					var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
						"application/json");
					var response = client.PutAsync(requestPath, content, cts.Token).Result;
					return response.IsSuccessStatusCode;
				}
			}
			catch (Exception ex)
			{
				Log($"Put<T>(\"{path}\") error: {ex.Message}");
				return false;
			}
		}

		public bool Put<T>(string path, T body, out int resultId, bool authenticationRequired = true)
		{
			resultId = 0;
			try
			{
				RefreshSettings();
				var cts = new CancellationTokenSource();
				cts.CancelAfter(_timeout);
				using (var client = _GetHttpClient(authenticationRequired))
				{
					var requestPath = $"{Settings.NoahApi}{path}";
					var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
						"application/json");
					var response = client.PutAsync(requestPath, content, cts.Token).Result;
					var success = response.IsSuccessStatusCode;
					if (success) int.TryParse(response.Content.ReadAsStringAsync().Result, out resultId);

					return success;
				}
			}
			catch (Exception ex)
			{
				Log($"Put<T>(\"{path}\") error: {ex.Message}");

				return false;
			}
		}

		public bool Put<T>(string path, T body, out IdPair resultId, bool authenticationRequired = true)
		{
			resultId = new IdPair();
			try
			{
				RefreshSettings();
				var cts = new CancellationTokenSource();
				cts.CancelAfter(_timeout);
				using (var client = _GetHttpClient(authenticationRequired))
				{
					var requestPath = $"{Settings.NoahApi}{path}";
					var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
						"application/json");
					var response = client.PutAsync(requestPath, content, cts.Token).Result;
					var success = response.IsSuccessStatusCode;
					if (success)
						try
						{
							resultId = JsonConvert.DeserializeObject<IdPair>(response.Content
								.ReadAsStringAsync().Result);
						}
						catch
						{
							// do nothing
						}

					return success;
				}
			}
			catch (Exception ex)
			{
				Log($"Put<T>(\"{path}\") error: {ex.Message}");

				return false;
			}
		}

		public IdPair PutAction(N4Action action)
		{
			Put("Action", action, out IdPair actionIds);
			if (actionIds.Guid != Guid.Empty)
				if (Model.Sessions.Any(x => x.Id == action.SessionId))
				{
					if (Model.Actions.TryGetValue(action.SessionId, out var actions))
						Model.Actions[action.SessionId].Add(action);
					else
						Model.Actions[action.SessionId] = new List<N4Action> { action };
				}

			return actionIds;
		}

		public void PutActionReferences(int actionId, int[] actionReferences)
		{
			Put($"ActionReferences?actionId={actionId}", actionReferences);
			Model.ActionReferences[actionId] = actionReferences;
		}

		public void PutAppPermissions(ArrayWrapper<N4AppPermission> appPermissions)
		{
			Put("AppPermissions", appPermissions);
		}

		public void PutArchiveAction(int actionId)
		{
			Put("ArchiveAction", actionId);
		}

		public void PutArchiveUnboundAction(int actionId)
		{
			Put("ArchiveUnboundAction", actionId);
		}

		public void PutDashboardAlert(N4DashboardAlert dashboardAlert)
		{
			Put("DashboardAlert", dashboardAlert);
		}

		public void PutDashboardAlertArchive(N4DashboardAlertArchive dashboardAlert)
		{
			Put("DashboardAlertArchive", dashboardAlert);
		}

		public void PutManufacturerSetups(ArrayWrapper<N4ManufacturerSetup> manufacturerSetups)
		{
			Put("ManufacturerSetups", manufacturerSetups);
		}

		public void PutMobileAppPermissions(int moduleId, int[] permissionIds)
		{
			Put($"MobileAppPermissions?moduleId={moduleId}", permissionIds);
		}

		public void PutPatientIdentification(N4PatientIdentification patientIdentification)
		{
			Put("PatientIdentification", patientIdentification);
		}

		public void PutPatientSetup(N4PatientSetup patientSetup)
		{
			Put("PatientSetup", patientSetup);
		}

		public void PutPreference(int preferenceId, byte[] preference)
		{
			Put($"Preference?preferenceId={preferenceId}", preference);
		}

		public int PutSession(N4Session session)
		{
			Put("Session", session, out int sessionId);
			Model.Sessions.Add(session);
			return sessionId;
		}

		public IdPair PutUnboundAction(N4UnboundAction unboundAction)
		{
			Put("UnboundAction", unboundAction, out IdPair actionIds);
			return actionIds;
		}


		public void PutUnboundActionReferences(int unboundActionId, int[] unboundActionReferences)
		{
			Put($"UnboundActionReferences?unboundActionId={unboundActionId}", unboundActionReferences);
		}

		public void PutUserSetup(N4UserSetup userSetup)
		{
			Put("UserSetup", userSetup);
		}

		public void RefreshSettings()
		{
			try
			{
				var path = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					@"TIMS\Settings.json");
				var contents = File.ReadAllText(path, Encoding.ASCII);
				if (string.IsNullOrWhiteSpace(contents))
					Settings = new ClientSettings();
				else
					Settings = JsonConvert.DeserializeObject<ClientSettings>(contents);
				//JsonConvert.DeserializeObject()
			}
			catch (Exception)
			{
				/* DO NOTHING */
			}
		}

		public void RegisterMobileApp(N4MobileApp mobileApp)
		{
			Put("RegisterMobileApp", mobileApp);
		}

		public void UnregisterMobileApp(int moduleId)
		{
			Delete($"UnregisterMobileApp?moduleId={moduleId}");
		}

		public void UpdateDashboardAlert(N4DashboardAlert dashboardAlert)
		{
			Put("UpdateDashboardAlert", dashboardAlert);
		}

		public void UpdateDashboardAlertArchive(N4DashboardAlertArchive dashboardAlert)
		{
			Put("UpdateDashboardAlertArchive", dashboardAlert);
		}

		public int ValidateLogin(string username, string password)
		{
			var result = Post<N4LoginResult>("ValidateLogin", new { Username = username, Password = password }, false);
			if (result == null)
				return -1;
			//The token is saved by the client app
			//Settings.Token = result.JwtToken;
			return result.UserId;
		}
	}
}