using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Json;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public static class DataReaderToJArrayHelper
{
    public static JArray BuildJArrayFromDataReader(this IDataReader dataReader)
    {
        var jArray = new JArray();
        while (dataReader.Read())
        {
            var jObj = new JObject();
            for (var i = 0; i < dataReader.FieldCount; i++)
            {
                var key = dataReader.GetName(i);
                var value = dataReader.GetValue(i);
                jObj[key] = JToken.FromObject(value, JsonSerializationHelper.Serializer);
            }

            jArray.Add(jObj);
        }

        return jArray;
    }
}