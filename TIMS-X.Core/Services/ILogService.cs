using System.Collections.Generic;
using Serilog.Events;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface ILogService
	{
		IEnumerable<LogItem> GetLogItems();
		void ProcessLogEvent(LogEvent logEvent);
	}
}