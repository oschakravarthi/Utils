using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class FloatConverter : JsonConverter<float>
{
    private FloatConverter()
    {
    }

    public static FloatConverter Instance { get; } = new();

    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "NaN":
                    return float.NaN;

                case "I":
                    return float.PositiveInfinity;

                case "NI":
                    return float.NegativeInfinity;

                default:
                    return Convert.ToSingle(value);
            }
        }

        return reader.GetSingle(); // JsonException thrown if reader.TokenType != JsonTokenType.Number
    }

    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
    {
        if (float.IsNaN(value))
        {
            writer.WriteStringValue("NaN");
            return;
        }

        if (float.IsPositiveInfinity(value))
        {
            writer.WriteStringValue("I");
            return;
        }

        if (float.IsNegativeInfinity(value))
        {
            writer.WriteStringValue("NI");
            return;
        }

        writer.WriteNumberValue(value);
    }
}