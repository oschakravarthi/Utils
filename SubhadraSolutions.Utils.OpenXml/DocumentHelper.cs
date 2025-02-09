using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Reporting;
using System;
using System.Collections;

namespace SubhadraSolutions.Utils.OpenXml;

public static class DocumentHelper
{
    public static void ExportToDocument<T>(T obj, IDocumentBuilder documentBuilder, string title = null)
    {
        var fontSize = 48;

        if (title != null)
        {
            documentBuilder.AddHeading(title, fontSize);
            fontSize -= 4;
        }

        ExportToDocument(typeof(T), obj, documentBuilder, fontSize);
    }

    private static void ExportToDocument(Type objectType, object obj, IDocumentBuilder documentBuilder, int fontSize)
    {
        foreach (var property in objectType.GetPublicProperties(true))
        {
            var propertyValue = property.GetValue(obj);
            if (propertyValue == null)
            {
                continue;
            }

            var propertyTitle = property.GetMemberDisplayName(true);
            documentBuilder.AddHeading(propertyTitle, fontSize);

            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                var elementType = property.PropertyType.GetEnumerableItemType();
                var method = documentBuilder.GetType().GetMethod(nameof(IDocumentBuilder.AddTable))
                    .MakeGenericMethod(elementType);
                method.Invoke(documentBuilder, [propertyValue]);
            }
            else
            {
                ExportToDocument(property.PropertyType, propertyValue, documentBuilder, fontSize - 4);
            }
        }
    }
}