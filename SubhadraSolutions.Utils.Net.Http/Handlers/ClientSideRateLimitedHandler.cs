using SubhadraSolutions.Utils.Diagnostics;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http.Handlers
{
    public class ClientSideRateLimitedHandler : DelegatingHandler
    {
        private readonly RateLimiterWrapper rateLimiterWrapper;

        public ClientSideRateLimitedHandler(RateLimiterWrapper rateLimiterWrapper)
            : base(new HttpClientHandler())
        {
            this.rateLimiterWrapper = rateLimiterWrapper;
        }

        public ClientSideRateLimitedHandler(RateLimiterWrapper rateLimiterWrapper, HttpMessageHandler httpMessageHandler)
            : base(httpMessageHandler)
        {
            this.rateLimiterWrapper = rateLimiterWrapper;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var begin = SharedStopwatch.Elapsed;
            using RateLimitLease lease = await rateLimiterWrapper.AcquireAsync(cancellationToken).ConfigureAwait(false);
            if (lease.IsAcquired)
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }

            var hasRetry = lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter);
            var response = BuildResponseMessageForTooManyRequests();
            if (hasRetry)
            {
                response.Headers.Add("Retry-After", ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
            }

            return response;
        }

        public static HttpResponseMessage BuildResponseMessageForTooManyRequests()
        {
#if NETFRAMEWORK
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
#else
            return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
#endif
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                rateLimiterWrapper.Dispose();
            }
        }
    }
}