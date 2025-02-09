using SubhadraSolutions.Utils.Net.Http.Configs;
using System;
using System.Linq;
using System.Net.Http;

namespace SubhadraSolutions.Utils.Net.Http.Handlers
{
    public static class HandlerHelper
    {
        private static readonly int TooManyRequestsStatusCode = 429;

        public static HttpMessageHandler BuildHttpMessageHandler(this RateLimiterWrapperConfig config, params int[] retryStatusCodes)
        {
            var rateLimiter = RateLimiterConfig.BuildRateLimiter(config);
            if (rateLimiter == null)
            {
                return null;
            }

            var rateLimiterWrapper = new RateLimiterWrapper(rateLimiter, config.TryUntilSucceeded, config.Timeout);
            HttpMessageHandler handler = new ClientSideRateLimitedHandler(rateLimiterWrapper);
            if (config.TryUntilSucceeded)
            {
                if (!retryStatusCodes.Contains(TooManyRequestsStatusCode))
                {
                    retryStatusCodes = retryStatusCodes.Concat([TooManyRequestsStatusCode]).ToArray();
                }
                handler = new RetryHandler(retryStatusCodes, config.Timeout, handler);
            }

            return handler;
        }
    }
}