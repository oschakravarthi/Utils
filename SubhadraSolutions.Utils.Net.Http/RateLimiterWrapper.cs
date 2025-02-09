using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Diagnostics;
using System;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http
{
    public class RateLimiterWrapper : AbstractDisposable
    {
        private readonly RateLimiter rateLimiter;

        public RateLimiterWrapper(RateLimiter rateLimiter, bool tryUntilSucceeded, TimeSpan timeout)
        {
            this.rateLimiter = rateLimiter;
            this.TryUntilSucceeded = tryUntilSucceeded;
            this.Timeout = timeout == TimeSpan.Zero ? NetDefaults.DefaultTimeout : timeout;
        }

        public bool TryUntilSucceeded { get; private set; }
        public TimeSpan Timeout { get; private set; }

        public async Task<RateLimitLease> AcquireAsync(CancellationToken cancellationToken)
        {
            var begin = SharedStopwatch.Elapsed;
            while (true)
            {
                var lease = await this.rateLimiter.AcquireAsync(permitCount: 1, cancellationToken).ConfigureAwait(false);
                if (lease.IsAcquired)
                {
                    return lease;
                }
                var elapsed = SharedStopwatch.Elapsed - begin;
                if (!this.TryUntilSucceeded || elapsed >= this.Timeout)
                {
                    return lease;
                }
                var hasRetry = lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter);
                if (retryAfter == TimeSpan.Zero)
                {
                    retryAfter = NetDefaults.WaitTimeBeforeReattempt;
                }
                if (elapsed + retryAfter >= this.Timeout)
                {
                    return lease;
                }
                lease.Dispose();
                await Task.Delay(retryAfter).ConfigureAwait(false);
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.rateLimiter?.Dispose();
        }
    }
}