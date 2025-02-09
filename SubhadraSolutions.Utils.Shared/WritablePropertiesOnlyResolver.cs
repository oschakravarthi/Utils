using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils;

public sealed class WritablePropertiesOnlyResolver : DefaultContractResolver
{
    public static readonly WritablePropertiesOnlyResolver Instance = new();

    private WritablePropertiesOnlyResolver()
    {
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = base.CreateProperties(type, memberSerialization);
        var filtered = props
            .Where(p => type.GetProperty(p.PropertyName).DeclaringType.GetProperty(p.PropertyName).CanWrite).ToList();
        return filtered;
    }
}