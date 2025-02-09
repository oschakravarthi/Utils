//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.ApplicationInsights;
//using Microsoft.Extensions.Logging;
//using SubhadraSolutions.Utils.Telemetry.Metrics;

//namespace SubhadraSolutions.Utils.ApplicationInsights
//{
//    public class Logger : ILogger
//    {
//        private readonly TelemetryClient telemetryClient;
//        public Logger(TelemetryClient telemetryClient)
//        {
//            this.telemetryClient = telemetryClient;
//        }
//        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
//        {
//            throw new NotImplementedException();
//        }

//        public bool IsEnabled(LogLevel logLevel)
//        {
//            throw new NotImplementedException();
//        }

//        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
//        {
//            this.telemetryClient.TrackTrace()
//        }
//    }
//}