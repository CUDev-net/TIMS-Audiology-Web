using System.Collections.Generic;
using Serilog.Events;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;

namespace TIMS_X.CoreServices
{
	public class LogService : ILogService
	{
		private readonly List<LogItem> _logItems;

		public LogService()
		{
			_logItems = new List<LogItem>();
		}

		public void ProcessLogEvent(LogEvent logEvent)
		{
			_logItems.Add(new LogItem(logEvent));
		}

		public IEnumerable<LogItem> GetLogItems()
		{
			return _logItems;
		}
	}
}