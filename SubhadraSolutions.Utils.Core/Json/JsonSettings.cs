using Newtonsoft.Json;
using Remote.Linq.Newtonsoft.Json;
using Remote.Linq.Text.Json;
using SubhadraSolutions.Utils.Json.Converters;
using System.Text.Json;

namespace SubhadraSolutions.Utils.Json;

public static class JsonSettings
{
    public static readonly JsonSerializerSettings RestSerializerSettings;
    public static readonly JsonSerializerSettings RestSerializerSettingsForDebug;
    public static readonly JsonSerializerSettings LinqSerializerSettings;
    public static readonly JsonSerializerOptions RestJsonSerializerOptions;
    public static readonly JsonSerializerOptions LinqJsonSerializerOptions;
    public const int MaxDepth = 128;

    static JsonSettings()
    {
        RestSerializerSettings = new JsonSerializerSettings
        {
            //ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            //PreserveReferencesHandling = PreserveReferencesHandling.All,
            //NullValueHandling = NullValueHandling.Ignore,
            //DateTimeZoneHandling=DateTimeZoneHandling.RoundtripKind
            MaxDepth = MaxDepth,
            DateFormatString = "o"
        }.AddConverters();
        RestSerializerSettingsForDebug = new JsonSerializerSettings
        {
            MaxDepth = MaxDepth,
            Formatting = Formatting.Indented,
        }.AddConverters(true);

        LinqSerializerSettings = new JsonSerializerSettings
        {
            //ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            //PreserveReferencesHandling = PreserveReferencesHandling.All,
            //NullValueHandling = NullValueHandling.Ignore,
            //DateTimeZoneHandling=DateTimeZoneHandling.RoundtripKind
            MaxDepth = MaxDepth,
            Formatting = Formatting.Indented,
        }.ConfigureRemoteLinq();
        //linqSerializerSettings = linqSerializerSettings.AddConverters();

        RestJsonSerializerOptions = new JsonSerializerOptions()
        {
            MaxDepth = MaxDepth,
        }.AddConverters();

        LinqJsonSerializerOptions = new JsonSerializerOptions()
        {
            MaxDepth = MaxDepth
        }.ConfigureRemoteLinq();
    }

    public static JsonSerializerOptions AddConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(FloatConverter.Instance);
        options.Converters.Add(DoubleConverter.Instance);
        options.Converters.Add(TimeZoneInfoConverter.Instance);
        options.Converters.Add(ExceptionConverter.Instance);
        options.Converters.Add(DateTimeConverter.Instance);

        return options;
    }

    public static JsonSerializerSettings AddConverters(this JsonSerializerSettings settings, bool forDebug = false)
    {
        settings.Converters.Add(NewtonsoftDoubleConverter.Instance);
        settings.Converters.Add(NewtonsoftFloatConverter.Instance);
        settings.Converters.Add(NewtonsoftTimeZoneInfoConverter.Instance);
        settings.Converters.Add(NewtonsoftExceptionConverter.Instance);
        settings.Converters.Add(NewtonsoftDateTimeConverter.Instance);
        if (forDebug)
        {
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        return settings;
    }
}