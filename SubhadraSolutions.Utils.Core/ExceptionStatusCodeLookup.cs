using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils
{
    public class ExceptionStatusCodeLookup
    {
        private static readonly Dictionary<Type, int> lookup = [];

        public static void Add<T>(int statucCode) where T : Exception
        {
            lookup[typeof(T)] = statucCode;
        }

        public static int? Get(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (lookup.TryGetValue(type, out var result))
            {
                return result;
            }
            return null;
        }
    }
}