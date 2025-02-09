using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Docs.Extensions;
using MudBlazor.Services;
using SubhadraSolutions.Utils.ApiClient.Logging;
using SubhadraSolutions.Utils.Logging;
using SubhadraSolutions.Utils.Net.Http;

namespace SubhadraSolutions.Utils.Blazor;

public static class HostingHelper
{
    public static WebAssemblyHostBuilder ConfigureWebAssemblyHostBuilderBasics(this WebAssemblyHostBuilder builder)
    {
        var services = builder.Services;
        services.AddSingleton<IConfiguration>(builder.Configuration);

        services.AddMudServices();

        services.TryAddDocsViewServices();

        return builder;
    }

    public static WebAssemblyHostBuilder ConfigureExceptionHandling(this WebAssemblyHostBuilder builder, string publishingApiPath)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var httpClient = serviceProvider.GetRequiredService<IHttpClient>();

        var logItemWriter = new ItemWriter<LogItem>(httpClient, publishingApiPath);
        builder.Services.AddSingleton<IItemWriter<LogItem>>(logItemWriter);

        var productInfo = serviceProvider.GetRequiredService<ProductInfo>();
        var logger = new SmartLogger(logItemWriter, productInfo, LogLevel.Critical, LogLevel.Error);
        builder.Services.AddSingleton<ILogger>(logger);
        builder.Services.AddSingleton<ISmartLogger>(logger);

        var loggerProvider = new LoggerProvider(logger);
        builder.Logging.AddProvider(loggerProvider);

        return builder;
    }
}