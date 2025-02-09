using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System.Collections.Generic;
using System.Reflection;

namespace SubhadraSolutions.Utils;

public static class InternExtensions
{
    public static readonly MethodInfo InternItemsTemplateMethod =
        typeof(InternExtensions).GetMethod(nameof(InternItems), BindingFlags.Public | BindingFlags.Static);

    public static T InternFluent<T>(this T a)
    {
        return GlobalInterner<T>.InternFluent(a);
    }

    [DynamicallyInvoked]
    public static void InternItems<T>(this IEnumerable<T> objects)
    {
        foreach (var obj in objects)
        {
            InternObject(obj);
        }
    }

    public static T1 InternItemsFluent<T, T1>(this T1 objects) where T1 : IEnumerable<T>
    {
        return GlobalInterner<T>.InternItemsFluent(objects);
    }

    public static void InternObject<T>(this T a)
    {
        GlobalInterner<T>.InternObject(a);
    }

    public static IEnumerable<T> WrapIntern<T>(this IEnumerable<T> objects)
    {
        return GlobalInterner<T>.WrapIntern(objects);
    }
}