using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SubhadraSolutions.Utils.Json.Mvc;

public class ConfigureMvcJsonOptions : IConfigureOptions<MvcOptions>
{
    private readonly IOptionsMonitor<JsonOptions> _jsonOptions;
    private readonly string _jsonSettingsName;
    private readonly ILoggerFactory _loggerFactory;

    public ConfigureMvcJsonOptions(
        string jsonSettingsName,
        IOptionsMonitor<JsonOptions> jsonOptions,
        ILoggerFactory loggerFactory)
    {
        this._jsonSettingsName = jsonSettingsName;
        this._jsonOptions = jsonOptions;
        this._loggerFactory = loggerFactory;
    }

    public void Configure(MvcOptions options)
    {
        var jsonOptions = this._jsonOptions.Get(this._jsonSettingsName);
        var logger = this._loggerFactory.CreateLogger<SpecificSystemTextJsonInputFormatter>();
        options.InputFormatters.Insert(0,
            new SpecificSystemTextJsonInputFormatter(this._jsonSettingsName, jsonOptions, logger));
        options.OutputFormatters.Insert(0,
            new SpecificSystemTextJsonOutputFormatter(this._jsonSettingsName, jsonOptions.JsonSerializerOptions));
    }
}