using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace SubhadraSolutions.Utils.Json.Mvc;

public class SpecificSystemTextJsonInputFormatter(string settingsName, JsonOptions options,
        ILogger<SpecificSystemTextJsonInputFormatter> logger)
    : SystemTextJsonInputFormatter(options, logger)
{
    public string SettingsName { get; } = settingsName;

    public override bool CanRead(InputFormatterContext context)
    {
        if (context.HttpContext.GetJsonSettingsName() != SettingsName)
        {
            return false;
        }

        return base.CanRead(context);
    }
}