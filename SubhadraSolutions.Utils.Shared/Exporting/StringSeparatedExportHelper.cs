using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.IO;

namespace SubhadraSolutions.Utils.Exporting;

public static class StringSeparatedExportHelper<T>
{
    private static readonly string[] columnNames;
    private static readonly Action<T, string[]> populator;

    static StringSeparatedExportHelper()
    {
        var type = typeof(T);
        var properties = type.GetPublicProperties(true);
        populator = PropertiesToStringValuesHelper.BuildActionForToStringArray<T>(properties);
        columnNames = new string[properties.Count];
        for (var i = 0; i < properties.Count; i++) columnNames[i] = properties[i].GetPropertyColumnName();
    }

    public static void Export(IEnumerable<T> objects, TextWriter writer, string separator, bool writeHeaders)
    {
        var numberOfColumns = columnNames.Length;
        if (writeHeaders)
        {
            for (var i = 0; i < numberOfColumns; i++)
            {
                if (i > 0)
                {
                    writer.Write(separator);
                }

                writer.Write(columnNames[i]);
            }

            writer.WriteLine();
        }

        var values = new string[numberOfColumns];
        foreach (var obj in objects)
        {
            populator(obj, values);
            for (var i = 0; i < numberOfColumns; i++)
            {
                if (i > 0)
                {
                    writer.Write(separator);
                }

                writer.Write(values[i]);
            }

            writer.WriteLine();
        }
    }
}