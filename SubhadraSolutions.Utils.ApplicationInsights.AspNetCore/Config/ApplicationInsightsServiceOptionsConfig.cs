using SubhadraSolutions.Utils.Azure.Security;
using System;
using System.Reflection;
using System.Security;

namespace SubhadraSolutions.Utils.ApplicationInsights.AspNetCore.Config;

/// <summary>
///     Application Insights service options defines the custom behavior of the features to add, as opposed to the default
///     selection of features obtained from Application Insights.
/// </summary>
public class ApplicationInsightsServiceOptionsConfig
{
    public const string DefaultSectionName = "ApplicationInsights";

    private readonly object _synclock = new();
    private SecureString _connectionStringSecret;
    private string _connectionStringSecretUri;

    /// <summary>
    ///     Gets or sets a value indicating whether QuickPulseTelemetryModule and QuickPulseTelemetryProcessor are registered
    ///     with the configuration.
    ///     Setting EnableQuickPulseMetricStream to
    ///     <value>false</value>
    ///     , will disable the default quick pulse metric stream. Defaults to
    ///     <value>true</value>
    ///     .
    /// </summary>
    public bool EnableQuickPulseMetricStream { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether PerformanceCollectorModule should be enabled.
    ///     Defaults to
    ///     <value>true</value>
    ///     .
    /// </summary>
    public bool EnablePerformanceCounterCollectionModule { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether AppServicesHeartbeatTelemetryModule should be enabled.
    ///     Defaults to
    ///     <value>true</value>
    ///     .
    ///     IMPORTANT: This setting will be ignored if either <see cref="EnableDiagnosticsTelemetryModule" /> or
    ///     <see cref="EnableHeartbeat" /> are set to false.
    /// </summary>
    public bool EnableAppServicesHeartbeatTelemetryModule { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether AzureInstanceMetadataTelemetryModule should be enabled.
    ///     Defaults to
    ///     <value>true</value>
    ///     .
    ///     IMPORTANT: This setting will be ignored if either <see cref="EnableDiagnosticsTelemetryModule" /> or
    ///     <see cref="EnableHeartbeat" /> are set to false.
    /// </summary>
    public bool EnableAzureInstanceMetadataTelemetryModule { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether DependencyTrackingTelemetryModule should be enabled.
    ///     Defaults to
    ///     <value>true</value>
    ///     .
    /// </summary>
    public bool EnableDependencyTrackingTelemetryModule { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether EventCounterCollectionModule should be enabled.
    ///     Defaults to
    ///     <value>true</value>
    ///     .
    /// </summary>
    public bool EnableEventCounterCollectionModule { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether telemetry processor that controls sampling is added to the service.
    ///     Setting EnableAdaptiveSampling to
    ///     <value>false</value>
    ///     , will disable the default adaptive sampling feature. Defaults to
    ///     <value>true</value>
    ///     .
    /// </summary>
    public bool EnableAdaptiveSampling { get; set; } = true;

    /// <summary>
    ///     Gets or sets the connection string for the application.
    /// </summary>
    public SecureString GetConnectionString()
    {
        if (_connectionStringSecret == null)
        {
            lock (_synclock)
            {
                if (_connectionStringSecret == null)
                {
                    var uri = ConnectionStringSecretUri;
                    if (!string.IsNullOrWhiteSpace(uri))
                    {
                        try
                        {
                            _connectionStringSecret = KeyVaultHelper.GetSecretValueAsync(ConnectionStringSecretUri).Result;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                        }
                    }
                    if (_connectionStringSecret == null)
                    {
                        var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
                        if (connectionString != null)
                        {
                            _connectionStringSecret = connectionString.ConvertToSecureString();
                        }
                    }
                }
            }
        }

        return _connectionStringSecret;
    }

    /// <summary>
    ///     Gets or sets the connection string secret URI for the application.
    /// </summary>
    public string ConnectionStringSecretUri
    {
        get => _connectionStringSecretUri;
        set
        {
            if (_connectionStringSecretUri != value)
            {
                _connectionStringSecretUri = value;
                _connectionStringSecret = null;
            }
        }
    }

    /// <summary>
    ///     Gets or sets the application version reported with telemetries.
    /// </summary>
    public string ApplicationVersion { get; set; } = Assembly.GetEntryAssembly()?.GetName().Version.ToString();

    /// <summary>
    ///     Gets or sets a value indicating whether telemetry channel should be set to developer mode.
    /// </summary>
    public bool? DeveloperMode { get; set; }

    /// <summary>
    ///     Gets or sets the endpoint address of the channel.
    /// </summary>
    public string EndpointAddress { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether a logger would be registered automatically in debug mode.
    /// </summary>
    public bool EnableDebugLogger { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether heartbeats are enabled.
    ///     IMPORTANT: This setting will be ignored if <see cref="EnableDiagnosticsTelemetryModule" /> is set to false.
    ///     IMPORTANT: Disabling this will cause the following settings to be ignored:
    ///     <see cref="EnableAzureInstanceMetadataTelemetryModule" />.
    ///     <see cref="EnableAppServicesHeartbeatTelemetryModule" />.
    /// </summary>
    public bool EnableHeartbeat { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether AutoCollectedMetricExtractors are added or not.
    ///     Defaults to
    ///     <value>true</value>
    ///     .
    /// </summary>
    public bool AddAutoCollectedMetricExtractor { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the
    ///     <see cref="Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing.DiagnosticsTelemetryModule" /> should
    ///     be enabled.
    ///     IMPORTANT: Disabling this will cause the following settings to be ignored:
    ///     <see cref="EnableHeartbeat" />.
    ///     <see cref="EnableAzureInstanceMetadataTelemetryModule" />.
    ///     <see cref="EnableAppServicesHeartbeatTelemetryModule" />.
    /// </summary>
    public bool EnableDiagnosticsTelemetryModule { get; set; } = true;

    /// <summary>
    ///     Publish Metrics Tracker Metrics
    /// </summary>
    public bool PublishMetricsTrackerMetrics { get; set; }

    public HttpTelemetryConfig Http { get; set; }
}