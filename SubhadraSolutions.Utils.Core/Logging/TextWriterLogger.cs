using Microsoft.Extensions.Logging;
using System.IO;

namespace SubhadraSolutions.Utils.Logging
{
    internal class TextWriterLogger : AbstractLoggerBase
    {
        private readonly TextWriter writer;

        public TextWriterLogger(TextWriter writer, bool logAsJson, params LogLevel[] enabledLogLevels)
            : base(logAsJson, enabledLogLevels)
        {
            this.writer = writer;
        }

        protected override void Log(LogLevel logLevel, string message)
        {
            this.writer.WriteLine(message);
        }
    }
}