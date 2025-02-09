using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.Data.Annotations;

public class AttributesLookup : IEquatable<AttributesLookup>
{
    //private readonly Type originalType;
    private readonly List<Type> originalTypes;

    public AttributesLookup(Type type)
    {
        originalTypes = [type];
    }

    public AttributesLookup(IEnumerable<Type> types)
    {
        originalTypes = [];
        var hashSet = new HashSet<Type>();
        foreach (var type in types)
        {
            if (!hashSet.Contains(type))
            {
                hashSet.Add(type);
                originalTypes.Add(type);
            }
        }
    }

    public bool Equals(AttributesLookup other)
    {
        if (other == null)
        {
            return false;
        }

        if (originalTypes.Count != other.originalTypes.Count)
        {
            return false;
        }

        for (var i = 0; i < originalTypes.Count; i++)
            if (originalTypes[i] != other.originalTypes[i])
            {
                return false;
            }

        return true;
    }

    public T GetCustomAttribute<T>(MemberInfo member) where T : Attribute
    {
        return GetCustomAttribute<T>(member, false);
    }

    public object GetCustomAttribute(MemberInfo member, Type attributeType)
    {
        return GetCustomAttribute(member, attributeType, false);
    }

    public T GetCustomAttribute<T>(MemberInfo member, bool inherit) where T : Attribute
    {
        return GetCustomAttributes<T>(member, inherit).FirstOrDefault();
    }

    public object GetCustomAttribute(MemberInfo member, Type attributeType, bool inherit)
    {
        return GetCustomAttributes(member, attributeType, inherit).FirstOrDefault();
    }

    public IEnumerable<T> GetCustomAttributes<T>(MemberInfo member) where T : Attribute
    {
        return GetCustomAttributes<T>(member, false);
    }

    public IEnumerable<T> GetCustomAttributes<T>(MemberInfo member, bool inherit) where T : Attribute
    {
        foreach (var attrib in GetCustomAttributes(member, typeof(T), inherit))
        {
            yield return (T)attrib;
        }
    }

    //public IEnumerable<T> GetCustomAttributes<T>(MemberInfo member, bool inherit) where T : Attribute
    //{
    //    var attributeUsage = typeof(T).GetCustomAttribute<AttributeUsageAttribute>();
    //    var visited = new HashSet<T>(DynamicEqualityComparer<T>.Instance);
    //    foreach (var originalMember in GetOriginalMembers(member.Name))
    //    {
    //        foreach (T attribute in originalMember.GetCustomAttributes<T>(inherit))
    //        {
    //            if(visited.Contains(attribute))
    //            {
    //                continue;
    //            }
    //            visited.Add(attribute);
    //            if (attributeUsage == null)
    //            {
    //                yield return attribute;
    //            }
    //            else
    //            {
    //                if (attributeUsage.AllowMultiple)
    //                {
    //                    yield return attribute;
    //                }
    //                else
    //                {
    //                    yield return attribute;
    //                    yield break;
    //                }
    //            }
    //        }
    //    }
    //}
    public IEnumerable<object> GetCustomAttributes(MemberInfo member, Type attributeType, bool inherit)
    {
        foreach (var originalMember in GetOriginalMembers(member.Name))
        {
            var attributes = originalMember.GetCustomAttributes(attributeType, inherit).ToList();
            if (attributes.Count > 0)
            {
                foreach (var attribute in attributes)
                {
                    yield return attribute;
                }

                yield break;
            }
        }
    }

    public IEnumerable<string> GetLinkedMembers(MemberInfo member)
    {
        var customAttributes = GetCustomAttributes<LinkedPropertyAttribute>(member, true);
        foreach (var attribute in customAttributes)
        {
            foreach (var linkedOriginalMember in GetOriginalMembers(attribute.LinkedPropertyName))
            {
                if (linkedOriginalMember == null)
                {
                    continue;
                }

                yield return attribute.LinkedPropertyName;
                break;
            }
        }
    }

    public IEnumerable<string> GetLinkedMembersWithGivenAttribute<T>(MemberInfo member) where T : Attribute
    {
        var customAttributes = GetCustomAttributes<LinkedPropertyAttribute>(member, true);
        foreach (var attribute in customAttributes)
        {
            foreach (var linkedOriginalMember in GetOriginalMembers(attribute.LinkedPropertyName))
            {
                if (linkedOriginalMember == null)
                {
                    continue;
                }

                if (IsDefined<T>(linkedOriginalMember, true))
                {
                    yield return attribute.LinkedPropertyName;
                    break;
                }
            }
        }
    }

    public string GetLinkedMemberWithGivenAttribute<T>(MemberInfo member) where T : Attribute
    {
        return GetLinkedMembersWithGivenAttribute<T>(member).FirstOrDefault();
    }

    public MemberInfo GetMember(string memberName)
    {
        return GetOriginalMembers(memberName).FirstOrDefault();
    }

    public bool IsDefined<T>(MemberInfo member) where T : Attribute
    {
        return IsDefined<T>(member, false);
    }

    public bool IsDefined<T>(MemberInfo member, bool inherit) where T : Attribute
    {
        return GetCustomAttribute<T>(member, inherit) != null;
    }

    private IEnumerable<MemberInfo> GetOriginalMembers(string memberName)
    {
        foreach (var type in originalTypes)
        {
            var members = type.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance);

            if (members?.Length > 0)
            {
                yield return members[0];
            }
        }
    }
}