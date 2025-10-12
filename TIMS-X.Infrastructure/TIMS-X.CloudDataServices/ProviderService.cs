using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;

namespace TIMS_X.CloudDataServices
{
	public class ProviderService : IProviderService
	{
		private readonly IConfigurationService _configurationService;
		private readonly ContextHelper _contextHelper;
		private readonly IRequestService _requestService;

		public ProviderService(IRequestService requestService, IConfigurationService configurationService,
			ContextHelper contextHelper)
		{
			_requestService = requestService;
			_configurationService = configurationService;
			_contextHelper = contextHelper;
		}

		public async Task<IEnumerable<ProviderItem>> GetProvidersAsync(bool includeInactive)
		{
			var builder = new UriBuilder(_configurationService.ProviderDataServiceUrl);
			builder.AppendToPath("All");
			builder.Query = $"includeInactive={includeInactive}";
			var uri = builder.ToString();
			return await _requestService.GetAsync<IEnumerable<ProviderItem>>(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
		}
	}
}