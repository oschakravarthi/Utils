//using System;
//using System.Collections.Generic;
//using Newtonsoft.Json.Linq;

//namespace SubhadraSolutions.Utils;

//public static class DynamicHelper
//{
//    /// <summary>
//    /// Retrieves the named field from the given dynamic.
//    /// </summary>
//    /// <param name="d"></param>
//    /// <param name="fieldName"></param>
//    /// <returns></returns>
//    public static dynamic DynamicFieldLookup(dynamic d, string fieldName)
//    {
//        if (!((IDictionary<string, JToken>)d).ContainsKey(fieldName))
//        {
//            throw new ArgumentException($"Input JSON missing required field {fieldName}");
//        }

//        return d[fieldName];
//    }

//    /// <summary>
//    ///     Retrieves the named field from the given dynamic and casts it to type T.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="d"></param>
//    /// <param name="fieldName"></param>
//    /// <returns></returns>
//    public static T DynamicFieldLookup<T>(dynamic d, string fieldName)
//    {
//        if (!((IDictionary<string, JToken>)d).ContainsKey(fieldName))
//        {
//            throw new ArgumentException($"Input JSON missing required field {fieldName}");
//        }

//        return (T)Convert.ChangeType(d[fieldName], typeof(T));
//    }
//}