using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

internal static class OpCodeLookupHelper
{
    private static readonly Dictionary<short, OpCode> _opCodesDictionary;

    static OpCodeLookupHelper()
    {
        var fields = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
        _opCodesDictionary = fields.Select(field => (OpCode)field.GetValue(null)).ToDictionary(code => code.Value);
    }

    internal static OpCode GetOpCodeByValue(short value)
    {
        return _opCodesDictionary[value];
    }

    internal static bool TryGetOpCodeByValue(short value, out OpCode code)
    {
        return _opCodesDictionary.TryGetValue(value, out code);
    }
}