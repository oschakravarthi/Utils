using System;
using System.Diagnostics;

namespace SubhadraSolutions.Utils;

[Serializable]
[DebuggerDisplay("[{Item1}, {Item2}]")]
public class ValuePair<T1, T2> : ValuePairImpl<T1, T2>
{
    public ValuePair()
    {
    }

    public ValuePair(T1 item1, T2 item2)
        : base(item1, item2)
    {
    }
}