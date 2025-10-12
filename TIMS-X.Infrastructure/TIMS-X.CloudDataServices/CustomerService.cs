using System;
using System.Threading.Tasks;
using Serilog;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;

namespace TIMS_X.CloudDataServices
{
	public class CustomerService : ICustomerService
	{
		private readonly IConfigurationService _configurationService;
		private readonly IRequestService _requestService;

		public CustomerService(IRequestService requestService, IConfigurationService configurationService)
		{
			_requestService = requestService;
			_configurationService = configurationService;
		}

		public async Task<bool> ValidateCustomerAsync(string officeCode, string server, string port)
		{
			if (string.IsNullOrEmpty(officeCode))
				return false;
			var clientSettings = new ClientSettings { ServerUrl = server, ServerPort = port, OfficeCode = officeCode };
			var builder = new UriBuilder(clientSettings.CustomerApi);
			builder.AppendToPath("ValidateCustomer");
			var uri = builder.ToString();
			try
			{
				var isValid = await _requestService
					.GetAsync<bool>(uri, client => { client.DefaultRequestHeaders.Add("OfficeCode", officeCode); })
					.ConfigureAwait(false);

				return isValid;
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				return false;
			}
		}

		public async Task<bool> PingAsync(string serverUrl, string port)
		{
			var uri = $"{serverUrl}:{port}/api/Customer/Ping";
			try
			{
				var isValid = await _requestService
					.GetAsync<bool>(uri, client => { client.Timeout = TimeSpan.FromSeconds(3); }).ConfigureAwait(false);

				return isValid;
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				return false;
			}
		}

		public async Task<ConnectionInfo> GetConnectionInfoAsync(string officeCode)
		{
			var builder = new UriBuilder(_configurationService.CustomerDataServiceUrl);
			builder.AppendToPath("ConnectionInfo");
			var uri = builder.ToString();
			try
			{
				var connectionInfo = await _requestService
					.GetAsync<ConnectionInfo>(uri,
						client => { client.DefaultRequestHeaders.Add("OfficeCode", officeCode); })
					.ConfigureAwait(false);

				return connectionInfo;
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				return null;
			}
		}
	}
}