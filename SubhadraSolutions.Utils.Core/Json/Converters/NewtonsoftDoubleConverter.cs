using Newtonsoft.Json;
using System;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class NewtonsoftDoubleConverter : JsonConverter
{
    private NewtonsoftDoubleConverter()
    {
    }

    public static NewtonsoftDoubleConverter Instance { get; } = new();

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(double) || objectType == typeof(double?);
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

        return Convert.ToDouble(reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not double val)
        {
            writer.WriteNull();
            return;
        }

        if (double.IsNaN(val))
        {
            writer.WriteValue("NaN");
            return;
        }

        if (double.IsPositiveInfinity(val))
        {
            writer.WriteValue("I");
            return;
        }

        if (double.IsNegativeInfinity(val))
        {
            writer.WriteValue("NI");
            return;
        }

        writer.WriteValue(val);
    }
}