using SubhadraSolutions.Utils.Abstractions;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

internal sealed class MsilReader : AbstractDisposable
{
    private readonly BinaryReader _methodReader;

    private readonly Module _module;

    private readonly MethodBase method;

    internal MsilReader(MethodBase method)
    {
        //Guard.ArgumentShouldNotBeNull(method, nameof(method));
        this.method = method;
        _module = method.Module;
        _methodReader = new BinaryReader(new MemoryStream(method.GetMethodBody().GetILAsByteArray()));
    }

    internal MsilInstruction Current { get; private set; }

    internal static MethodInformation BuildMethodInformation(MethodInfo method)
    {
        var info = new MethodInformation();
        AbstractMethodInformation.PopulateMethodBaseInformation(method, info);
        info.ReturnType = method.ReturnType.AssemblyQualifiedName;
        var body = method.GetMethodBody();
        var implicitParametersCount = method.IsStatic ? 0 : 1;
        info.LocalVariableTypes = new string[body.LocalVariables.Count + implicitParametersCount];
        for (var i = 0; i < body.LocalVariables.Count; i++)
            info.LocalVariableTypes[body.LocalVariables[i].LocalIndex + implicitParametersCount] =
                body.LocalVariables[i].LocalType.AssemblyQualifiedName;

        if (implicitParametersCount == 1)
        {
            info.LocalVariableTypes[0] = method.DeclaringType.AssemblyQualifiedName;
        }

        var reader = new MsilReader(method);
        while (reader.Read()) info.Instructions.Add(reader.Current);

        return info;
    }

    internal bool Read()
    {
        if (_methodReader.BaseStream.Length == _methodReader.BaseStream.Position)
        {
            return false;
        }

        var index = (int)_methodReader.BaseStream.Position;
        int instructionValue;

        if (_methodReader.BaseStream.Length - 1 == _methodReader.BaseStream.Position)
        {
            instructionValue = _methodReader.ReadByte();
        }
        else
        {
            instructionValue = _methodReader.ReadUInt16();
            if ((instructionValue & OpCodes.Prefix1.Value) != OpCodes.Prefix1.Value)
            {
                instructionValue &= 0xff;
                _methodReader.BaseStream.Position--;
            }
            else
            {
                instructionValue = ((0xFF00 & instructionValue) >> 8) | ((0xFF & instructionValue) << 8);
            }
        }

        if (!OpCodeLookupHelper.TryGetOpCodeByValue((short)instructionValue, out var code))
        {
            throw new InvalidProgramException();
        }

        var dataSize = GetSize(code.OperandType);
        var data = new byte[dataSize];
        _methodReader.Read(data, 0, dataSize);

        var objData = GetData(_module, code, data, out var inline);

        Current = new MsilInstruction(code.Value, objData, inline);
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        _methodReader.Dispose();
    }

    private static object GetData(Module module, OpCode code, byte[] rawData, out int inline)
    {
        inline = 0;
        if (code.OperandType == OperandType.InlineNone)
        {
            return null;
        }

        object data = null;
        switch (code.OperandType)
        {
            case OperandType.InlineField:
                data = module.ResolveField(BitConverter.ToInt32(rawData, 0));
                break;

            case OperandType.InlineBrTarget:
            case OperandType.InlineSwitch:
            case OperandType.InlineI:
                data = BitConverter.ToInt32(rawData, 0);
                break;

            case OperandType.InlineI8:
                data = BitConverter.ToInt64(rawData, 0);
                break;

            case OperandType.InlineMethod:
                data = module.ResolveMethod(BitConverter.ToInt32(rawData, 0));
                break;

            case OperandType.InlineR:
                data = BitConverter.ToDouble(rawData, 0);
                break;

            case OperandType.InlineSig:
                inline = BitConverter.ToInt32(rawData, 0);
                data = module.ResolveSignature(inline);
                break;

            case OperandType.InlineString:
                inline = BitConverter.ToInt32(rawData, 0);
                data = module.ResolveString(inline);
                break;

            case OperandType.InlineTok:
            case OperandType.InlineType:
                data = module.ResolveType(BitConverter.ToInt32(rawData, 0));
                break;

            case OperandType.InlineVar:
                data = BitConverter.ToInt16(rawData, 0);
                break;

            case OperandType.ShortInlineVar:
            case OperandType.ShortInlineI:
            case OperandType.ShortInlineBrTarget:
                data = rawData[0];
                break;

            case OperandType.ShortInlineR:
                data = BitConverter.ToSingle(rawData, 0);
                break;
        }

        return data;
    }

    private static int GetSize(OperandType opType)
    {
        switch (opType)
        {
            case OperandType.InlineNone:
                return 0;

            case OperandType.ShortInlineBrTarget:
            case OperandType.ShortInlineI:
            case OperandType.ShortInlineVar:
                return 1;

            case OperandType.InlineVar:
                return 2;

            case OperandType.InlineBrTarget:
            case OperandType.InlineField:
            case OperandType.InlineI:
            case OperandType.InlineMethod:
            case OperandType.InlineSig:
            case OperandType.InlineString:
            case OperandType.InlineSwitch:
            case OperandType.InlineTok:
            case OperandType.InlineType:
            case OperandType.ShortInlineR:

                return 4;

            case OperandType.InlineI8:
            case OperandType.InlineR:

                return 8;

            default:
                return 0;
        }
    }

    private static void WriteInstruction(MsilInstruction ins, BinaryWriter writer)
    {
        var code = ins.OpCode;
        writer.Write(code.Value);
        switch (code.OperandType)
        {
            case OperandType.InlineField:
                var fieldInfo = (FieldInfo)ins.Data;
                writer.Write(fieldInfo.MetadataToken);
                break;

            case OperandType.InlineBrTarget:
            case OperandType.InlineSwitch:
            case OperandType.InlineI:
                var i = (int)ins.Data;
                writer.Write(i);
                break;

            case OperandType.InlineI8:
                var l = (long)ins.Data;
                writer.Write(l);
                break;

            case OperandType.InlineMethod:
                var methodbase = (MethodBase)ins.Data;
                writer.Write(methodbase.MetadataToken);
                break;

            case OperandType.InlineR:
                var d = (double)ins.Data;
                writer.Write(d);
                break;

            case OperandType.InlineSig:
                writer.Write(ins.Inline);
                //data = module.ResolveSignature(BitConverter.ToInt32(rawData, 0));
                break;

            case OperandType.InlineString:
                writer.Write(ins.Inline);
                //data = module.ResolveString(BitConverter.ToInt32(rawData, 0));
                break;

            case OperandType.InlineTok:
            case OperandType.InlineType:
                var t = (Type)ins.Data;
                writer.Write(t.MetadataToken);
                break;

            case OperandType.InlineVar:
                var s = (short)ins.Data;
                writer.Write(s);
                break;

            case OperandType.ShortInlineVar:
            case OperandType.ShortInlineI:
            case OperandType.ShortInlineBrTarget:
                var b = (byte)ins.Data;
                writer.Write(b);
                break;

            case OperandType.ShortInlineR:
                var f = (float)ins.Data;
                writer.Write(f);
                break;
        }
    }
}