using Aqua.TypeSystem;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;
using SubhadraSolutions.Utils.Json;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace SubhadraSolutions.Utils.Compression.LZ4;

public static class CompressionHelper
{
    private static readonly LZ4EncoderSettings encoderSettings = new()
    {
        CompressionLevel = LZ4Level.L12_MAX
    };

    private static readonly TypeInfoProvider typeInfoProvider = new();

    public static void Decompress(this byte[] data, Stream outputStream)
    {
        var stream = data.Decompress();
        stream.CopyTo(outputStream);
        outputStream.Flush();
        stream.Dispose();
    }

    public static Stream Decompress(this byte[] data)
    {
        return data.Decompress(out var _);
    }

    public static Stream Decompress(this byte[] data, out Type returnType)
    {
        var length = BitConverter.ToInt32(data, 0);
        var typeName = Encoding.Unicode.GetString(data, 4, length);
        var typeInfo = DeserializeTypeInfo(typeName);
        returnType = typeInfo.ToType();

        var metaLength = 4 + length;
        var ms = new MemoryStream(data, metaLength, data.Length - metaLength);

        Stream stream = returnType.IsPrimitiveOrExtendedPrimitive() ? ms : LZ4Stream.Decode(ms, leaveOpen: false);
        return stream;
    }

    public static T DecompressAndDeserialize<T>(this byte[] data)
    {
        return (T)data.DecompressAndDeserialize();
    }

    public static object DecompressAndDeserialize(this byte[] data)
    {
        var stream = data.Decompress(out var returnType);
        if (returnType == typeof(string))
        {
            using var sr = new StreamReader(stream, Encoding.Unicode);
            return sr.ReadToEnd();
        }

        return JsonSerializationHelper.DeserializeFromStream(stream, returnType);
    }

    public static byte[] SerializeAndCompress(this object obj)
    {
        var list = ReflectionHelper.BuildListIfEnumerableAndNotListOrReturnSame(obj);
        if (list != null)
        {
            obj = list;
        }

        var objectType = obj.GetType();
        var typeInfo = typeInfoProvider.GetTypeInfo(objectType);
        var typeName = SerializeTypeInfo(typeInfo);
        var typeNameBytes = Encoding.Unicode.GetBytes(typeName);
        var lengthBytes = BitConverter.GetBytes(typeNameBytes.Length);
        using var ms = new MemoryStream();

        ms.Write(lengthBytes, 0, lengthBytes.Length);
        ms.Write(typeNameBytes, 0, typeNameBytes.Length);

        Stream stream = objectType.IsPrimitiveOrExtendedPrimitive() ? ms : LZ4Stream.Encode(ms, encoderSettings, true);
        if (obj is string s)
        {
            using var sw = new StreamWriter(stream, Encoding.Unicode);
            sw.Write(s);
        }
        else
        {
            JsonSerializationHelper.SerializeToStream(obj, stream);
        }

        if (stream != ms)
        {
            stream.Dispose();
        }

        return ms.ToArray();
    }

    private static TypeInfo DeserializeTypeInfo(string xml)
    {
        var serializer = new DataContractSerializer(typeof(TypeInfo));
        using var input = new StringReader(xml);
        using var reader = XmlReader.Create(input);
        return (TypeInfo)serializer.ReadObject(reader);
    }

    private static string SerializeTypeInfo(TypeInfo typeInfo)
    {
        var serializer = new DataContractSerializer(typeof(TypeInfo));
        using var output = new StringWriter();
        using (var writer = XmlWriter.Create(output))
        {
            serializer.WriteObject(writer, typeInfo);
        }

        return output.ToString();
    }
}