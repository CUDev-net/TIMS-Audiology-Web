using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Events;

namespace TIMS_X.Core.Models
{
    public class LogItem
    {
        public LogItem(LogEvent logEvent)
        {
            Timestamp = logEvent.Timestamp;
            SeverityLevel = logEvent.Level;
            Message = logEvent.RenderMessage();
        }

        public LogItem()
        {

        }
        public DateTimeOffset Timestamp { get; set; }
        public LogEventLevel SeverityLevel { get; set; }
        public string Message { get; set; }
    }
}
