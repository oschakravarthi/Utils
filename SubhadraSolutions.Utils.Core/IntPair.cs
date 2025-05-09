using System;
using System.Diagnostics;
using System.Linq;
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

    [XmlIgnore]
    public int Item1
    {
        get => _Item1;
        set => _Item1 = value;
    }

    [XmlIgnore]
    public int Item2
    {
        get => _Item2;
        set => _Item2 = value;
    }

    [XmlAttribute("X")]
    public string Values
    {
        get => _Item1 + Separator + _Item2;
        set
        {
            if (value != null)
            {
                var numericDigits = "0123456789".ToCharArray();
                var source = value.SkipWhile(ch => numericDigits.Contains(ch));
                source = source.TakeWhile(ch => !numericDigits.Contains(ch));
                Separator = new string(source.ToArray());
                var num = value.IndexOf(Separator);
                if (num >= 0)
                {
                    int.TryParse(value.Substring(0, num), out var result);
                    int.TryParse(value.Substring(num + Separator.Length), out var result2);
                    _Item1 = result;
                    _Item2 = result2;
                }
            }
        }
    }
}