using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class ExceptionConverter : JsonConverter<Exception>
{
    private ExceptionConverter()
    {
    }

    public static ExceptionConverter Instance { get; } = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Exception).IsAssignableFrom(typeToConvert);
    }

    public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return new Exception(value);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToDetailedString());
    }
}