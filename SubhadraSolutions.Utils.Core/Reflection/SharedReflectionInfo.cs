using System;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.Reflection;

public static class SharedReflectionInfo
{
    public static readonly MethodInfo ArrayIndexerGetMethod = typeof(object[]).GetMethod("Get");
    public static readonly MethodInfo ArrayIndexerSetMethod = typeof(object[]).GetMethod("Set");

    public static readonly MethodInfo ObjectToStringMethod =
        typeof(StringHelper).GetMethod("GetObjectAsString", BindingFlags.Static | BindingFlags.Public);

    public static readonly MethodInfo QueryableCountTemplateMethod = typeof(Queryable).GetMethods()
        .First(method => method.Name == nameof(Queryable.Count) && method.GetParameters().Length == 1);

    public static readonly MethodInfo QueryableSelectTemplateMethod = typeof(Queryable)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m =>
            m.Name == nameof(Queryable.Select) && m.IsGenericMethod && m.GetGenericArguments().Length == 2);

    public static readonly MethodInfo StringEqualsMethod = FindStringEqualsMethod();

    public static readonly MethodInfo StringInternMethod =
        typeof(string).GetMethod("Intern", BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo EnumParseMethod = typeof(Enum)
        .GetMethods(BindingFlags.Static | BindingFlags.Public).First(m =>
            !m.IsGenericMethod && m.Name == nameof(Enum.Parse) && m.GetParameters().Length == 2);

    private static MethodInfo FindStringEqualsMethod()
    {
        var stringType = typeof(string);
        var methods = stringType.GetMethods(BindingFlags.Public | BindingFlags.Static);

        for (var i = 0; i < methods.Length; i++)
        {
            var method = methods[i];
            if (method.ReturnType != typeof(bool))
            {
                continue;
            }

            if (method.Name != "Equals")
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 2)
            {
                continue;
            }

            if (parameters[0].ParameterType == stringType && parameters[1].ParameterType == stringType)
            {
                return method;
            }
        }

        return null;
    }
}