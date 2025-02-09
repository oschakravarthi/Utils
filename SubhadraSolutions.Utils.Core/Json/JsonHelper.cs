using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.IO;
using System.Linq;

namespace SubhadraSolutions.Utils.Json;

public static class JsonHelper
{
    public static string JsonPrettify(string json)
    {
        if (json == null)
        {
            return null;
        }
        using (var stringReader = new StringReader(json))
        using (var stringWriter = new StringWriter())
        {
            var jsonReader = new JsonTextReader(stringReader);
            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }

    public static JToken GetFlattenJson(object obj, string path = null,
        bool forAzureAppServiceConfiguration = false)
    {
        if (obj == null)
        {
            return null;
        }

        JToken jToken;
        if (forAzureAppServiceConfiguration)
        {
            jToken = new JArray();
        }
        else
        {
            jToken = new JObject();
        }

        PopulateFlattenJson(obj, jToken, path, forAzureAppServiceConfiguration);
        return jToken;
    }

    private static void PopulateFlattenJson(object obj, JToken topJToken, string path,
        bool forAzureAppServiceConfiguration)
    {
        if (obj == null)
        {
            return;
        }

        var type = obj.GetType();
        if (type.IsPrimitiveOrExtendedPrimitive() || type.IsEnumerableType())
        {
            var jToken = JToken.FromObject(obj);
            if (forAzureAppServiceConfiguration)
            {
                var value = jToken == null ? null : jToken.ToString();
                var jObject = new JObject
                {
                    ["name"] = path,
                    ["value"] = value
                };
                var jArray = topJToken as JArray;
                jArray.Add(jObject);
            }
            else
            {
                topJToken[path] = jToken;
            }
        }
        else
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                var childPath = $"{path}:{property.Name}";
                PopulateFlattenJson(value, topJToken, childPath, forAzureAppServiceConfiguration);
            }
        }
    }

    public static JToken Normalize(this JToken token)
    {
        var result = token;

        switch (token.Type)
        {
            case JTokenType.Object:
                var jObject = (JObject)token;

                if (jObject?.HasValues == true)
                {
                    var newObject = new JObject();

                    foreach (var property in jObject.Properties().OrderBy(x => x.Name))
                    {
                        var value = property.Value as JToken;
                        if (value != null)
                        {
                            value = Normalize(value);
                        }

                        newObject.Add(property.Name, value);
                    }
                    return newObject;
                }

                break;

            case JTokenType.Array:

                var jArray = (JArray)token;

                if (jArray?.Count > 0)
                {
                    var normalizedArrayItems = jArray
                        .Select(x => x.Normalize())
                        .OrderBy(x => x.ToString(), StringComparer.Ordinal);

                    result = new JArray(normalizedArrayItems);
                }

                break;

            default:
                break;
        }

        return result;
    }

    public static bool DeepEquals(JToken a, JToken b)
    {
        if (a == b)
        {
            return true;
        }
        if (a == null || b == null)
        {
            return false;
        }
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        a = a.Normalize();
        b = b.Normalize();

        return JToken.DeepEquals(a, b);
    }
}