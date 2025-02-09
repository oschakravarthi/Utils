using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;

namespace SubhadraSolutions.Utils.Json.Mvc;

public class SpecificSystemTextJsonOutputFormatter(string settingsName, JsonSerializerOptions jsonSerializerOptions)
    : SystemTextJsonOutputFormatter(jsonSerializerOptions)
{
    public string SettingsName { get; } = settingsName;

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        if (context.HttpContext.GetJsonSettingsName() != SettingsName)
        {
            return false;
        }

        return base.CanWriteResult(context);
    }
}