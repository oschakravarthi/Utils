using SubhadraSolutions.Utils.Diagnostics;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http.Handlers
{
    public class RetryHandler : DelegatingHandler
    {
        private readonly Func<HttpResponseMessage, bool> retryFunc;
        private readonly TimeSpan timeout;

        public RetryHandler(int[] statusCodes, TimeSpan timeout)
            : this(statusCodes, timeout, new HttpClientHandler())
        {
        }

        public RetryHandler(int[] statusCodes, TimeSpan timeout, HttpMessageHandler httpMessageHandler)
            : this(timeout, (r) => statusCodes.Contains((int)r.StatusCode), httpMessageHandler)
        {
        }

        public RetryHandler(TimeSpan timeout, Func<HttpResponseMessage, bool> retryFunc)
            : this(timeout, retryFunc, new HttpClientHandler())
        {
        }

        public RetryHandler(TimeSpan timeout, Func<HttpResponseMessage, bool> retryFunc, HttpMessageHandler httpMessageHandler)
            : base(httpMessageHandler)
        {
            this.timeout = timeout == TimeSpan.Zero ? NetDefaults.DefaultTimeout : timeout;
            this.retryFunc = retryFunc;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var begin = SharedStopwatch.Elapsed;
            while (true)
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                var statucCode = (int)response.StatusCode;
                if (!this.retryFunc(response))
                {
                    return response;
                }
                var elapsed = SharedStopwatch.Elapsed - begin;
                if (elapsed >= this.timeout)
                {
                    return response;
                }
                await Task.Delay(NetDefaults.WaitTimeBeforeReattempt).ConfigureAwait(false);
            }
        }
    }
}