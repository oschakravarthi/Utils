using System;

namespace SubhadraSolutions.Utils.Logging;

public class LogItem
{
    public LogItem()
    {
        Timestamp = GlobalSettings.Instance.DateTimeNow;
#if DEBUG
        Environment = "DEV";
#else
this.Environment = "PROD";
#endif
    }

    public DateTime Timestamp { get; set; }
    public int EventId { get; set; }
    public string EventName { get; set; }
    public string ProductName { get; set; }
    public string ProductVersion { get; set; }
    public string Environment { get; set; }
    public string Message { get; set; }
    public string LogLevel { get; set; }
    public string Exception { get; set; }

    public override string ToString()
    {
        return this.ExportAsString();
    }
}