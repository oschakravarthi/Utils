using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubhadraSolutions.Utils.ApplicationInsights.AspNetCore.Config;
using System;

namespace SubhadraSolutions.Utils.Exposition.REST;

public static class HostingHelper
{
    public static WebApplication AddRestExposition(this WebApplication app, Func<object, bool> isResponceAnErrorFunc = null)
    {
        var expositionLookup = app.Services.GetRequiredService<IExpositionLookup>();
        var productInfo = app.Services.GetRequiredService<ProductInfo>();
        var configuration = app.Services.GetService<IConfiguration>();

        var httpTelemetryConfig = configuration.GetSection("ApplicationInsightsConfig:Http").Get<HttpTelemetryConfig>();
        var restExposer = new RestExposer(expositionLookup, productInfo, httpTelemetryConfig, isResponceAnErrorFunc);
        restExposer.Initialize();
        restExposer.MapRoutes(app);

        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint($"{expositionLookup.ApiBaseUrl}/openapi", $"{productInfo.Name} API {productInfo.Version}");
            o.RoutePrefix = "swagger";
            o.InjectStylesheet("swagger-theme.css");
        });
        return app;
    }
}