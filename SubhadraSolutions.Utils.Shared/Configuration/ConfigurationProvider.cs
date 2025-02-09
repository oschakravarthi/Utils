using Microsoft.Extensions.Configuration;
using SubhadraSolutions.Utils.Exposition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Configuration;

public class ConfigurationProvider
{
    private readonly IConfiguration configuration;
    private readonly Func<string, bool> keyPredicate;

    public ConfigurationProvider(IConfiguration configuration, Func<string, bool> keyPredicate)
    {
        this.configuration = configuration;
        this.keyPredicate = keyPredicate;
    }

    [Expose]
    public List<KeyValuePair<string, string>> GetConfiguration()
    {
        var kvps = configuration.AsEnumerable().Where(x => x.Value != null);
        if (this.keyPredicate != null)
        {
            kvps = kvps.Where(x => keyPredicate(x.Key));
        }
        return kvps.OrderBy(x => x.Key).ToList();
    }
}