using System;
using System.IO;
using System.Reflection;

namespace SubhadraSolutions.Utils.Reflection;

public class ThisAssembly(Assembly assembly)
{
    public static ThisAssembly Default = Current;

    private static ThisAssembly Instance;
    private readonly Assembly m_assembly = assembly;

    public static ThisAssembly Application
    {
        get
        {
            Instance = new ThisAssembly(Assembly.GetEntryAssembly());
            return Instance;
        }
    }

    public static ThisAssembly Current
    {
        get
        {
            Instance = new ThisAssembly(Assembly.GetCallingAssembly());
            return Instance;
        }
    }

    public string Company => GetFirstAttribute((AssemblyCompanyAttribute item) => item.Company);

    public string Copyright =>
        GetFirstAttribute((AssemblyCopyrightAttribute item) => item.Copyright).Replace("©", "(C)");

    public string Description => GetFirstAttribute((AssemblyDescriptionAttribute item) => item.Description);

    public string InformationalVersion =>
        GetFirstAttribute((AssemblyInformationalVersionAttribute item) => item.InformationalVersion, () => Version);

    public string Name => m_assembly.FullName.Split(',')[0];

    public string Product => GetFirstAttribute((AssemblyProductAttribute item) => item.Product);

    public string Title => GetFirstAttribute((AssemblyTitleAttribute item) => item.Title,
        () => Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

    public string Version => m_assembly.GetName().Version.ToString();

    public static implicit operator Assembly(ThisAssembly pThis)
    {
        Assembly result = null;
        if (pThis != null)
        {
            result = pThis.m_assembly;
        }

        return result;
    }

    private string GetFirstAttribute<TAttribute>(Func<TAttribute, string> getMemberVariable,
        Func<string> getDefaultValue = null)
    {
        var text = string.Empty;
        var customAttributes = m_assembly.GetCustomAttributes(typeof(TAttribute), false);
        if (customAttributes.Length != 0)
        {
            text = getMemberVariable((TAttribute)customAttributes[0]);
        }

        if (string.IsNullOrEmpty(text) && getDefaultValue != null)
        {
            text = getDefaultValue();
        }

        return text;
    }
}