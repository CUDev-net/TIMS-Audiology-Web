using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface ISchedulerService
	{
		Task<Dictionary<int, List<ScheduledPatientItem>>> GetPatientsScheduledTodayAsync(IEnumerable<int> providerIds);
	}
}