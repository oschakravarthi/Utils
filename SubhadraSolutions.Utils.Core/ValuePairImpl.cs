using System.Diagnostics;
using System.Xml.Serialization;

namespace SubhadraSolutions.Utils;

[DebuggerDisplay("[{_Item1}, {_Item2}]")]
public class ValuePairImpl<T1, T2>
{
    public ValuePairImpl()
    {
        _Item1 = default;
        _Item2 = default;
    }

    public ValuePairImpl(T1 item1, T2 item2)
    {
        _Item1 = item1;
        _Item2 = item2;
    }

    [XmlIgnore] public T1 _Item1 { get; set; }

    [XmlIgnore] public T2 _Item2 { get; set; }
}