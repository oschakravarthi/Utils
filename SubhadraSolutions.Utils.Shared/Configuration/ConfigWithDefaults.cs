using System.Collections.Generic;
using System.Diagnostics;

namespace SubhadraSolutions.Utils.Configuration;

public class ConfigWithDefaults : Dictionary<string, ConfigWithDefaults.ConfigWithDefaultsEntry>
{
    public void Add(string name, string defaultValue, string label)
    {
        Add(name, new ConfigWithDefaultsEntry
        {
            Name = name,
            Default = defaultValue,
            Label = label
        });
    }

    //public string GetConfigOrDefault(string name, string nameFn = null)
    //{
    //    string newValue = string.Empty;
    //    if (string.IsNullOrEmpty(nameFn))
    //    {
    //        nameFn = ActiveCode.Parent.Name;
    //    }
    //    if (string.IsNullOrEmpty(name))
    //    {
    //        throw new ArgumentNullException("name");
    //    }
    //    if (!ContainsKey(name))
    //    {
    //        throw new ArgumentOutOfRangeException("name", "Unknown config value \"" + name + "\"!");
    //    }
    //    ConfigurationManager.AppSettings.GetConfigOrDefault(name, base[name].Label, this, out newValue, nameFn);
    //    return newValue;
    //}

    [DebuggerDisplay("{Name}: {Default} ({Label})")]
    public class ConfigWithDefaultsEntry
    {
        public string Default;
        public string Label;
        public string Name;

        public string GetKey()
        {
            return Name;
        }
    }
}