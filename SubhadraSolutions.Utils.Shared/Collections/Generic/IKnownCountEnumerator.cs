using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public interface IKnownCountEnumerator<out T> : IEnumerator<T>
{
    int Count { get; }
}