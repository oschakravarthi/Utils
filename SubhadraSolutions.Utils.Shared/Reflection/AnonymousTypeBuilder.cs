using Aqua.TypeSystem;
using Aqua.TypeSystem.Emit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Reflection;

public static class AnonymousTypeBuilder
{
    public static Type BuildAnonymousType(IEnumerable<Tuple<string, Type>> properties)
    {
        var typeInfo = CreateAnonymousTypeInfo(properties);
        var emitter = new TypeEmitter();
        var emittedType = emitter.EmitType(typeInfo);
        return emittedType.MakeGenericType(properties.Select(p => p.Item2).ToArray());
    }

    private static TypeInfo CreateAnonymousTypeInfo(IEnumerable<Tuple<string, Type>> properties)
    {
        var genericArgs = properties.Select(x => new TypeInfo(x.Item2)).ToList();

        var t = new TypeInfo
        {
            IsAnonymousType = true,
            IsGenericType = true,
            Name = "T",
            Namespace = "N",
            GenericArguments = genericArgs
        };

        t.Properties = properties.Select(x => new PropertyInfo(x.Item1, new TypeInfo(x.Item2), t)).ToList();
        return t;
    }
}