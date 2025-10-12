using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;

namespace TIMS_X.CloudDataServices
{
	public class PatientService : IPatientService
	{
		private readonly IConfigurationService _configurationService;
		private readonly ContextHelper _contextHelper;
		private readonly IRequestService _requestService;

		public PatientService(IRequestService requestService, IConfigurationService configurationService,
			ContextHelper contextHelper)
		{
			_requestService = requestService;
			_configurationService = configurationService;
			_contextHelper = contextHelper;
		}

		public async Task<byte[]> GetPatientNHAXAsync(int patientId)
		{
			var builder = new UriBuilder(_configurationService.NoahDataServiceUrl);
			builder.AppendToPath("NHAX");
			builder.Query = $"patientId={patientId}";

			var uri = builder.ToString();
			var result = await _requestService.GetAsync<byte[]>(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
			return result;
		}

		public async Task<IEnumerable<PatientItem>> SearchPatientsAsync(string queryString, SearchType searchType,
			bool includeInactive)
		{
			var builder = new UriBuilder(_configurationService.PatientDataServiceUrl);
			builder.AppendToPath("Search");
			builder.Query = $"query={queryString}&searchType={searchType}&includeInactive={includeInactive}";

			var uri = builder.ToString();
			var result = await _requestService.GetAsync<IEnumerable<PatientItem>>(uri, _contextHelper.CurrentUser.Jwt)
				.ConfigureAwait(false);
			return result;
		}
	}
}