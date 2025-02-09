using Newtonsoft.Json;
using System;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class NewtonsoftDateTimeConverter : JsonConverter
{
    private NewtonsoftDateTimeConverter()
    {
    }

    public static NewtonsoftDateTimeConverter Instance { get; } = new();

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return existingValue;
        }

        if (reader.TokenType == JsonToken.String)
        {
            var value = (string)reader.Value;
            return Convert.ToDateTime(value);
        }

        return Convert.ToDouble(reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not DateTime val)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(val.ToString(GlobalSettings.Instance.DateAndTimeSerializationFormat));
    }
}