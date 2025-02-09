using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubhadraSolutions.Utils.ApplicationInsights.AspNetCore.Config;

namespace SubhadraSolutions.Utils.ApplicationInsights.AspNetCore
{
    public static class HostingHelper
    {
        public static IServiceCollection EnableTelemetry(this IServiceCollection services)
        {
            var telemetryClient = InitializeTelemetryClient(services);

            if (telemetryClient != null)
            {
                services.AddSingleton(telemetryClient);
            }
            return services;
        }

        public static TelemetryClient InitializeTelemetryClient(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var applicationInsightsServiceOptionsConfig = configuration.GetSection(ApplicationInsightsServiceOptionsConfig.DefaultSectionName).Get<ApplicationInsightsServiceOptionsConfig>();
            return InitializeTelemetryClient(services, applicationInsightsServiceOptionsConfig);
        }

        public static TelemetryClient InitializeTelemetryClient(this IServiceCollection services, ApplicationInsightsServiceOptionsConfig applicationInsightsServiceOptionsConfig)
        {
            if (applicationInsightsServiceOptionsConfig == null)
            {
                return null;
            }
            var connectionString = applicationInsightsServiceOptionsConfig.GetConnectionString();
            if (connectionString == null)
            {
                return null;
            }
            var applicationInsightsServiceOptions =
                BuildApplicationInsightsServiceOptions(applicationInsightsServiceOptionsConfig);
            services.AddApplicationInsightsTelemetry(applicationInsightsServiceOptions);

            var serviceProvider = services.BuildServiceProvider();
            var telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();

            if (applicationInsightsServiceOptionsConfig.PublishMetricsTrackerMetrics)
            {
                var metricsPublisher = new MetricsPublisher(telemetryClient);
            }

            return telemetryClient;
        }

        private static ApplicationInsightsServiceOptions BuildApplicationInsightsServiceOptions(
            ApplicationInsightsServiceOptionsConfig config)
        {
            var connectionString = config.GetConnectionString();
            var result = new ApplicationInsightsServiceOptions
            {
                EnableQuickPulseMetricStream = config.EnableQuickPulseMetricStream,
                EnablePerformanceCounterCollectionModule = config.EnablePerformanceCounterCollectionModule,
                EnableAppServicesHeartbeatTelemetryModule = config.EnableAppServicesHeartbeatTelemetryModule,
                EnableAzureInstanceMetadataTelemetryModule = config.EnableAzureInstanceMetadataTelemetryModule,
                EnableDependencyTrackingTelemetryModule = config.EnableDependencyTrackingTelemetryModule,
                EnableEventCounterCollectionModule = config.EnableEventCounterCollectionModule,
                EnableAdaptiveSampling = config.EnableAdaptiveSampling,
                ApplicationVersion = config.ApplicationVersion,
                DeveloperMode = config.DeveloperMode,
                EndpointAddress = config.EndpointAddress,
                EnableDebugLogger = config.EnableDebugLogger,
                EnableHeartbeat = config.EnableHeartbeat,
                AddAutoCollectedMetricExtractor = config.AddAutoCollectedMetricExtractor,
                EnableDiagnosticsTelemetryModule = config.EnableDiagnosticsTelemetryModule,
                ConnectionString = connectionString.SecureStringToString()
            };
            return result;
        }
    }
}