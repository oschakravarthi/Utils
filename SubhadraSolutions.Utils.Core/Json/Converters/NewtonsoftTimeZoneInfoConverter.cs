using Newtonsoft.Json;
using System;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class NewtonsoftTimeZoneInfoConverter : JsonConverter
{
    private NewtonsoftTimeZoneInfoConverter()
    {
    }

    public static NewtonsoftTimeZoneInfoConverter Instance { get; } = new();

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TimeZoneInfo);
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
            return TimeZoneInfo.FromSerializedString(value);
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not TimeZoneInfo val)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(val.ToSerializedString());
        }
    }
}