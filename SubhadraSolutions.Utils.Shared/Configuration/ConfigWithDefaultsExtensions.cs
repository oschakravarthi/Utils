using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Diagnostics.Tracing;
using System.Collections.Specialized;

namespace SubhadraSolutions.Utils.Configuration;

public static class ConfigWithDefaultsExtensions
{
    public static bool GetConfigOrDefault(this NameValueCollection ConfigurationManagerMember, string setting,
        string label, ConfigWithDefaults defaults, out string newValue, string nameFn = null)
    {
        nameFn ??= ActiveCode.Parent.Name;
        return GetConfigOrDefault_helper(setting, label, ConfigurationManagerMember[setting], defaults, nameFn,
            out newValue);
    }

    private static bool GetConfigOrDefault_helper(string setting, string label, string configValue,
        ConfigWithDefaults defaults, string nameFn, out string newValue)
    {
        var result = true;
        if (string.IsNullOrEmpty(configValue))
        {
            if (string.IsNullOrEmpty(label))
            {
                label = "\"" + setting + "\" setting";
            }

            TraceLogger.TraceInformation("{0}: Config: {1} missing in \"{2}\" file; using value from defaults.", nameFn,
                label, "{{App, Web}}.config");
            if (defaults.TryGetValue(setting, out var @default))
            {
                if (@default != null)
                {
                    configValue = @default.Default;
                }

                result = false;
            }
        }

        newValue = configValue;
        return result;
    }
}