using SubhadraSolutions.Utils.Reflection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Json.Converters;

public sealed class TypeJsonConverter : JsonConverter<Type>
{
    private TypeJsonConverter()
    {
    }

    public static TypeJsonConverter Instance { get; } = new();

    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return CoreReflectionHelper.GetType(value);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        var assemblyQualifiedName = value.AssemblyQualifiedName;
        writer.WriteStringValue(assemblyQualifiedName);
    }
}