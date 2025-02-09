using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;

namespace SubhadraSolutions.Utils.Net.HeaderPropagation;

internal class HeaderPropagationMessageHandlerBuilderFilter(IOptions<HeaderPropagationOptions> options,
        IHttpContextAccessor contextAccessor)
    : IHttpMessageHandlerBuilderFilter
{
    private readonly HeaderPropagationOptions _options = options.Value;

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        return builder =>
        {
            builder.AdditionalHandlers.Add(new HeaderPropagationMessageHandler(_options, contextAccessor));
            next(builder);
        };
    }
}