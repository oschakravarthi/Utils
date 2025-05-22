using System.Diagnostics;
using System.Xml.Serialization;

namespace SubhadraSolutions.Utils;

[DebuggerDisplay("[{_Item1}, {_Item2}]")]
public class ValuePairImpl<T1, T2>
{
    public ValuePairImpl()
    {
        Item1 = default;
        Item2 = default;
    }

    public ValuePairImpl(T1 item1, T2 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }

    [XmlIgnore] public T1 Item1 { get; set; }

    [XmlIgnore] public T2 Item2 { get; set; }
}