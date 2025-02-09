//using System;
//using System.Collections.Generic;

//namespace SubhadraSolutions.Utils.Configuration;

//public class ApplicationSettings
//{
//    public ApplicationSettings(IDictionary<string, string> lookup)
//    {
//        if (lookup == null)
//        {
//            this.Lookup = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
//        }
//        else
//        {
//            this.Lookup = new Dictionary<string, string>(lookup, StringComparer.InvariantCultureIgnoreCase);
//        }
//    }

//    public string this[string key]
//    {
//        get
//        {
//            Lookup.TryGetValue(key, out var value);
//            return value;
//        }
//        set => Lookup[key] = value;
//    }

//    public IDictionary<string, string> Lookup { get; }
//}