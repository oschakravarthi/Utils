using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SubhadraSolutions.Utils;

[Serializable]
[DebuggerDisplay("[{Values}]")]
public class IntPair : ValuePairImpl<int, int>
{
    [XmlIgnore] public string Separator = "|";

    public IntPair()
    {
    }

    public IntPair(int item1, int item2)
        : base(item1, item2)
    {
    }
}