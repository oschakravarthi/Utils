using SubhadraSolutions.Utils.Data.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SubhadraSolutions.Utils.Reflection;

public static class ReflectionHelper
{
    private static readonly MethodInfo GetDefaultValueTemplateMethod =
        typeof(ReflectionHelper).GetMethod(nameof(GetDefaultValue), BindingFlags.Static | BindingFlags.Public);

    public static object BuildListIfEnumerableAndNotListOrReturnSame(object obj)
    {
        var objType = obj.GetType();
        if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>))
        {
            return obj;
        }

        if (objType.IsPrimitiveOrExtendedPrimitive())
        {
            return obj;
        }

        var elementType = objType.GetEnumerableItemType();
        if (elementType == null)
        {
            return obj;
        }

        var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType), obj);
        return list;
    }

    public static void EnsureTypeHasAllTheSpecifiedProperties(this Type type, IEnumerable<string> propertyNames)
    {
        foreach (var property in propertyNames)
        {
            if (type.GetProperty(property) == null)
            {
                throw new MissingMemberException($"Property {property} does not exist in type {type}");
            }
        }
    }

    public static ConstructorInfo GetConstructorOfAnonymousType(this Type anonymousType)
    {
        var constructors = anonymousType.GetConstructors();

        if (constructors.Length == 1)
        {
            return constructors[0];
        }

        return constructors.First(c => c.GetParameters().Length > 0);
    }

    public static object GetDefaultValue<T>()
    {
        return default(T);
    }

    public static string GetFormatString(this PropertyInfo property, AttributesLookup attributesLookup = null)
    {
        return GetFormatString(property, out _, attributesLookup);
    }

    public static string GetFormatString(this PropertyInfo property, out DisplayFormatAttribute displayFormatAttribute,
        AttributesLookup attributesLookup = null)
    {
        string formatString = null;

        if (attributesLookup != null)
        {
            displayFormatAttribute = attributesLookup.GetCustomAttribute<DisplayFormatAttribute>(property, true);
        }
        else
        {
            displayFormatAttribute = property.GetCustomAttribute<DisplayFormatAttribute>(true);
        }

        if (displayFormatAttribute != null)
        {
            formatString = displayFormatAttribute.DataFormatString;
        }

        if (formatString == null)
        {
            if (property.PropertyType.IsDateType())
            {
                formatString = "{0:" + GlobalSettings.Instance.ShortDateFormat + "}";
                switch (property.Name)
                {
                    case "Month":
                    case "Week":
                        formatString = "{0:yyyy/MM}";
                        break;
                }
            }
            else
            {
                if (property.PropertyType.IsRealType())
                {
                    formatString = "{0:" + GlobalSettings.Instance.RealNumberFormat + "}";
                }
            }
        }

        return formatString;
    }

    public static string GetMemberDescription(this MemberInfo member)
    {
        return GetMemberDisplayNameOrDescription(member, true, false);
    }

    public static string GetMemberDescription(string memberName, DisplayAttribute displayAttribute,
        bool considerDescription, bool preferShortName)
    {
        if (considerDescription && !string.IsNullOrWhiteSpace(displayAttribute.Description))
        {
            return displayAttribute.Description;
        }

        if (preferShortName && !string.IsNullOrWhiteSpace(displayAttribute.ShortName))
        {
            return displayAttribute.ShortName;
        }

        if (!string.IsNullOrWhiteSpace(displayAttribute.Name))
        {
            return displayAttribute.Name;
        }

        if (!string.IsNullOrWhiteSpace(displayAttribute.ShortName))
        {
            return displayAttribute.ShortName;
        }

        //memberName = memberName.Replace("__", ".").Replace('_', ' ');
        return memberName.BuildDisplayName();
    }

    public static string GetMemberDisplayName(this MemberInfo member, bool preferShortName)
    {
        return GetMemberDisplayNameOrDescription(member, false, preferShortName);
    }

    public static string GetMemberDisplayName(this MemberInfo member, bool preferShortName,
        AttributesLookup attributesLookup)
    {
        return GetMemberDisplayNameOrDescription(member, false, preferShortName, attributesLookup);
    }

    public static object GetMemberValue(this MemberInfo member, object obj)
    {
        return member switch
        {
            FieldInfo fieldInfo => fieldInfo.GetValue(obj),
            PropertyInfo propertyInfo => propertyInfo.GetValue(obj),
            _ => throw new InvalidOperationException()
        };
    }

    public static string GetPropertyColumnName(this PropertyInfo property)
    {
        var columnName = property.Name;

        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>(true);

        if (columnAttribute != null)
        {
            columnName = columnAttribute.Name;
        }

        return columnName;
    }

    public static List<PropertyInfo> GetPublicProperties<T>(bool sortOnPropertyOrder)
    {
        return GetPublicProperties(typeof(T), sortOnPropertyOrder);
    }

    public static List<PropertyInfo> GetPublicProperties(this Type type, bool sortOnPropertyOrder)
    {
        var properties = GetPublicProperties(type);
        if (sortOnPropertyOrder)
        {
            properties.Sort(OrderComparer<PropertyInfo>.Instance);
        }

        return properties;
    }

    public static List<PropertyInfo> GetSortedPublicProperties<T>(this AttributesLookup attributesLookup)
    {
        return GetSortedPublicProperties(typeof(T), attributesLookup);
    }

    public static List<PropertyInfo> GetSortedPublicProperties(this Type type, AttributesLookup attributesLookup = null)
    {
        var properties = GetPublicProperties(type);

        if (attributesLookup == null)
        {
            properties.Sort(OrderComparer<PropertyInfo>.Instance);
        }
        else
        {
            properties.Sort(new AttributeLookupSupportedColumnOrderComparer(attributesLookup));
        }

        return properties;
    }

    public static ConstructorInfo GetSuitableConstructor(this Type type)
    {
        var defaultConstructor = type.GetConstructor(Type.EmptyTypes);
        if (defaultConstructor != null)
        {
            return defaultConstructor;
        }

        var constructors = type.GetConstructors();
        if (constructors.Length > 0)
        {
            return constructors[0];
        }

        return null;
    }

    public static T[] GetUnderlyingArrayOfList<T>(this List<T> list)
    {
        return (T[])list.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(list);
    }

    public static bool HasAllTheSpecifiedProperties(Type type, IEnumerable<string> propertyNames)
    {
        foreach (var property in propertyNames)
        {
            if (type.GetProperty(property) == null)
            {
                return false;
            }
        }

        return true;
    }

    //TODO: Not covering all validations. Just added checks only for space and a dot.
    public static bool IsValidMemberIdentifier(string name)
    {
        if (name == null)
        {
            return false;
        }

        return name.Contains('.') || name.Contains(' ');
    }

    //TODO: Not covering all validations. Just added checks only for space and a dot.
    public static string MakeValidMemberIdentifier(string name)
    {
        return name.Replace(".", "__").Replace(' ', '_');
    }

    public static string ToHumanReadableString(this Type type)
    {
        var name = type.FullName;
        if (!type.IsGenericType)
        {
            return name;
        }

        var index = name.IndexOf('`');
        name = name.Substring(0, index);
        var typeArguments = type.GetGenericArguments();
        var sb = new StringBuilder();
        sb.Append(name);
        sb.Append('<');

        for (var i = 0; i < typeArguments.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(',');
            }

            var typeName = typeArguments[i].ToHumanReadableString();
            sb.Append(typeName);
        }

        sb.Append('>');
        return sb.ToString();
    }

    private static string GetMemberDisplayNameOrDescription(this MemberInfo member, bool considerDescription,
        bool preferShortName)
    {
        var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();

        if (displayAttribute != null)
        {
            return GetMemberDescription(member.Name, displayAttribute, considerDescription, preferShortName);
        }

        return member.Name;
    }

    private static string GetMemberDisplayNameOrDescription(this MemberInfo member, bool considerDescription,
        bool preferShortName, AttributesLookup attributesLookup)
    {
        DisplayAttribute displayAttribute;
        if (attributesLookup != null)
        {
            displayAttribute = attributesLookup.GetCustomAttribute<DisplayAttribute>(member, true);
        }
        else
        {
            displayAttribute = member.GetCustomAttribute<DisplayAttribute>(true);
        }

        if (displayAttribute != null)
        {
            return GetMemberDescription(member.Name, displayAttribute, considerDescription, preferShortName);
        }

        var memberName = member.Name.BuildDisplayName();
        return memberName;
    }

    private static List<PropertyInfo> GetPublicProperties(this Type type)
    {
        if (type.IsInterface)
        {
            var propertyInfos = new List<PropertyInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);

            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();

                foreach (var subInterface in subType.GetInterfaces())
                {
                    if (considered.Contains(subInterface))
                    {
                        continue;
                    }

                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                var typeProperties =
                    subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

                var newPropertyInfos = new List<PropertyInfo>(typeProperties);

                propertyInfos.InsertRange(0, newPropertyInfos);
            }

            return propertyInfos;
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
        return properties.ToList();
        //var dictionary = new Dictionary<string, PropertyInfo>();

        //foreach (PropertyInfo property in properties)
        //{
        //    if (dictionary.TryGetValue(property.Name, out PropertyInfo p))
        //    {
        //        if (p.DeclaringType.IsAssignableFrom(property.DeclaringType))
        //        {
        //            dictionary[property.Name] = property;
        //        }
        //    }
        //    else
        //    {
        //        dictionary.Add(property.Name, property);
        //    }
        //}

        //return dictionary.Values.ToList();
    }

    public static object GetDefaultValueForType(this Type type)
    {
        var defaultValue = GetDefaultValueTemplateMethod.MakeGenericMethod(type).Invoke(null, Array.Empty<object>());
        return defaultValue;
    }
}