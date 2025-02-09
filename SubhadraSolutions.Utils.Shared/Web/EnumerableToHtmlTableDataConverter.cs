using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.EventProcessing;
using SubhadraSolutions.Utils.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SubhadraSolutions.Utils.Web;

public class EnumerableToHtmlTableDataConverter : IDataConverter<string>
{
    private static readonly MethodInfo GetHtmlGenericMethod =
        typeof(EnumerableToHtmlTableDataConverter).GetMethod(nameof(GetHtml),
            BindingFlags.NonPublic | BindingFlags.Instance);

    public static string DefaultBasePath { get; set; }
    public string BasePath { get; set; }
    public bool WriteDocumentRoot { get; set; }

    public string Convert(object input)
    {
        if (input is IEvent evt)
        {
            input = evt.PayloadObject;
        }

        var type = input.GetType().GetEnumerableItemType();
        var content = GetHtmlGenericMethod.MakeGenericMethod(type).Invoke(this, [input]);
        return (string)content;
    }

    private string GetHtml<T>(IEnumerable<T> data)
    {
        using var sw = new StringWriter();

        HtmlExportHelper.ExportAsHtml(data, sw, BasePath ?? DefaultBasePath, WriteDocumentRoot);
        return sw.ToString();
    }
}