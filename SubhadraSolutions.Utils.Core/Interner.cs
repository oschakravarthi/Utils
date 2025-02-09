using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils;

public static class Interner<T>
{
    private static Action<T, T> action;

    static Interner()
    {
        BuildDynamicMethod();
    }

    public static void Intern(T a, T b)
    {
        action(a, b);
    }

    public static void Intern(IEnumerable<T> objects)
    {
        using var enumerator = objects.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }

        var previous = enumerator.Current;

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            Intern(previous, current);
            previous = current;
        }
    }

    [DynamicallyInvoked]
    public static IEnumerable<T> WrapIntern(IEnumerable<T> objects)
    {
        using var enumerator = objects.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            yield break;
        }

        var previous = enumerator.Current;
        yield return previous;

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            Intern(previous, current);
            yield return current;
            previous = current;
        }
    }

    [ILEmitter]
    private static void BuildDynamicMethod()
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertiesCount = properties.Length;
        var dynamicMethod = new DynamicMethod("populateEntityFromRecordDynamicMethod", typeof(void),
            [type, type], typeof(Interner<T>));
        var ilGen = dynamicMethod.GetILGenerator();

        ilGen.DeclareLocal(typeof(string));
        ilGen.DeclareLocal(typeof(string));
        ilGen.DeclareLocal(typeof(bool));
        var isNetCore = GeneralHelper.IsNetCore();
        for (var i = 0; i < propertiesCount; i++)
        {
            var property = properties[i];

            if (property.PropertyType != typeof(string))
            {
                continue;
            }

            if (property.GetCustomAttribute<NotInternableAttribute>(true) != null)
            {
                continue;
            }

            FieldInfo field = null;
            if (isNetCore)
            {
                field = type.GetField($"<{property.Name}>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }

            if (field == null)
            {
                if (!property.CanWrite)
                {
                    continue;
                }
            }

            ilGen.Emit(OpCodes.Ldarg, 0);
            if (field != null)
            {
                ilGen.Emit(OpCodes.Ldfld, field);
            }
            else
            {
                ilGen.Emit(OpCodes.Callvirt, property.GetGetMethod());
            }

            ilGen.Emit(OpCodes.Stloc, 0);

            ilGen.Emit(OpCodes.Ldarg, 1);
            if (field != null)
            {
                ilGen.Emit(OpCodes.Ldfld, field);
            }
            else
            {
                ilGen.Emit(OpCodes.Callvirt, property.GetGetMethod());
            }

            ilGen.Emit(OpCodes.Stloc, 1);

            ilGen.Emit(OpCodes.Ldloc, 0);
            ilGen.Emit(OpCodes.Ldloc, 1);
            ilGen.Emit(OpCodes.Call, SharedReflectionInfo.StringEqualsMethod);
            ilGen.Emit(OpCodes.Stloc, 2);

            var label = ilGen.DefineLabel();

            ilGen.Emit(OpCodes.Ldloc, 2);
            ilGen.Emit(OpCodes.Brfalse, label);

            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Ldloc, 0);
            if (field != null)
            {
                ilGen.Emit(OpCodes.Stfld, field);
            }
            else
            {
                ilGen.Emit(OpCodes.Callvirt, property.GetSetMethod());
            }

            ilGen.MarkLabel(label);
        }

        ilGen.Emit(OpCodes.Ret);
        action = (Action<T, T>)dynamicMethod.CreateDelegate(typeof(Action<T, T>));
    }
}