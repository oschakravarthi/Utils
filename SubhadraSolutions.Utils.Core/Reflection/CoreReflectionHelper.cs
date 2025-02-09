using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Json;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SubhadraSolutions.Utils.Reflection;

public static class CoreReflectionHelper
{
    public static string GetDisplayGroupName(this MemberInfo member)
    {
        var attribute = member.GetCustomAttribute<DisplayAttribute>();
        return attribute == null ? null : attribute.GroupName;
    }

    public static void SetValuesForStaticProperties(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        foreach (var kvp in keyValuePairs)
        {
            SetValueForStaticProperty(kvp.Key, kvp.Value);
        }
    }

    public static void SetValueForStaticProperty(string key, string value)
    {
        var lastIndex = key.LastIndexOf('.');
        if (lastIndex <= 0)
        {
            throw new ArgumentException($"Invalid Key {key}");
        }
        var classname = key.Substring(0, lastIndex);
        var propertyName = key.Substring(lastIndex + 1);
        var type = GetType(classname);
        if (type == null)
        {
            throw new ArgumentException($"Could not find type {classname}");
        }
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
        if (property == null)
        {
            throw new ArgumentException($"Could not find property {property} in type {classname}");
        }
        if (!property.IsDefined<ConfigurableAttribute>(false, false))
        {
            throw new ArgumentException($"ConfigurableAttribute is not defined on property {property} in type {classname}");
        }
        var jobj = new JObject
        {
            [propertyName] = value
        };
        var propertyValue = JsonSerializationHelper.BuildObjectFromJObject(jobj, propertyName, property.PropertyType, out _, out _);
        property.SetValue(null, propertyValue);
    }

    public static Type GetEnumerableItemType(this Type type)
    {
        if (type == typeof(string))
        {
            return null;
        }

        if (!typeof(IEnumerable).IsAssignableFrom(type))
        {
            return null;
        }

        foreach (var i in type.GetInterfaces())
        {
            if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return i.GetGenericArguments()[0];
            }
        }

        if (type.IsGenericType)
        {
            return type.GetGenericArguments()[0];
        }

