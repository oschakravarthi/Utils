using Microsoft.Extensions.Logging;
using SubhadraSolutions.Utils.Logging;
using System;
using System.Globalization;

namespace SubhadraSolutions.Utils;

public sealed class GlobalSettings
{
    private GlobalSettings()
    {
        UseUtcTime = false;
        MachineName = Environment.MachineName;
        // this.ShortDateFormat = "MM/dd/yyyy";
        // this.LongDateFormat = "MM-dd-yyyy hh:mm:ss:tt.fff";
        DateAndTimeSerializationFormat = "yyyy-MM-dd HH:mm:ss.fff";
        DateAndTimeDisplayFormat = "yyyy-MM-dd hh:mm:ss:tt";
        DateOnlySerializationFormat = "yyyy-MM-dd";
        var culture = CultureInfo.CurrentCulture;
        ShortDateFormat = culture.DateTimeFormat.ShortDatePattern;
        //LongDateFormat = culture.DateTimeFormat.LongDatePattern;
        RealNumberFormat = "N" + culture.NumberFormat.NumberDecimalDigits;
        MonthFormat = "yyyy-MM";
        DateFormat = "dd-MM-yyyy";
        TimeFormat = "hh:mm:ss tt";

        var logger = new ConsoleLogger(logAsJson: true, useColors: true);
        DefaultLogger = logger;
        DefaultTraceLogger = logger;
    }

    public string DateAndTimeDisplayFormat { get; set; }
    public static GlobalSettings Instance { get; } = new();

    public string DateAndTimeFormat => $"{DateFormat} {TimeFormat}";

    public string DateAndTimeSerializationFormat { get; set; }
    public string DateFormat { get; set; }
    public string DateOnlySerializationFormat { get; set; }

    public DateTime DateTimeNow => UseUtcTime ? DateTime.UtcNow : DateTime.Now;

    public ILogger DefaultLogger { get; set; }
    public ILogger DefaultTraceLogger { get; set; }
    public string MachineName { get; }
    public string MonthFormat { get; set; }
    public string RealNumberFormat { get; set; }
    public string ShortDateFormat { get; set; }
    public string TimeFormat { get; set; }
    public bool UseUtcTime { get; set; }
}