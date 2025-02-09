using Microsoft.Extensions.Configuration;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Linq;
using System.Threading.RateLimiting;

namespace SubhadraSolutions.Utils.Net.Http.Configs
{
    public class RateLimiterConfig
    {
        public string Type { get; set; }
        public IConfigurationSection Options { get; set; }

        public static RateLimiter BuildRateLimiter(RateLimiterConfig config)
        {
            if (config == null || string.IsNullOrWhiteSpace(config.Type))
            {
                return null;
            }
            var type = CoreReflectionHelper.GetType(config.Type);
            if (type == null)
            {
                return null;
            }
            var constructors = type.GetConstructors();
            var constructor = constructors.FirstOrDefault(x => x.GetParameters().Length == 1);
            if (constructor == null)
            {
                constructor = constructors.FirstOrDefault(x => x.GetParameters().Length == 0);
                if (constructor == null)
                {
                    return null;
                }
                return (RateLimiter)Activator.CreateInstance(type);
            }
            var optionsParameter = constructor.GetParameters()[0];
            var options = config.Options.Get(optionsParameter.ParameterType);
            return (RateLimiter)Activator.CreateInstance(type, options);
        }
    }
}