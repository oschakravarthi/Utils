using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class TimeZoneInfoConverter : JsonConverter<TimeZoneInfo>
{
    private TimeZoneInfoConverter()
    {
    }

    public static TimeZoneInfoConverter Instance { get; } = new();

    public override TimeZoneInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (value == null)
            {
                return null;
            }

            return TimeZoneInfo.FromSerializedString(value);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, TimeZoneInfo value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.ToSerializedString());
        }
    }
}