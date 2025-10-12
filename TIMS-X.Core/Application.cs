using System;
using System.Reactive.Linq;
using Serilog;
using Serilog.Events;

namespace TIMS_X.Core
{
    public static class Application
    {
        public static void Initialize(Action<LogEvent> logEventHandler)
        {
            InitializeLogging(logEventHandler);
        }

        public static void InitializeLogging(Action<LogEvent> logEventHandler)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Observers(events => events
                    .Do(logEventHandler)
                    .Subscribe())
                .CreateLogger();
        }

        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}