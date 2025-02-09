using SubhadraSolutions.Utils.Data.Annotations;
using System;

namespace SubhadraSolutions.Utils.Net
{
    public static class NetDefaults
    {
        [Configurable]
        public static TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        [Configurable]
        public static TimeSpan WaitTimeBeforeReattempt { get; set; } = TimeSpan.FromMilliseconds(100);

        public const string CorrelationIdHeaderName = "X-Correlation-ID";
        public const string RemoteLinqIdHeaderName = "X-RemoteLinq-ID";
    }
}