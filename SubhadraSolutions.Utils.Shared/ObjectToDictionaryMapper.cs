using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils;

public static class ObjectToDictionaryMapper<T>
{
    private static readonly MethodInfo dictionaryAddMethod = typeof(IDictionary<string, object>).GetMethod("Add");

    private static Action<T, IDictionary<string, object>> populateDictionaryWithPropertiesMethod;

    static ObjectToDictionaryMapper()
    {
        BuildCreateEntityFromRecordDynamicMethod();
    }

    public static IEnumerable<Dictionary<string, object>> BuildDictionariesFromObjects(IEnumerable<T> objects)
    {
        foreach (var obj in objects)
        {
            yield return BuildDictionary(obj);
        }
    }

    public static Dictionary<string, object> BuildDictionary(T obj)
    {
        var dictionary = new Dictionary<string, object>();
        Populate(obj, dictionary);
        return dictionary;
    }

    public static void Populate(T obj, IDictionary<string, object> dictionary)
    {
        populateDictionaryWithPropertiesMethod(obj, dictionary);
    }

    [ILEmitter]
    private static void BuildCreateEntityFromRecordDynamicMethod()
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0).ToArray();

        var propertiesCount = properties.Length;
        var dm = new DynamicMethod("populateDictionaryWithProperties", typeof(void),
            [type, typeof(IDictionary<string, object>)], typeof(ObjectToDictionaryMapper<T>));
        var ilGen = dm.GetILGenerator();

        for (var i = 0; i < propertiesCount; i++)
        {
            var property = properties[i];

            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Ldstr, property.Name);

            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Callvirt, property.GetGetMethod());

            if (property.PropertyType.IsValueType)
            {
                ilGen.Emit(OpCodes.Box, property.PropertyType);
            }

            ilGen.Emit(OpCodes.Callvirt, dictionaryAddMethod);
        }

        ilGen.Emit(OpCodes.Ret);
        populateDictionaryWithPropertiesMethod = dm.CreateDelegate<Action<T, IDictionary<string, object>>>();
    }

    private delegate void PopulateDictionaryWithPropertiesDelegate(T obj, IDictionary<string, object> dictionary);
}