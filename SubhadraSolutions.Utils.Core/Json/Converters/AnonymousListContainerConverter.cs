//using System.Collections;
//using System.Text.Json;
//using System.Text.Json.Nodes;
//using System.Text.Json.Serialization;
//using Newtonsoft.Json;

//namespace SubhadraSolutions.Utils.Json.Converters
//{
//    public class AnonymousListContainerConverter : JsonConverter<AnonymousListContainer>
//    {
//        private AnonymousListContainerConverter()
//        {
//        }

//        public static AnonymousListContainerConverter Instance { get; } = new();

//        public override AnonymousListContainer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        {
//            if (reader.TokenType == JsonTokenType.StartObject)
//            {
//                var token = JsonNode.Parse(ref reader);
//                var propertiesToken = token[nameof(AnonymousListContainer.Properties)];
//                var listToken = token[nameof(AnonymousListContainer.List)];

//                var properties = propertiesToken.Deserialize<List<Tuple<string, Type>>>(options);
//                var dtoType = AnonymousTypeBuilder.BuildAnonymousType(properties);
//                var list = (IList)listToken.Deserialize(typeof(List<>).MakeGenericType(dtoType), options);

//                return new AnonymousListContainer
//                {
//                    Properties = properties,
//                    List = list
//                };
//            }

//            return null;
//        }

//        public override void Write(Utf8JsonWriter writer, AnonymousListContainer value, JsonSerializerOptions options)
//        {
//            writer.WriteStartObject();
//            writer.WritePropertyName(nameof(AnonymousListContainer.Properties));
//            JsonSerializer.Serialize(writer, value.Properties, options);
//            writer.WritePropertyName(nameof(AnonymousListContainer.List));
//            JsonSerializer.Serialize(writer, value.List, options);
//            writer.WriteEndObject();
//            //JsonSerializer.Serialize(writer, value, options);
//        }
//    }
//}