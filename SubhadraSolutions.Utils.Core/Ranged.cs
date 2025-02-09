using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils;

public class Ranged<T, I> : Range<T> where T : IComparable<T>
{
    [JsonConstructor]
    public Ranged(T from, T upto, I info) : base(from, upto)
    {
        Info = info;
    }

    public Ranged(Range<T> range, I info) : this(range.From, range.Upto, info)
    {
    }

    [JsonInclude] public I Info { get; private set; }

    public override string ToString()
    {
        return $"{Info}\t{base.ToString()}";
    }
}