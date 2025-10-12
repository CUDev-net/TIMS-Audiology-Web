using System.Collections.Generic;
using TIMS_X.Core.Models;

namespace TIMS_X.CloudAssistant.Models
{
	public class LocalCache
	{
		public List<ProviderItem> Providers { get; set; }
		public List<PatientItem> RecentPatients { get; set; }
		public Dictionary<int, List<ScheduledPatientItem>> TodayPatients { get; set; }
	}
}