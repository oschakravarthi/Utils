using Microsoft.AspNetCore.Http;

namespace SubhadraSolutions.Utils.Json.Mvc;

internal static class HttpContextExtensions
{
    public static string GetJsonSettingsName(this HttpContext context)
    {
        return context.GetEndpoint()?.Metadata.GetMetadata<JsonSettingsNameAttribute>()?.Name;
    }
}