using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;
using TIMS_X.Core.Utils;

namespace TIMS_X.CloudDataServices
{
	public class SchedulerService : ISchedulerService
	{
		private readonly IConfigurationService _configurationService;
		private readonly ContextHelper _contextHelper;
		private readonly IRequestService _requestService;

		public SchedulerService(IRequestService requestService, IConfigurationService configurationService,
			ContextHelper contextHelper)
		{
			_requestService = requestService;
			_configurationService = configurationService;
			_contextHelper = contextHelper;
		}

		public async Task<Dictionary<int, List<ScheduledPatientItem>>> GetPatientsScheduledTodayAsync(
			IEnumerable<int> providerIds)
		{
			var builder = new UriBuilder(_configurationService.SchedulerDataServiceUrl);
			builder.AppendToPath("PatientsScheduledToday");
			var paramBuilder = new ParameterBuilder();
			paramBuilder.AddRange(nameof(providerIds), providerIds);
			builder.Query = paramBuilder.ToString();

			var uri = builder.ToString();
			return await _requestService
				.GetAsync<Dictionary<int, List<ScheduledPatientItem>>>(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
		}
	}
}