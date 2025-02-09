using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Diagnostics.Tracing;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SubhadraSolutions.Utils.Configuration;

public static class ConfigurationManagerExtensions
{
    public static bool GetConfigOrDefault(this NameValueCollection ConfigurationManagerMember, string setting,
        string label, Dictionary<string, string> defaults, out string newValue, string nameFn = null)
    {
        nameFn ??= ActiveCode.Parent.Name;
        return GetConfigOrDefault_helper(setting, label, ConfigurationManagerMember[setting], defaults, nameFn,
            out newValue);
    }

    private static bool GetConfigOrDefault_helper<TValue>(string setting, string label, TValue configValue,
        Dictionary<string, TValue> defaults, string nameFn, out TValue newValue)
    {
        var result = true;
        if (configValue == null || configValue.Equals(default(TValue)))
        {
            label ??= "\"" + setting + "\" setting";
            TraceLogger.TraceInformation("{0}: Config: {1} missing in \"{2}\" file; using value from defaults.", nameFn,
                label, "{{App, Web}}.config");
            defaults.TryGetValue(setting, out configValue);
            result = false;
        }

        newValue = configValue;
        return result;
    }
}