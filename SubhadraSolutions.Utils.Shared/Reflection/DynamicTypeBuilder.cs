using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

public static class DynamicTypeBuilder
{
    [ILEmitter]
    public static Type BuildType(ModuleBuilder mb, string className, IEnumerable<Tuple<string, Type>> properties)
    {
        var tb = mb.DefineType(className, TypeAttributes.Public);

        var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);

        var ctor1IL = ctor.GetILGenerator();
        ctor1IL.Emit(OpCodes.Ldarg, 0);
        ctor1IL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
        ctor1IL.Emit(OpCodes.Ret);

        foreach (var prop in properties)
        {
            var propName = prop.Item1;
            var propType = prop.Item2;
            var fieldIndex = tb.DefineField("m_" + propName, propType, FieldAttributes.Private);
            var pb = tb.DefineProperty(propName, PropertyAttributes.HasDefault, propType, null);

            const MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            var getAccessor = tb.DefineMethod("get_" + propName, getSetAttr, propType, Type.EmptyTypes);
            var getIL = getAccessor.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg, 0);
            getIL.Emit(OpCodes.Ldfld, fieldIndex);
            getIL.Emit(OpCodes.Ret);

            var setAccessor = tb.DefineMethod("set_" + propName, getSetAttr, null, [propType]);

            var setIL = setAccessor.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg, 0);
            setIL.Emit(OpCodes.Ldarg, 1);
            setIL.Emit(OpCodes.Stfld, fieldIndex);
            setIL.Emit(OpCodes.Ret);

            pb.SetGetMethod(getAccessor);
            pb.SetSetMethod(setAccessor);
        }

        return tb.CreateType();
    }
}