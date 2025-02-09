using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SubhadraSolutions.Utils.Json.Converters;

public abstract class AbstractJsonConverter<T> : JsonConverter
{
    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
        return typeof(T).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var target = Create(objectType, jsonObject);
        serializer.Populate(jsonObject.CreateReader(), target);
        return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    protected abstract T Create(Type objectType, JObject jsonObject);
}