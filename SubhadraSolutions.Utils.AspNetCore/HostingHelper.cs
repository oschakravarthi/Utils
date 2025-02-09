using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubhadraSolutions.Utils.Exposition;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Logging;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System;
using System.Net;

namespace SubhadraSolutions.Utils.AspNetCore;

public static class HostingHelper
{
    public static IHostApplicationBuilder ConfigureWebApplicationBuilderBasics(this IHostApplicationBuilder builder)
    {
        //builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

        //services.Configure<GzipCompressionProviderOptions>(options =>
        //{
        //    options.Level = CompressionLevel.SmallestSize;
        //});
        //services.Configure<BrotliCompressionProviderOptions>(options =>
        //{
        //    options.Level = CompressionLevel.Fastest;
        //});
        //services.AddResponseCompression(options =>
        //{
        //    options.EnableForHttps = true;
        //    options.Providers.Add<BrotliCompressionProvider>();
        //    options.Providers.Add<GzipCompressionProvider>();
        //});

        builder.Configuration.AddEnvironmentVariables();

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        return builder;
    }

    public static WebApplication ConfigureWebApplicationBasics(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        if (System.Diagnostics.Debugger.IsAttached)
        {
            app.UseWebAssemblyDebugging();
        }
        else
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        {
            app.UseHsts();
        }

        var configurationRefresherProvider = app.Services.GetService<IConfigurationRefresherProvider>();
        if (configurationRefresherProvider != null)
        {
            app.UseAzureAppConfiguration();
        }
        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        return app;
    }

    public static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
    {
        ServicePointManager.DefaultConnectionLimit = int.MaxValue;

        services.AddSingleton(services);

        services.AddHttpContextAccessor();

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var productInfo = configuration.GetSection(ProductInfo.DefaultSectionName).Get<ProductInfo>();
        productInfo.IsServer = true;
        services.AddSingleton(productInfo);

        var companyInfo = configuration.GetSection(CompanyInfo.DefaultSectionName).Get<CompanyInfo>();
        services.AddSingleton(companyInfo);

        //var memoryCacheOptions = new MemoryCacheOptions
        //{
        //    SizeLimit = (long)Math.Pow(1024, 3),
        //};
        //var memoryCache = new MemoryCache(memoryCacheOptions);
        //services.AddSingleton<IMemoryCache>(memoryCache);

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        services.AddSingleton(loggerFactory);
        //services.AddLogging(loggerBuilder =>
        //{
        //    loggerBuilder.ClearProviders();
        //    loggerBuilder.AddConsole();
        //});
        //
        //expositionLookup.EnsureNoConflictsInApis();
        //Host.ConfigureServices(services);

        return services;
    }

    public static IServiceCollection ConfigureExposition(this IServiceCollection services, string restApiPath, bool enableMetrics, out IExpositionLookup expositionLookup)
    {
        var serviceProvider = services.BuildServiceProvider();
        var productInfo = serviceProvider.GetRequiredService<ProductInfo>();
        expositionLookup = null;
        if (enableMetrics)
        {
            var telemetryClient = serviceProvider.GetService<TelemetryClient>();
            expositionLookup = new MetricsSupportedExpositionLookup(restApiPath, telemetryClient);
        }
        if (expositionLookup == null)
        {
            expositionLookup = new ExpositionLookup(restApiPath);
        }

        services.AddSingleton(expositionLookup);
        expositionLookup.RegisterObject("common/productinfo", productInfo);

        var compantInfo = serviceProvider.GetRequiredService<CompanyInfo>();
        expositionLookup.RegisterObject("common/companyinfo", compantInfo);

        var queryableLookup = new QueryableLookupImpl();
        services.AddSingleton<IQueryableLookup>(queryableLookup);

        return services;
    }

    public static IServiceCollection ExposeConfiguration(this IServiceCollection services, string restApiRelativePath = "common/config", Func<string, bool> keyPredicate = null)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var expositionLookup = serviceProvider.GetRequiredService<IExpositionLookup>();

        var configurationReader = new Configuration.ConfigurationProvider(configuration, keyPredicate);
        expositionLookup.RegisterMethods(restApiRelativePath, configurationReader);
        return services;
    }

    public static IServiceCollection EnableMetricsTracking(this IServiceCollection services, string restApiRelativePath = "common/metrics")
    {
        var serviceProvider = services.BuildServiceProvider();
        var expositionLookup = serviceProvider.GetRequiredService<IExpositionLookup>();

        MetricsTracker.Instance.Enabled = true;
        expositionLookup.RegisterMethods(restApiRelativePath, MetricsTracker.Instance);

        return services;
    }

    public static IServiceCollection ConfigureExceptionHandling(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logItemWriter = serviceProvider.GetService<IItemWriter<LogItem>>();
        if (logItemWriter == null)
        {
            logItemWriter = new ConsoleItemWriter<LogItem>();
            services.AddSingleton(logItemWriter);
        }

        var productInfo = serviceProvider.GetRequiredService<ProductInfo>();
        var logger = new SmartLogger(logItemWriter, productInfo, LogLevel.Critical, LogLevel.Error);
        services.AddSingleton<ILogger>(logger);
        services.AddSingleton<ISmartLogger>(logger);

        var loggerProvider = new LoggerProvider(logger);
        services.AddSingleton<ILoggerProvider>(loggerProvider);

        var expositionLookup = serviceProvider.GetService<IExpositionLookup>();
        expositionLookup?.RegisterMethods("log", logItemWriter);

        return services;
    }
}