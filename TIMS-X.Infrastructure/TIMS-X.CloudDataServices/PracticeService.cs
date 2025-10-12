using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;

namespace TIMS_X.CloudDataServices
{
	public class PracticeService : IPracticeService
	{
		private readonly IConfigurationService _configurationService;
		private readonly ContextHelper _contextHelper;
		private readonly IRequestService _requestService;

		public PracticeService(IRequestService requestService, IConfigurationService configurationService,
			ContextHelper contextHelper)
		{
			_requestService = requestService;
			_configurationService = configurationService;
			_contextHelper = contextHelper;
		}
		
		public async Task<string> GetPracticeNameAsync(string officeCode)
		{
			var builder = new UriBuilder(_configurationService.PracticeDataServiceUrl);
			builder.AppendToPath("Name");
			var uri = builder.ToString();
			return await _requestService
				.GetAsync<string>(uri, client => { client.DefaultRequestHeaders.Add("OfficeCode", officeCode); })
				.ConfigureAwait(false);
		}

		public async Task<bool> UsesAdAuthenticationAsync(string officeCode)
		{
			var builder = new UriBuilder(_configurationService.PracticeDataServiceUrl);
			builder.AppendToPath("UsesAdAuthentication");

			_contextHelper.UseActiveDirectoryAuthentication = _configurationService.Settings.UseADAuthentication;

			return _contextHelper.UseActiveDirectoryAuthentication;
		}

		public async Task<string> GetServerVersionAsync()
		{
			var builder = new UriBuilder(_configurationService.PracticeDataServiceUrl);
			builder.AppendToPath("GetVersion");
			var uri = builder.ToString();
			return await _requestService.GetAsync<string>(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
		}

		public async Task<IEnumerable<SiteItem>> GetSitesAsync(bool includeInactive, string officeCode)
		{
			var builder = new UriBuilder(_configurationService.PracticeDataServiceUrl);
			builder.AppendToPath("Sites");
			builder.Query = $"includeInactive={includeInactive}";
			var uri = builder.ToString();
			return await _requestService
				.GetAsync<IEnumerable<SiteItem>>(uri,
					client => { client.DefaultRequestHeaders.Add("OfficeCode", officeCode); }).ConfigureAwait(false);
/*            return await _requestService.GetAsync<IEnumerable<SiteItem>>(uri, _contextHelper.CurrentUser.Jwt)
                .ConfigureAwait(false);*/
		}

		public async Task<ConnectionTestResult> TestConnectionAsync(string officeCode)
		{
			var builder = new UriBuilder(_configurationService.PracticeDataServiceUrl);
			builder.AppendToPath("TestConnection");
			builder.Query = "officeCode=" + officeCode;
			var uri = builder.ToString();
			try
			{
				var connected = await _requestService
					.GetAsync<bool>(uri, client => { client.DefaultRequestHeaders.Add("OfficeCode", officeCode); })
					.ConfigureAwait(false);

				if (!connected) return ConnectionTestResult.DatabaseFailure;
			}
			catch (Exception ex)
			{
				return ConnectionTestResult.GatewayFailure;
			}

			return ConnectionTestResult.Success;
		}
	}
}