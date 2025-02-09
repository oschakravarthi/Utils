using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Reflection;

namespace SubhadraSolutions.Utils.EventProcessing;

public class StaticMemberDataSource : IDataSource<object>
{
    public string MemberPath { get; set; }

    public object GetData()
    {
        //TODO: access thru singletons and nested types;
        var index = MemberPath.LastIndexOf('.');
        var typeName = MemberPath[..index];
        var type = TypesLookupHelper.GetType(typeName);
        if (type != null)
        {
            var memberName = MemberPath[(index + 1)..];
            var property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static);
            if (property != null)
            {
                return property.GetValue(null);
            }

            var method = type.GetMethod(memberName, BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                return method.Invoke(null, []);
            }
        }

        throw new ApplicationException("There is no such static member: " + MemberPath);
    }
}