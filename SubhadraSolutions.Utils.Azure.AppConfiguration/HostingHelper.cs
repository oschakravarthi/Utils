using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubhadraSolutions.Utils.Azure.AppConfiguration.Config;
using SubhadraSolutions.Utils.Configuration;

namespace SubhadraSolutions.Utils.Azure.AppConfiguration
{
    public static class HostingHelper
    {
        public static IHostApplicationBuilder ConfigureAzureAppConfiguration(this IHostApplicationBuilder builder, out IConfiguration configuration)
        {
            configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration == null)
            {
                return builder;
            }
            var azureAppConfig = configuration.GetSection(AzureAppConfig.DefaultSectionName).Get<AzureAppConfig>();
            if (azureAppConfig == null)
            {
                return builder;
            }
            return ConfigureAzureAppConfiguration(builder, azureAppConfig, configuration);
        }

        private static IHostApplicationBuilder ConfigureAzureAppConfiguration(this IHostApplicationBuilder builder, AzureAppConfig azureAppConfig)
        {
            IConfiguration defaultConfiguration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();
            if (defaultConfiguration == null)
            {
                return builder;
            }
            return ConfigureAzureAppConfiguration(builder, azureAppConfig, defaultConfiguration);
        }

        private static IHostApplicationBuilder ConfigureAzureAppConfiguration(this IHostApplicationBuilder builder, AzureAppConfig azureAppConfig, IConfiguration configuration)
        {
            if (azureAppConfig?.IsValid() == true)
            {
                try
                {
                    var azureAppConfigurationBuilder = builder.Configuration.AddAzureAppConfiguration(azureAppConfig);
                    builder.Services.AddAzureAppConfiguration();

                    var azureAppConfiguration = azureAppConfigurationBuilder.Build();
                    var mergedConfiguration = ConfigurationHelper.MergeConfigurationProviders(configuration, azureAppConfiguration);
                    builder.Services.AddSingleton<IConfiguration>(mergedConfiguration);
                }
                catch (Exception ex)
                {
                    var provider = builder.Services.BuildServiceProvider();

                    var logger = provider.GetService<ILogger>();
                    logger?.LogError(ex, $"{nameof(ConfigureAzureAppConfiguration)} Failed");
                }
            }

            return builder;
        }

        /// <summary>
        /// Connects to azure app config using endpoint
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder</param>
        /// <param name="azureConfiguration">The Azure Configuration</param>
        /// <returns>The Configuration Builder</returns>
        private static IConfigurationBuilder AddAzureAppConfiguration(this IConfigurationBuilder configurationBuilder, AzureAppConfig azureAppConfig)
        {
            if (!azureAppConfig.IsValid())
            {
                throw new Exception("Data required for connecting to Azure Configuration is not available");
            }

            // This will use managed identity to connect to azure app config and
            /// retrieving key vault referenced data too.
            var credential = new DefaultAzureCredential();

            var configuration = configurationBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(azureAppConfig.Endpoint), credential)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(credential);
                })
                .Select(keyFilter: KeyFilter.Any, labelFilter: azureAppConfig.DefaultLabel)
                .Select(keyFilter: KeyFilter.Any, labelFilter: azureAppConfig.Label)
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register(key: azureAppConfig.ReloadKey, label: azureAppConfig.Label, refreshAll: true)
                            .SetCacheExpiration(cacheExpiration: TimeSpan.FromSeconds(azureAppConfig.RefreshTimeInSeconds));
                });

                var refresher = options.GetRefresher();
            });

            return configuration;
        }
    }
}