        return typeof(object);
    }

    public static Type GetEnumerableItemTypeIfEnumerable(object data)
    {
        if (data == null)
        {
            return null;
        }

        if (data is IQueryable queryable)
        {
            return LinqHelper.GetLinqElementType(queryable);
        }

        return GetEnumerableItemType(data.GetType());
    }

    public static Assembly LoadAssemblyFromFileByShortAssemblyName(string shortAssemblyName)
    {
        var fileFormat = @"{0}\" + shortAssemblyName + ".dll";
        var dllFileName =
            string.Format(fileFormat, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        var assembly = LoadAssemblyFromFile(dllFileName);
        if (assembly != null)
        {
            return assembly;
        }

        dllFileName = string.Format(fileFormat,
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"bin");
        assembly = LoadAssemblyFromFile(dllFileName);
        if (assembly != null)
        {
            return assembly;
        }

        dllFileName = string.Format(fileFormat, AppDomain.CurrentDomain.BaseDirectory);
        assembly = LoadAssemblyFromFile(dllFileName);
        if (assembly != null)
        {
            return assembly;
        }

        dllFileName = string.Format(fileFormat, AppDomain.CurrentDomain.BaseDirectory + @"bin");
        assembly = LoadAssemblyFromFile(dllFileName);
        if (assembly != null)
        {
            return assembly;
        }

        dllFileName = string.Format(fileFormat, AppDomain.CurrentDomain.BaseDirectory + @"bin\CodeShare");
        assembly = LoadAssemblyFromFile(dllFileName);
        return assembly;
    }

    public static Type GetType(string typeName)
    {
        var t = Type.GetType(typeName);
        if (t != null)
        {
            return t;
        }

        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            t = a.GetType(typeName);
            if (t != null)
            {
                return t;
            }
        }

        if (!typeName.Contains(','))
        {
            return TypesLookupHelper.GetType(typeName);
        }

        var parser = new TypeParser(typeName);

        if (parser.FullyQualifiedTypeName != null)
        {
            Assembly assembly;
            try
            {
                assembly = LoadAssembly(parser.AssemblyDescriptionString);
            }
            catch (Exception ex)
            {
                assembly = LoadAssemblyFromFileByShortAssemblyName(parser.ShortAssemblyName);
                Debug.WriteLine(ex.ToDetailedString());
            }

            if (assembly != null)
            {
                t = assembly.GetType(parser.ShortTypeName);

                if (t != null)
                {
                    return t;
                }
            }
        }

        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            t = a.GetType(parser.FullyQualifiedTypeName);
            if (t != null)
            {
                return t;
            }
        }

        return null;
    }

    public static List<Assembly> LoadAssemblies(string assembliesDirectory)
    {
        var files = Directory.GetFiles(assembliesDirectory, "*.dll", SearchOption.AllDirectories);
        return files.Select(Assembly.LoadFile).ToList();
    }

    public static Assembly LoadAssembly(string fullyQualifiedAssemblyName)
    {
        var assembly = Assembly.Load(fullyQualifiedAssemblyName);
        return assembly;
    }

    public static Assembly LoadAssemblyFromFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            return Assembly.LoadFrom(fileName);
        }

        return null;
    }

    public static bool IsEnumerableType(this Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type);
    }

    public static bool IsPrimitiveOrExtendedPrimitive(this Type type)
    {
        // if (!type.IsPrimitive && !(type == typeof(string)) && !type.IsEnum && !(type == typeof(DateTime)) && !(type == typeof(TimeSpan)))
        // {
        //    return type == typeof(SqlDecimal);
        // }
        return type.IsPrimitive || type.IsNumericType() || type == typeof(bool) || type == typeof(bool?) ||
               type == typeof(string) || type == typeof(char) || type == typeof(char?) || type == typeof(Guid) ||
               type == typeof(Guid?) || type.IsEnum || type == typeof(DateTime) || type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?) || type == typeof(TimeSpan) ||
               type == typeof(TimeSpan?) || type == typeof(decimal) || type == typeof(decimal?) ||
               type == typeof(SqlDecimal) || type == typeof(SqlDecimal?);
    }

    public static bool IsNumericType(this Type type)
    {
        return type.IsIntegerType() || type.IsRealType();
    }

    public static bool IsIntegerType(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        return type == typeof(byte)
               || type == typeof(byte?)
               || type == typeof(long)
               || type == typeof(long?)
               || type == typeof(sbyte)
               || type == typeof(sbyte?)
               || type == typeof(ulong)
               || type == typeof(ulong?)
               || type == typeof(int)
               || type == typeof(int?)
               || type == typeof(uint)
               || type == typeof(uint?)
               || type == typeof(short)
               || type == typeof(short?)
               || type == typeof(ushort)
               || type == typeof(ushort?);
    }

    public static bool IsRealType(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        return type == typeof(float)
               || type == typeof(float?)
               || type == typeof(double)
               || type == typeof(double?)
               || type == typeof(decimal)
               || type == typeof(decimal?);
    }

    public static bool IsAnonymousType(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        // HACK: The only way to detect anonymous types right now.
        // return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
        //    && type.IsGenericType && type.Name.Contains("AnonymousType")
        //    && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
        //    && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        return type.Name.StartsWith("<>") || type.Name.StartsWith("VB$");
    }

    public static bool IsDateOrTimeType(this Type type)
    {
        return type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(DateTimeOffset) ||
               type == typeof(DateTimeOffset?) || type == typeof(DateOnly) || type == typeof(DateOnly?) ||
               type == typeof(TimeOnly) || type == typeof(TimeOnly?);
    }

    public static bool IsDateType(this Type type)
    {
        return type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(DateTimeOffset) ||
               type == typeof(DateTimeOffset?) || type == typeof(DateOnly) || type == typeof(DateOnly?);
    }

    public static bool IsDefined<T>(this MemberInfo member, bool inherit, bool checkInterfaces)
        where T : Attribute
    {
        return GetAttribute<T>(member, inherit, checkInterfaces) != null;
    }

    public static Type FindIEnumerable(this Type seqType)
    {
        if (seqType == null || seqType == typeof(string))
        {
            return null;
        }

        if (seqType.IsArray)
        {
            return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
        }

        if (seqType.IsGenericType)
        {
            foreach (var type in seqType.GetGenericArguments())
            {
                var type2 = typeof(IEnumerable<>).MakeGenericType(type);
                if (type2.IsAssignableFrom(seqType))
                {
                    return type2;
                }
            }
        }

        var interfaces = seqType.GetInterfaces();

        if (interfaces != null && interfaces.Length != 0)
        {
            var genericArguments = interfaces;

            for (var i = 0; i < genericArguments.Length; i++)
            {
                var type3 = FindIEnumerable(genericArguments[i]);
                if (type3 != null)
                {
                    return type3;
                }
            }
        }

        if (seqType.BaseType != null && seqType.BaseType != typeof(object))
        {
            return FindIEnumerable(seqType.BaseType);
        }

        return null;
    }

    public static T GetAttribute<E, T>(this E enumVal) where T : Attribute where E : Enum
    {
        var type = enumVal.GetType();
        var memInfo = type.GetMember(enumVal.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 ? (T)attributes[0] : null;
    }

    public static T GetAttribute<T>(this MemberInfo member, bool inherit, bool checkInterfaces)
        where T : Attribute
    {
        var customAttribute = member.GetCustomAttribute<T>(inherit);

        if (customAttribute != null)
        {
            return customAttribute;
        }

        if (!checkInterfaces)
        {
            return null;
        }

        foreach (var iface in member.DeclaringType.GetInterfaces())
        {
            var interfaceMap = member.ReflectedType.GetInterfaceMap(iface);
            var targetMethods = interfaceMap.TargetMethods;

            if (member is MethodInfo method)
            {
                if (targetMethods.Contains(member))
                {
                    foreach (var im in interfaceMap.InterfaceMethods)
                    {
                        if (im.Name == member.Name && DoesSignatureEqual(im, method))
                        {
                            customAttribute = im.GetCustomAttribute<T>(inherit);

                            if (customAttribute != null)
                            {
                                return customAttribute;
                            }

                            break;
                        }
                    }
                }
            }
        }

        return null;
    }

    public static bool DoesSignatureEqual(MethodInfo method1, MethodInfo method2)
    {
        if (method1.ReturnType != method2.ReturnType)
        {
            return false;
        }

        var method1parameters = method1.GetParameters();
        var method2parameters = method2.GetParameters();

        if (method1parameters.Length != method2parameters.Length)
        {
            return false;
        }

        for (var i = 0; i < method1parameters.Length; i++)
            if (method1parameters[i].ParameterType != method2parameters[i].ParameterType)
            {
                return false;
            }

        return true;
    }

    public static bool IsDynamic(this PropertyInfo property)
    {
        return property.IsDefined(typeof(DynamicAttribute), true);
    }

    public static Delegate BuildPropertyAccessor(this PropertyInfo property)
    {
        return BuildPropertyAccessor(property, property.PropertyType);
    }

    public static Delegate BuildPropertyAccessor(this PropertyInfo property, Type returnType)
    {
        return BuildPropertyAccessor(property.DeclaringType, property, returnType);
    }

    [ILEmitter]
    public static Delegate BuildPropertyAccessor(this Type objectType, PropertyInfo property, Type returnType)
    {
        var dm = new DynamicMethod("getPropertyValueDynamicMethod" + GeneralHelper.Identity, property.PropertyType,
            [property.DeclaringType], typeof(CoreReflectionHelper));
        var ilGen = dm.GetILGenerator();
        EmitIlForPropertyAccessor(property, ilGen);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate(typeof(Func<,>).MakeGenericType(objectType, returnType));
    }

    public static LambdaExpression BuildPropertyAccessorAsLambda(this Type entityType, PropertyInfo property)
    {
        var parameterExpression = Expression.Parameter(entityType, "e");
        Expression expression = Expression.Property(parameterExpression, property);
        var exp = Expression.Lambda(typeof(Func<,>).MakeGenericType(entityType, property.PropertyType), expression,
            parameterExpression);
        return exp;
    }

    public static Func<T, object> BuildPropertyAccessorAsObject<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        return BuildPropertyAccessorAsObject<T>(property);
    }

    [ILEmitter]
    public static Func<T, object> BuildPropertyAccessorAsObject<T>(this PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var dm = new DynamicMethod("getPropertyValueDynamicMethodAsObject" + GeneralHelper.Identity, propertyType,
            [typeof(T)], typeof(CoreReflectionHelper));
        var ilGen = dm.GetILGenerator();
        EmitIlForPropertyAccessor(property, ilGen);
        if (propertyType.IsValueType)
        {
            if (!(propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                ilGen.Emit(OpCodes.Box, propertyType);
            }
        }

        ilGen.Emit(OpCodes.Ret);
        return (Func<T, object>)dm.CreateDelegate(typeof(Func<T, object>).MakeGenericType(typeof(T), propertyType));
    }

    public static Func<TItem, string> BuildPropertyGetter<TItem>(string propertyName, bool kmNumberFormat,
        AttributesLookup attributesLookup = null)
    {
        var property = typeof(TItem).GetProperty(propertyName);
        return BuildPropertyGetter<TItem>(property, kmNumberFormat, attributesLookup);
    }

    public static bool IsKMNumberFormatSupported(this PropertyInfo property,
        AttributesLookup attributesLookup = null)
    {
        if (!property.PropertyType.IsNumericType())
        {
            return false;
        }

        if (attributesLookup != null)
        {
            return !attributesLookup.IsDefined<NotMeasureAttribute>(property, true);
        }

        return !property.IsDefined(typeof(NotMeasureAttribute), true);
    }

    public static Func<TItem, string> BuildPropertyGetter<TItem>(this PropertyInfo property, bool kmNumberFormat,
        AttributesLookup attributesLookup = null)
    {
        var propertyType = property.PropertyType;
        var kmNumberFormatSupported = IsKMNumberFormatSupported(property, attributesLookup);
        if (kmNumberFormat && kmNumberFormatSupported)
        {
            var objectType = typeof(TItem);
            var dynamicMethod = new DynamicMethod("DynamicString" + GeneralHelper.Identity, typeof(string),
                [objectType], typeof(PropertiesToStringValuesHelper));
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Callvirt, property.GetMethod);
            var local = ilGen.DeclareLocal(propertyType);
            ilGen.Emit(OpCodes.Stloc, local);

            Label? ifNull = null;
            Label? ret = null;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var rawPropertyType = propertyType.GetGenericArguments()[0];
                //var local1 = ilGen.DeclareLocal(rawPropertyType);
                //EmitHelper.InitializeLocal(local1, ilGen);

                ifNull = ilGen.DefineLabel();
                ret = ilGen.DefineLabel();

                ilGen.Emit(OpCodes.Ldloca_S, local);
                var hasValueMethod = propertyType.GetProperty(nameof(Nullable<int>.HasValue)).GetMethod;
                ilGen.Emit(OpCodes.Call, hasValueMethod);
                ilGen.Emit(OpCodes.Brfalse_S, ifNull.Value);

                ilGen.Emit(OpCodes.Ldloca_S, local);
                var getValueMethod = propertyType.GetProperty(nameof(Nullable<int>.Value)).GetMethod;
                ilGen.Emit(OpCodes.Call, getValueMethod);
                //ilGen.Emit(OpCodes.Stloc, local1);
                //ilGen.Emit(OpCodes.Ldloc, local1);

                propertyType = rawPropertyType;
            }
            else
            {
                ilGen.Emit(OpCodes.Ldloc, local);
            }

            if (propertyType == typeof(decimal))
            {
                ilGen.Emit(OpCodes.Call, NumberHelper.MinifyDecimalMethod);
            }
            else
            {
                if (propertyType != typeof(double))
                {
                    ilGen.Emit(OpCodes.Conv_R8);
                }

                ilGen.Emit(OpCodes.Call, NumberHelper.MinifyDoubleMethod);
            }

            if (ret != null)
            {
                ilGen.Emit(OpCodes.Br_S, ret.Value);
            }

            if (ifNull != null)
            {
                ilGen.MarkLabel(ifNull.Value);
                ilGen.Emit(OpCodes.Ldnull);
                ilGen.Emit(OpCodes.Br_S, ret.Value);
            }

            if (ret != null)
            {
                ilGen.MarkLabel(ret.Value);
            }

            ilGen.Emit(OpCodes.Ret);
            return (Func<TItem, string>)dynamicMethod.CreateDelegate(typeof(Func<TItem, string>));
        }

        return PropertiesToStringValuesHelper.BuildDynamicMethodForToString<TItem>(property);
    }

    public static Func<TItem, TValue> BuildPropertyGetter<TItem, TValue>(string propertyName)
    {
        var arg = Expression.Parameter(typeof(TItem));

        var body = Expression.PropertyOrField(arg, propertyName);
        Expression result = body;
        var property = (PropertyInfo)body.Member;
        if (property.PropertyType != typeof(TValue))
        {
            result = Expression.Convert(result, typeof(TValue));
        }

        return Expression.Lambda<Func<TItem, TValue>>(result, arg).Compile();
    }

    [ILEmitter]
    public static void EmitIlForPropertyAccessor(this PropertyInfo property, ILGenerator ilGen)
    {
        ilGen.Emit(OpCodes.Ldarg, 0);
        ilGen.Emit(OpCodes.Callvirt, property.GetMethod);
    }
}