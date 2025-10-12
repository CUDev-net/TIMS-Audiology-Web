using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Exceptions;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;
using TIMS_X.Core.Utils;

namespace TIMS_X.CloudDataServices
{
	public class UserService : IUserService
	{
		private readonly IConfigurationService _configurationService;
		private readonly ContextHelper _contextHelper;
		private readonly IRequestService _requestService;

		public UserService(IRequestService requestService, IConfigurationService configurationService,
			ContextHelper contextHelper)
		{
			_requestService = requestService;
			_configurationService = configurationService;
			_contextHelper = contextHelper;
		}

		public async Task<List<PatientItem>> GetRecentPatientsListAsync(int userId)
		{
			var builder = new UriBuilder(_configurationService.UserDataServiceUrl);
			builder.AppendToPath("RecentPatientsList");
			var paramBuilder = new ParameterBuilder().Add(nameof(userId), userId);
			builder.Query = paramBuilder.ToString();
			var uri = builder.ToString();
			var patients = await _requestService.GetAsync<List<PatientItem>>(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
			return patients;
		}

		public async Task PutRecentPatientAsync(int userId, int patientId)
		{
			var builder = new UriBuilder(_configurationService.UserDataServiceUrl);
			builder.AppendToPath("AddRecentPatient");
			builder.Query = new ParameterBuilder().Add(nameof(userId), userId)
				.Add(nameof(patientId), patientId)
				.ToString();
			var uri = builder.ToString();
			await _requestService.GetAsync(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
		}

		public async Task<int> GetFirstUserIdAsync()
		{
			var builder = new UriBuilder(_configurationService.UserDataServiceUrl);
			builder.AppendToPath("FirstUserId");
			var uri = builder.ToString();
			var userId = await _requestService.GetAsync<int>(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
			return userId;
		}

		public async Task<bool> LoginAsync(string officeCode, int siteId, UserItem userItem, string password)
		{
			var builder = new UriBuilder(_configurationService.UserDataServiceUrl);
			builder.AppendToPath("Authenticate");
			var uri = builder.ToString();
			var form = new AuthenticationForm
			{
				UserId = userItem.Id,
				SiteId = siteId,
				AdDomain = userItem.AdDomain,
				AdUsername = userItem.AdUsername,
				Password = password,
				TimsToken = "A72Y5h7541254882e37039d28429l293dl34kl"
			};
			try
			{
				var user = await _requestService.PostAsync<AuthenticationForm, User>(uri, form,
					client => { client.DefaultRequestHeaders.Add("OfficeCode", officeCode); }).ConfigureAwait(false);

				_contextHelper.CurrentUser = user;
				_contextHelper.OfficeCode = officeCode;

				return user != null;
			}
			catch (UnauthorizedUserException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<IEnumerable<UserItem>> GetLoginUsersAsync(bool includeInactive, string officeCode)
		{
			var builder = new UriBuilder(_configurationService.UserDataServiceUrl);
			builder.AppendToPath("LoginUsers");
			var uri = builder.ToString();

			var users = await _requestService
				.GetAsync<IEnumerable<UserItem>>(uri,
					client => { client.DefaultRequestHeaders.Add("OfficeCode", officeCode); }).ConfigureAwait(false);
			return users;
		}
	}
}