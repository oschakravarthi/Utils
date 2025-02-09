using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class DoubleConverter : JsonConverter<double>
{
    private DoubleConverter()
    {
    }

    public static DoubleConverter Instance { get; } = new();

    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "NaN":
                    return double.NaN;

                case "I":
                    return double.PositiveInfinity;

                case "NI":
                    return double.NegativeInfinity;

                default:
                    return Convert.ToDouble(value);
            }
        }

        return reader.GetDouble(); // JsonException thrown if reader.TokenType != JsonTokenType.Number
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        if (double.IsNaN(value))
        {
            writer.WriteStringValue("NaN");
            return;
        }

        if (double.IsPositiveInfinity(value))
        {
            writer.WriteStringValue("I");
            return;
        }

        if (double.IsNegativeInfinity(value))
        {
            writer.WriteStringValue("NI");
            return;
        }

        writer.WriteNumberValue(value);
    }
}