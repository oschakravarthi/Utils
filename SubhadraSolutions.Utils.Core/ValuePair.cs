using System;
using System.Diagnostics;
using System.Xml.Serialization;

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

    [XmlAttribute("A")]
    public T1 Item1
    {
        get => _Item1;
        set => _Item1 = value;
    }

    [XmlAttribute("B")]
    public T2 Item2
    {
        get => _Item2;
        set => _Item2 = value;
    }
}