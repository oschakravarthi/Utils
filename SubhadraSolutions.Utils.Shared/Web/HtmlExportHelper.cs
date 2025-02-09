using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.Web;

public static class HtmlExportHelper
{
    public static void ExportAsHtml<T>(IEnumerable<T> items, string outputFileName, string basePath, bool writeDocumentRoot)
    {
        using (var sw = new StreamWriter(outputFileName))
        {
            ExportAsHtml(items, sw, basePath, writeDocumentRoot);
        }
    }

    public static void ExportAsHtml<T>(IEnumerable<T> items, TextWriter writer, string basePath, bool writeDocumentRoot)
    {
        if (writeDocumentRoot)
        {
            writer.WriteRaw("<html>");
            writer.WritePageHeader(null);
            writer.WriteRaw("<body>");
        }

        var type = typeof(T);
        AttributesLookup attributesLookup = null;
        if (items is IQueryable queryable)
        {
            var toSeed = LinqHelper.ExploreToSeed(queryable);
            attributesLookup = new AttributesLookup(toSeed);
        }

        var properties = type.GetSortedPublicProperties(attributesLookup);
        var propertiesCount = properties.Count;
        var propertyValuesLookupIndex = new int[propertiesCount];

        var nonListProperties = new List<PropertyInfo>();

        var linksLookup = new List<NavigationAttribute>[propertiesCount];
        var isListPropertyLookup = new bool[propertiesCount];
        var propertyToArrayFuncs = new Func<T, IEnumerable<string>>[propertiesCount];
        var isNumericLookup = new bool[propertiesCount];
        for (int i = 0, j = 0; i < propertiesCount; i++)
        {
            var property = properties[i];
            var propertyType = property.PropertyType;
            isNumericLookup[i] = propertyType.IsNumericType();
            var isPropertyTypeEnumerable = !propertyType.IsPrimitiveOrExtendedPrimitive() && propertyType.IsEnumerableType();

            if (!isPropertyTypeEnumerable)
            {
                propertyValuesLookupIndex[i] = j;
                nonListProperties.Add(property);
                j++;
            }
            else
            {
                propertyToArrayFuncs[i] =
                    PropertiesToStringValuesHelper.BuildGetValueAsStringEnumerableFunc<T>(property);
            }

            isListPropertyLookup[i] = isPropertyTypeEnumerable;

            var attribs = property.GetCustomAttributes<NavigationAttribute>().ToList();

            if (attribs.Count > 0)
            {
                attribs.Sort(NavigationAttributeComparer.Instance);
                linksLookup[i] = attribs;
            }
        }

        var action = PropertiesToStringValuesHelper.BuildActionForToStringArray<T>(nonListProperties);
        var stringValues = new string[propertiesCount];

        writer.WriteTableBegin();
        WriteTableHeaderRow(properties, writer);
        writer.WriteTableBodyBegin();
        foreach (var item in items)
        {
            action(item, stringValues);
            writer.WriteTableRowBegin();

            for (var i = 0; i < propertiesCount; i++)
            {
                var links = linksLookup[i];
                writer.WriteTableCellBegin(isNumericLookup[i]);

                if (isListPropertyLookup[i])
                {
                    var propertyValueAsStrings = propertyToArrayFuncs[i](item);
                    var j = 0;

                    foreach (var stringValue in propertyValueAsStrings)
                    {
                        if (links == null || string.IsNullOrEmpty(stringValue))
                        {
                            //AddTextToParagraph(p, stringValue);
                        }
                        else
                        {
                            AddNavigations(stringValue, links, writer, basePath);
                        }

                        if (j > 0)
                        {
                            writer.WriteSpace();
                        }

                        j++;
                    }
                }
                else
                {
                    var stringValue = stringValues[propertyValuesLookupIndex[i]];

                    if (links == null || string.IsNullOrEmpty(stringValue))
                    {
                        writer.WriteTextContent(stringValue);
                    }
                    else
                    {
                        AddNavigations(stringValue, links, writer, basePath);
                    }
                }

                writer.WriteTableCellEnd();
            }

            writer.WriteTableRowEnd();
        }

        writer.WriteTableBodyEnd();
        writer.WriteTableEnd();

        if (writeDocumentRoot)
        {
            writer.WriteRaw("</body>");
            writer.WriteRaw("</html>");
        }
    }
    private static void AddNavigations(string stringValue, List<NavigationAttribute> links, TextWriter writer,
        string basePath)
    {
        for (var j = 0; j < links.Count; j++)
        {
            var linkPath = string.Format(links[j].LinkTemplate, stringValue);

            if (!linkPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                linkPath = basePath.TrimEnd('/') + "/" + linkPath.TrimStart('/');
            }

            if (j > 0)
            {
                //rp.Bold = new();
            }

            string text;

            if (j == 0)
            {
                text = stringValue;
            }
            else
            {
                text = links[j].Name;

                if (string.IsNullOrEmpty(text))
                {
                    text = "Link" + j;
                }

                text = "[" + text + "]";
            }

            writer.WriteLink(linkPath, text);
            if (j < links.Count - 1)
            {
                writer.WriteSpace();
            }
        }
    }

    private static void WriteTableHeaderRow(IEnumerable<PropertyInfo> properties, TextWriter writer)
    {
        writer.WriteTableHeaderBegin();
        foreach (var property in properties)
        {
            var title = property.GetMemberDisplayName(true);
            writer.WriteTableHeaderCell(title);
        }

        writer.WriteTableHeaderEnd();
    }
}