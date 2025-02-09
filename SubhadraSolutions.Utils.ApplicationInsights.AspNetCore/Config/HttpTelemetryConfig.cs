namespace SubhadraSolutions.Utils.ApplicationInsights.AspNetCore.Config
{
    public class HttpTelemetryConfig
    {
        public bool LogRequestBody { get; set; }
        public bool LogResponseBody { get; set; }
        public AlwaysOrOnError Condition { get; set; }
    }
}