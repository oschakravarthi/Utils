using Newtonsoft.Json;
using System;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class NewtonsoftFloatConverter : JsonConverter
{
    private NewtonsoftFloatConverter()
    {
    }

    public static NewtonsoftFloatConverter Instance { get; } = new();

    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(float) || objectType == typeof(float?);
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
                    return float.NaN;

                case "I":
                    return float.PositiveInfinity;

                case "NI":
                    return float.NegativeInfinity;

                default:
                    return Convert.ToSingle(value);
            }
        }

        return Convert.ToSingle(reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not float val)
        {
            writer.WriteNull();
            return;
        }

        if (float.IsNaN(val))
        {
            writer.WriteValue("NaN");
            return;
        }

        if (float.IsPositiveInfinity(val))
        {
            writer.WriteValue("I");
            return;
        }

        if (float.IsNegativeInfinity(val))
        {
            writer.WriteValue("NI");
            return;
        }

        writer.WriteValue(val);
    }
}