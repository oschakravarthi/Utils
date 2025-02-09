using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

public static class EmitHelper
{
    [ILEmitter]
    public static void CallSafeInvoke(this ILGenerator ilGen, OpCode codeToPlaceDelegateOnStack,
        OpCode codeToPlaceArgumentArrayOnStack)
    {
        ilGen.Emit(codeToPlaceDelegateOnStack);
        ilGen.Emit(codeToPlaceArgumentArrayOnStack);
        ilGen.Emit(OpCodes.Call, EventHelper.SafeInvokeMethod);
    }

    [ILEmitter]
    public static void CreateArray(this ILGenerator ilGen, Type arrayType, int size, int localVariableIndexToSave)
    {
        ilGen.Emit(OpCodes.Ldc_I4, size);
        ilGen.Emit(OpCodes.Newarr, arrayType);
        ilGen.Emit(OpCodes.Stloc, localVariableIndexToSave);
    }

    [ILEmitter]
    public static void InitializeLocal(LocalBuilder local, ILGenerator ilGen)
    {
        var type = local.LocalType;

        //var opcode = local.LocalIndex > 254 ? OpCodes.Stloc : OpCodes.Stloc_S;
        var opcode = OpCodes.Stloc;
        if (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
        {
            ilGen.Emit(OpCodes.Ldnull);
            ilGen.Emit(opcode, local);
            return;
        }

        if (type == typeof(bool))
        {
            ilGen.Emit(OpCodes.Ldc_I4, 0);
            ilGen.Emit(opcode, local);
            return;
        }

        if (type == typeof(char))
        {
            ilGen.Emit(OpCodes.Ldc_I4, 0);
            ilGen.Emit(opcode, local);
            return;
        }

        if (type == typeof(Guid))
        {
            ilGen.Emit(OpCodes.Ldloca, local);
            ilGen.Emit(OpCodes.Initobj, type);
            return;
        }

        if (type == typeof(DateTime) || type == typeof(decimal))
        {
            //var op= local.LocalIndex > 254 ? OpCodes.Ldloca : OpCodes.Ldloca_S
            var op = OpCodes.Ldloca;
            ilGen.Emit(op, local);
            ilGen.Emit(OpCodes.Initobj, type);
            return;
        }

        if (type == typeof(float))
        {
            ilGen.Emit(OpCodes.Ldc_R4, 0.0f);
            ilGen.Emit(opcode, local);
            return;
        }

        if (type == typeof(double))
        {
            ilGen.Emit(OpCodes.Ldc_R8, 0.0);
            ilGen.Emit(opcode, local);
            return;
        }

        if (type == typeof(short) || type == typeof(ushort))
        {
            ilGen.Emit(OpCodes.Ldc_I4, (short)0);
            ilGen.Emit(opcode, local);
            return;
        }

        if (type == typeof(int) || type == typeof(uint) || type == typeof(char))
        {
            ilGen.Emit(OpCodes.Ldc_I4, 0);
            ilGen.Emit(opcode, local);
            return;
        }

        if (type == typeof(long) || type == typeof(ulong))
        {
            ilGen.Emit(OpCodes.Ldc_I8, 0L);
            ilGen.Emit(opcode, local);
            return;
        }

        throw new ArgumentException("Don't know how to initialize Type:" + type, nameof(local));
        //ilGen.Emit(OpCodes.Ldc_I4_0);

        //if (type == typeof(long) || type == typeof(ulong))
        //{
        //    ilGen.Emit(OpCodes.Conv_I8);
        //}

        //ilGen.Emit(opcode, local);
    }

    [ILEmitter]
    public static void ReplaceArrayElement(this ILGenerator ilGen, OpCode codeToPlaceArrayOnStack, int index,
        OpCode codeToPlaceElementOnStack)
    {
        ilGen.ReplaceArrayElementOfReferenceType(codeToPlaceArrayOnStack, index,
            () => ilGen.Emit(codeToPlaceElementOnStack));
    }

    [ILEmitter]
    public static void ReplaceArrayElementOfReferenceType(this ILGenerator ilGen, OpCode codeToPlaceArrayOnStack,
        int index, Action actionToPlaceElementOnStack)
    {
        ilGen.Emit(codeToPlaceArrayOnStack);
        ilGen.Emit(OpCodes.Ldc_I4, index);
        actionToPlaceElementOnStack();
        ilGen.Emit(OpCodes.Stelem_Ref);
    }
}