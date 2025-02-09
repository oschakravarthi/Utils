using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.HeaderPropagation;

public class HeaderPropagationMessageHandler(HeaderPropagationOptions options, IHttpContextAccessor contextAccessor)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (contextAccessor.HttpContext != null)
        {
            foreach (var headerName in options.HeaderNames)
            {
                // Get the incoming header value
                var headerValue = contextAccessor.HttpContext.Request.Headers[headerName];
                if (StringValues.IsNullOrEmpty(headerValue))
                {
                    continue;
                }

                request.Headers.TryAddWithoutValidation(headerName, (string[])headerValue);
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}