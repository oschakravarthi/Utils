using Microsoft.Extensions.DependencyInjection;
using Remote.Linq.Text.Json;

namespace SubhadraSolutions.Utils.Json.Mvc;

public static class HostingHelper
{
    public const string NameForReferenced = "REFERENCED";

    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson()
            .AddJsonOptions(o =>
            {
                //var settings = o.SerializerSettings;
                //settings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                //// dont mess with case of properties
                //var resolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
                //resolver.NamingStrategy = null;

                o.JsonSerializerOptions.AddConverters();
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
                o.JsonSerializerOptions.IgnoreReadOnlyProperties = false;
                //o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                //o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            })
            .AddJsonOptions(NameForReferenced, o =>
            {
                //var settings = o.SerializerSettings;
                //settings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                //// dont mess with case of properties
                //var resolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
                //resolver.NamingStrategy = null;

                o.JsonSerializerOptions.AddConverters();
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
                o.JsonSerializerOptions.IgnoreReadOnlyProperties = false;
                o.JsonSerializerOptions.ConfigureRemoteLinq().MaxDepth = 128;
                //o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

        return services;
    }
}