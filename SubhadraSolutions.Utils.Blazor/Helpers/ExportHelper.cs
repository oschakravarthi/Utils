using Microsoft.JSInterop;
using SubhadraSolutions.Utils.OpenXml;
using SubhadraSolutions.Utils.OpenXml.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SubhadraSolutions.Utils.Blazor.Helpers;

public static class ExportHelper
{
    public static void ExportToExcel(object enumerable, string title, IJSRuntime js)
    {
        if (enumerable != null)
        {
            using var ms = new MemoryStream();
            if (string.IsNullOrEmpty(title))
            {
                title = "Data" + GeneralHelper.Identity;
            }

            ExcelHelper.ExportToExcel((IEnumerable)enumerable, ms, title);
            var bytes = ms.ToArray();
            SaveAs(js, title + ".xlsx", bytes);
        }
    }

    public static void ExportToExcel(IJSRuntime js, string name,
        params KeyValuePair<IEnumerable, string>[] dataAndSheetNamePairs)
    {
        var builder = new ExcelBuilder();

        foreach (var kvp in dataAndSheetNamePairs)
        {
            builder.AddWorksheetPart(kvp.Key, kvp.Value);
        }

        using var ms = new MemoryStream();
        builder.Build(ms);
        var bytes = ms.ToArray();

        if (string.IsNullOrEmpty(name))
        {
            name = "Data" + GeneralHelper.Identity;
        }

        SaveAs(js, name + ".xlsx", bytes);
    }

    public static System.Threading.Tasks.ValueTask<object> SaveAs(IJSRuntime js, string filename, byte[] data)
    {
        return js.InvokeAsync<object>(
            "saveAsFile",
            filename,
            Convert.ToBase64String(data));
    }
}