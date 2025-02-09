using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Configuration
{
    public static class ConfigurationHelper
    {
        public static IConfigurationRoot MergeConfigurationProviders(params IConfiguration[] configurations)
        {
            return MergeConfigurationProviders(configurations as IEnumerable<IConfiguration>);
        }

        /// <summary>
        /// Merge providers from the given configurations
        /// Using ConfigurationBuilder and adding it to builder.Services (Add/Replace) will ignore the default configuration
        /// This will remove providers like HostJsonFileConfigurationProvider that is required for host.json (Extensions) values to be honored
        /// </summary>
        /// <param name="defaultConfiguration">The default configuration</param>
        /// <param name="userConfiguration">The user configuration</param>
        /// <returns>the merged configuration</returns>
        public static IConfigurationRoot MergeConfigurationProviders(IEnumerable<IConfiguration> configurations)
        {
            var providerList = new List<IConfigurationProvider>();
            foreach (var configuration in configurations)
            {
                var configurationRoot = configuration as IConfigurationRoot;
                if (configurationRoot != null)
                {
                    providerList.AddRange(configurationRoot.Providers);
                }
            }

            return new ConfigurationRoot(providerList);
        }
    }
}