using System;

namespace SubhadraSolutions.Utils.Net.Http.Configs
{
    public class RateLimiterWrapperConfig : RateLimiterConfig
    {
        public bool TryUntilSucceeded { get; set; } = false;

        public TimeSpan Timeout { get; set; } = NetDefaults.DefaultTimeout;

        public RateLimiterWrapper BuildRateLimiterWrapper()
        {
            var rateLimiter = BuildRateLimiter(this);
            if (rateLimiter == null)
            {
                return null;
            }
            var rateLimiterWrapper = new RateLimiterWrapper(rateLimiter, this.TryUntilSucceeded, this.Timeout);
            return rateLimiterWrapper;
        }
    }
}