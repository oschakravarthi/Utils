//using System.Collections;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace SubhadraSolutions.Utils.Json.Converters
//{
//    public class NewtonsoftAnonymousListContainerConverter : JsonConverter
//    {
//        private NewtonsoftAnonymousListContainerConverter()
//        {
//        }

//        public static NewtonsoftAnonymousListContainerConverter Instance { get; } = new NewtonsoftAnonymousListContainerConverter();

//        public override bool CanRead
//        {
//            get
//            {
//                return true;
//            }
//        }

//        public override bool CanWrite
//        {
//            get
//            {
//                return false;
//            }
//        }

//        public override bool CanConvert(Type objectType)
//        {
//            return objectType == typeof(AnonymousListContainer);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType == JsonToken.Null)
//            {
//                return existingValue;
//            }
//            if (reader.TokenType == JsonToken.StartObject)
//            {
//                var token = ((JTokenReader)reader).CurrentToken;
//                var propertiesToken = token[nameof(AnonymousListContainer.Properties)];
//                var listToken = token[nameof(AnonymousListContainer.List)];

//                var properties = propertiesToken.ToObject<List<Tuple<string, Type>>>();
//                var dtoType = AnonymousTypeBuilder.BuildAnonymousType(properties);
//                var list = (IList)listToken.ToObject(typeof(List<>).MakeGenericType(dtoType));

//                return new AnonymousListContainer
//                {
//                    Properties = properties,
//                    List = list
//                };
//            }
//            return existingValue;
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}