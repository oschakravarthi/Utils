using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils;

public static class DynamicCopyHelper<TFrom, TTo>
{
    static DynamicCopyHelper()
    {
        Mapper = BuildMapper(null);
        MapperWithPropertyNameCleaning = BuildMapper(ReflectionHelper.MakeValidMemberIdentifier);
    }

    public static Action<TFrom, TTo> Mapper { get; }
    public static Action<TFrom, TTo> MapperWithPropertyNameCleaning { get; }

    [DynamicallyInvoked]
    public static void CopyTo(TFrom source, TTo target)
    {
        CopyTo(source, target, false);
    }

    public static void CopyTo(TFrom source, TTo target, bool cleanPropertyNames)
    {
        if (cleanPropertyNames)
        {
            MapperWithPropertyNameCleaning(source, target);
        }
        else
        {
            Mapper(source, target);
        }
    }

    private static Action<TFrom, TTo> BuildMapper(Func<string, string> propertyNameCleaner)
    {
        var sourceObjectType = typeof(TFrom);
        var targetObjectType = typeof(TTo);
        //var isNetCore = GeneralHelper.IsNetCore();

        var dynamicMethod = new DynamicMethod("CopyTo", typeof(void), [sourceObjectType, targetObjectType],
            typeof(DynamicCopyHelper<TFrom, TTo>));
        var ilGen = dynamicMethod.GetILGenerator();

        var sourceProperties = sourceObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var targetProperties = targetObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var sourceProperty in sourceProperties)
        //if (Attribute.IsDefined(sourceProperty, typeof(DoNotCopyAttribute)))
        //{
        //    continue;
        //}
        {
            if (sourceProperty.CanRead)
            {
                var sourcePropertyName = propertyNameCleaner == null
                    ? sourceProperty.Name
                    : propertyNameCleaner(sourceProperty.Name);
                foreach (var targetProperty in targetProperties)
                {
                    var targetPropertyName = propertyNameCleaner == null
                        ? targetProperty.Name
                        : propertyNameCleaner(targetProperty.Name);
                    if (sourcePropertyName == targetPropertyName)
                    {
                        if (sourceProperty.PropertyType == targetProperty.PropertyType)
                        {
                            var set = false;
                            //if (isNetCore)
                            //{
                            //    var targetField = targetObjectType.GetField(
                            //        $"<{targetPropertyName}>k__BackingField",
                            //        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            //    if (targetField != null)
                            //    {
                            //        ilGen.Emit(OpCodes.Ldarg_1);
                            //        ilGen.Emit(OpCodes.Ldarg_0);
                            //        ilGen.Emit(OpCodes.Callvirt, sourceProperty.GetMethod);
                            //        ilGen.Emit(OpCodes.Stfld, targetField);
                            //        set = true;
                            //    }
                            //}

                            if (!set)
                            {
                                if (targetProperty.CanWrite)
                                {
                                    ilGen.Emit(OpCodes.Ldarg_1);
                                    ilGen.Emit(OpCodes.Ldarg_0);
                                    ilGen.Emit(OpCodes.Callvirt, sourceProperty.GetMethod);
                                    ilGen.Emit(OpCodes.Callvirt, targetProperty.SetMethod);
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        ilGen.Emit(OpCodes.Ret);
        var methodDelegate = dynamicMethod.CreateDelegate<Action<TFrom, TTo>>();
        return methodDelegate;
    }
}