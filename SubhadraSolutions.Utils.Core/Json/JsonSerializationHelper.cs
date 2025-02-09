using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading;
using SubhadraSolutions.Utils.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace SubhadraSolutions.Utils.Json;

public static class JsonSerializationHelper
{
    public static readonly MethodInfo JsonDeserializeTemplateMethod = typeof(JsonSerializationHelper)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m =>
            m.Name == nameof(BuildFromUnknown) && m.GetParameters().Length == 1);

    static JsonSerializationHelper()
    {
        JsonConvert.DefaultSettings = () =>
        {
            return JsonSettings.RestSerializerSettings;
        };
        Serializer = JsonSerializer.Create(JsonSettings.RestSerializerSettings);
        LinqSerializer = JsonSerializer.Create(JsonSettings.LinqSerializerSettings);
    }

    public static JsonSerializer Serializer { get; }
    public static JsonSerializer LinqSerializer { get; }

    [DynamicallyInvoked]
    public static T BuildFromUnknown<T>(object obj)
    {
        return BuildFromUnknown<T>(obj, Serializer);
    }

    public static T BuildFromUnknown<T>(object obj, JsonSerializer serializer)
    {
        if (obj == null)
        {
            return default;
        }

        var returnType = typeof(T);
        if (returnType.IsInstanceOfType(obj))
        {
            return (T)obj;
        }

        if (obj is JToken token)
        {
            return BuildObject<T>(token, serializer);
        }

        if (obj is string json)
        {
            return Deserialize<T>(json, serializer);
        }

        throw new Exception("Could not Build from unknown");
    }

    public static JObject BuildJObjectFromQuery(string query)
    {
        var collection = UrlHelper.GetQueryFromUrl(query);
        var kvps = collection.GetValuesFromNameValueCollection();
        return BuildJObjectFromQuery(kvps);
    }

    public static JObject BuildJObjectFromQuery(IEnumerable<KeyValuePair<string, StringValues>> kvps)
    {
        var converted = kvps.Select(kvp =>
            new KeyValuePair<string, string[]>(kvp.Key, kvp.Value.ConvertStringValuesToStringArray()));
        return BuildJObjectFromQuery(converted);
    }

    public static JObject BuildJObjectFromQuery(IEnumerable<KeyValuePair<string, string[]>> kvps)
    {
        var jObject = new JObject();

        foreach (var kvp in kvps)
        {
            var values = kvp.Value;
            var jArray = new JArray();

            for (var i = 0; i < values.Length; i++)
            {
                jArray.Add(values[i]);
            }

            jObject.Add(kvp.Key, jArray);
        }

        return jObject;
    }

    public static async Task<JToken> BuildJTokenFromStreamAsync(Stream stream, bool closeInput)
    {
        using var sr = new StreamReader(stream);
        using var jr = new JsonTextReader(sr)
        {
            CloseInput = closeInput
        };
        var jToken = await JToken.LoadAsync(jr).ConfigureAwait(false);
        return jToken;
    }

    //public static JsonSerializerSettings BuildJsonSerializerSettings()
    //{
    //    var settings = new JsonSerializerSettings
    //    {
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    //        TypeNameHandling = TypeNameHandling.Auto,
    //        MaxDepth = 128
    //    };
    //    //settings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
    //    return settings;
    //}

    public static object BuildObject(this JToken jToken, IServiceProvider serviceProvider, Type instanceType)
    {
        return jToken.BuildObject(serviceProvider, instanceType, Serializer);
    }

    public static object BuildObject(this JToken jToken, IServiceProvider serviceProvider, Type instanceType,
        JsonSerializer serializer)
    {
        try
        {
            ThreadingHelper.StoreDataInThreadLocalStorage("serviceProvider", serviceProvider);
            return BuildObject(jToken, instanceType);
        }
        finally
        {
            ThreadingHelper.RemoveDataFromThreadLocalStorage("serviceProvider");
        }
    }

    public static T BuildObject<T>(this JToken jToken, IServiceProvider serviceProvider)
    {
        var obj = jToken.BuildObject(serviceProvider, typeof(T));
        return (T)obj;
    }

    public static object BuildObject(this JObject jObj, IServiceProvider serviceProvider)
    {
        return jObj.BuildObject(serviceProvider, typeof(object));
    }

    public static T BuildObject<T>(JToken token)
    {
        return BuildObject<T>(token, Serializer);
    }

    public static T BuildObject<T>(JToken token, JsonSerializer serializer)
    {
        return (T)BuildObject(token, typeof(T), serializer);
    }

    public static object BuildObject(JToken token, Type objectType)
    {
        if (token is JObject jObject)
        {
            return jObject.BuildObjectFromJObject(objectType);
        }

        return token.BuildObjectFromJToken(objectType);
    }

    public static object BuildObject(JToken token, Type objectType, JsonSerializer serializer)
    {
        if (token is JObject jObject)
        {
            return jObject.BuildObjectFromJObject(objectType, serializer);
        }

        return token.BuildObjectFromJToken(objectType, serializer, out _);
    }

    public static object BuildObjectFromJObject(this JObject jObj, string key, Type targetType, out bool foundKey, out JToken value)
    {
        if (targetType == typeof(Remote.Linq.Expressions.Expression))
        {
            return jObj.BuildObjectFromJObject(key, targetType, out foundKey, out value, LinqSerializer);
        }
        return jObj.BuildObjectFromJObject(key, targetType, out foundKey,out value, Serializer);
    }

    public static object BuildObjectFromJObject(this JObject jObj, string key, Type targetType, out bool foundKey, out JToken value,
        JsonSerializer serializer)
    {
        value = null;
        foundKey = jObj.ContainsKey(key);
        if (!foundKey)
        {
            return null;
        }

        value = jObj[key];
        if (value == null)
        {
            return null;
        }

        return value.BuildObjectFromJToken(targetType, serializer, out value);
    }

    public static T DeepClone<T>(T obj)
    {
        return (T)DeepCloneObject(obj);
    }

    public static object DeepCloneObject(object obj)
    {
        using var ms = new MemoryStream();
        SerializeToStream(obj, ms);
        ms.Seek(0, SeekOrigin.Begin);
        return DeserializeFromStream(ms);
    }

    public static T Deserialize<T>(string json)
    {
        return Deserialize<T>(json, Serializer);
    }

    public static JToken DeserializeToJToken(string json)
    {
        return JToken.Parse(json);
    }

    public static T Deserialize<T>(string json, JsonSerializer serializer)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        var jToken = JToken.Parse(json);

        return BuildObject<T>(jToken, serializer);
    }

    public static T DeserializeFromFile<T>(string file)
    {
        return DeserializeFromFile<T>(file, Serializer);
    }

    public static T DeserializeFromFile<T>(string file, JsonSerializer serializer)
    {
        using var sr = new StreamReader(file);
        return DeserializeFromStream<T>(sr.BaseStream, serializer);
    }

    public static T DeserializeFromStream<T>(Stream s)
    {
        return DeserializeFromStream<T>(s, Serializer);
    }

    public static T DeserializeFromStream<T>(Stream s, JsonSerializer serializer)
    {
        var obj = DeserializeFromStream(s, typeof(T), serializer);
        return (T)obj;
    }

    public static object DeserializeFromStream(Stream s)
    {
        return DeserializeFromStream(s, Serializer);
    }

    public static object DeserializeFromStream(Stream s, JsonSerializer serializer)
    {
        return DeserializeFromStream(s, typeof(object), serializer);
    }

    public static object DeserializeFromStream(Stream s, Type objectType)
    {
        return DeserializeFromStream(s, objectType, Serializer);
    }

    public static object DeserializeFromStream(Stream s, Type objectType, JsonSerializer serializer)
    {
        using var sr = new StreamReader(s, Encoding.UTF8, true, 1024, true);
        using var jr = new JsonTextReader(sr);
        jr.CloseInput = false;
        var jToken = JToken.Load(jr);
        return BuildObject(jToken, objectType, serializer);
    }

    //public static string Serialize<T>(T obj)
    //{
    //    return Serialize(obj, Serializer);
    //}

    //public static string Serialize<T>(T obj, JsonSerializer serializer)
    //{
    //    var json = SerializeObject(obj, serializer);
    //    return json;
    //}

    public static string Serialize(object obj)
    {
        return Serialize(obj, Serializer);
    }

    public static string Serialize(object obj, JsonSerializer serializer)
    {
        if (obj == null)
        {
            return null;
        }

        using var sw = new StringWriter();
        serializer.Serialize(sw, obj);
        sw.Flush();
        return sw.ToString();
    }

    public static async Task<string> SerializeAsync(object obj, JsonSerializer serializer)
    {
        if (obj == null)
        {
            return null;
        }

        using (var memoryStream = new MemoryStream()) // RecyclableMemoryStream will be returned, it inherits MemoryStream, however prevents data allocation into the LOH
        {
            using (var writer = new StreamWriter(memoryStream))
            {
                serializer.Serialize(writer, obj);

                await writer.FlushAsync().ConfigureAwait(false);

                memoryStream.Seek(0, SeekOrigin.Begin);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }

    public static void SerializeToFile(object obj, string file)
    {
        SerializeToFile(obj, file, Serializer);
    }

    public static void SerializeToFile(object obj, string file, JsonSerializer serializer)
    {
        var directory = Path.GetDirectoryName(file);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        using var sw = new StreamWriter(file);
        serializer.Serialize(sw, obj);
    }

    public static void SerializeToStream(object obj, Stream s)
    {
        SerializeToStream(obj, s, Serializer);
    }

    public static void SerializeToStream(object obj, Stream s, JsonSerializer serializer)
    {
        using var sw = new StreamWriter(s, Encoding.UTF8, 1024, true);
        serializer.Serialize(sw, obj);
        sw.Flush();
    }

    private static object BuildObjectFromJObject(this JObject jObj, Type instanceType)
    {
        return jObj.BuildObjectFromJObject(instanceType, Serializer);
    }

    private static object BuildObjectFromJObject(this JObject jObj, Type instanceType, JsonSerializer serializer)
    {
        var instance = jObj.ToObject(instanceType, serializer);
        return instance;
    }

    private static object BuildObjectFromJToken(this JToken jToken, Type targetType)
    {
        return jToken.BuildObjectFromJToken(targetType, Serializer, out _);
    }

    private static object BuildObjectFromJToken(this JToken jToken, Type targetType, JsonSerializer serializer, out JToken value)
    {
        value = jToken;

        if (jToken is JArray jArray)
        {
            if (targetType.IsPrimitiveOrExtendedPrimitive() || !targetType.IsEnumerableType())
            {
                if (jArray.Count > 1)
                {
                    throw new InvalidOperationException("Cannot convert array to single object");
                }

                if (jArray.Count > 0)
                {
                    value = jArray[0];
                }
            }
        }

        return value.ToObject(targetType, serializer);
    }
}