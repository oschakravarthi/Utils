using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class DateTimeConverter : JsonConverter<DateTime>
{
    private DateTimeConverter()
    {
    }

    public static DateTimeConverter Instance { get; } = new();

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return Convert.ToDateTime(value);
        }

        return reader.GetDateTime(); // JsonException thrown if reader.TokenType != JsonTokenType.Number
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
       writer.WriteStringValue(value.ToString(GlobalSettings.Instance.DateAndTimeSerializationFormat));
    }
}