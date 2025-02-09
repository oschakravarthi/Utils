using System.Collections.Generic;

namespace SubhadraSolutions.Utils;

public class CompositeCompaper<T> : IComparer<T>
{
    private readonly IComparer<T>[] comparers;

    public CompositeCompaper(params IComparer<T>[] comparers)
    {
        this.comparers = comparers;
    }

    public int Compare(T x, T y)
    {
        for (var i = 0; i < comparers.Length; i++)
        {
            var comparer = comparers[i];
            var result = comparer.Compare(x, y);
            if (result != 0)
            {
                return result;
            }
        }

        return 0;
    }
}