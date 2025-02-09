using Newtonsoft.Json;
using System;

namespace SubhadraSolutions.Utils.Json.Converters;

public class NewtonsoftExceptionConverter : JsonConverter
{
    public static NewtonsoftExceptionConverter Instance { get; } = new();

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType)
    {
        return typeof(Exception).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            var value = (string)reader.Value;
            return new Exception(value);
        }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var ex = (Exception)value;
        writer.WriteValue(ex.ToDetailedString());
    }
}