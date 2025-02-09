using System.Collections.Generic;
using System.Xml;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class CompositeKeyValue<TKey, TValue>
{
    public CompositeKeyValue()
    {
    }

    public CompositeKeyValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public CompositeKeyValue(KeyValuePair<TKey, TValue> kvp)
    {
        Key = kvp.Key;
        Value = kvp.Value;
    }

    public CompositeKeyValue(TKey key, TValue value, List<CompositeKeyValue<TKey, TValue>> innerKeyValues)
    {
        Key = key;
        Value = value;
        InnerKeyValues = innerKeyValues;
    }

    public List<CompositeKeyValue<TKey, TValue>> InnerKeyValues { get; set; }

    public TKey Key { get; set; }

    public CompositeKeyValue<TKey, TValue> Parent { get; set; }

    public TValue Value { get; set; }

    public static XmlDocument ExportAsXmlDocument(IEnumerable<CompositeKeyValue<TKey, TValue>> items)
    {
        var document = new XmlDocument();
        ExportToXmlDocument(items, document, document);
        return document;
    }

    public string GetPath(string separator)
    {
        var node = this;
        string path = null;
        while (node != null)
        {
            path = node.Key + separator + path;
            node = node.Parent;
        }

        if (!string.IsNullOrEmpty(separator))
        {
            return path.Substring(0, path.Length - separator.Length);
        }

        return path;
    }

    private static void ExportToXmlDocument(IEnumerable<CompositeKeyValue<TKey, TValue>> items, XmlNode parentNode,
        XmlDocument document)
    {
        XmlNode itemsNode = document.CreateElement("Items");
        parentNode.AppendChild(itemsNode);
        foreach (var item in items)
        {
            ExportToXmlDocument(item, itemsNode, document);
        }
    }

    private static void ExportToXmlDocument(CompositeKeyValue<TKey, TValue> item, XmlNode parentNode,
        XmlDocument document)
    {
        XmlNode itemNode = document.CreateElement("Item");
        var keyAttribute = document.CreateAttribute("Key");
        keyAttribute.Value = item.Key.ToString();
        itemNode.Attributes.Append(keyAttribute);

        if (item.Value != null)
        {
            var valueAttribute = document.CreateAttribute("Value");
            valueAttribute.Value = item.Value.ToString();
            itemNode.Attributes.Append(valueAttribute);
        }

        parentNode.AppendChild(itemNode);
        if (item.InnerKeyValues != null)
        {
            ExportToXmlDocument(item.InnerKeyValues, itemNode, document);
        }
    }
}