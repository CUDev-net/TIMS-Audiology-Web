using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface IPatientService
	{
		Task<byte[]> GetPatientNHAXAsync(int patientId);

		Task<IEnumerable<PatientItem>> SearchPatientsAsync(string queryString, SearchType searchType,
			bool includeInactive);
	}
}