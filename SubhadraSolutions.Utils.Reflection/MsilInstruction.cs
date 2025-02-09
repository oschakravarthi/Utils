using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

[Serializable]
internal sealed class MsilInstruction
{
    internal MsilInstruction(short opCodeValue, object data, int inline)
    {
        OpCodeValue = opCodeValue;
        Inline = inline;
        if (data != null)
        {
            var mb = data as MethodBase;
            if (mb != null)
            {
                TypeName = mb.DeclaringType.AssemblyQualifiedName;
                MethodCallInfo = new MethodCallInformation();
                AbstractMethodInformation.PopulateMethodBaseInformation(mb, MethodCallInfo);
                MethodCallInfo.MethodName = mb.Name;
            }
            else
            {
                if (data is Type t)
                {
                    TypeName = t.AssemblyQualifiedName;
                }
                else
                {
                    Data = data;
                }
            }
        }
    }

    public object Data { get; }

    internal int Inline { get; set; }
    internal MethodCallInformation MethodCallInfo { get; }

    internal OpCode OpCode => OpCodeLookupHelper.GetOpCodeByValue(OpCodeValue);

    internal short OpCodeValue { get; }

    internal string TypeName { get; set; }
}