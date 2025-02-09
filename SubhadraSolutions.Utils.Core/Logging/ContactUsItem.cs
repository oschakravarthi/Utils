using System;

namespace SubhadraSolutions.Utils.Logging;

public class ContactUsItem
{
    public ContactUsItem()
    {
        Timestamp = GlobalSettings.Instance.DateTimeNow;
#if DEBUG
        Environment = "DEV";
#else
this.Environment = "PROD";
#endif
    }

    public DateTime Timestamp { get; set; }
    public string ProductName { get; set; }
    public string ProductVersion { get; set; }
    public string Environment { get; set; }
    public string Intent { get; set; }

    public string Message { get; set; }

    public int Rating { get; set; }
}