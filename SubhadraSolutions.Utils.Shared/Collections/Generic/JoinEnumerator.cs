using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class JoinEnumerator<T>(List<IEnumerator<T>> enumerators) : AbstractDisposable, IEnumerator<T>
{
    private int _currentIndex;

    object IEnumerator.Current => enumerators[_currentIndex].Current;

    T IEnumerator<T>.Current => enumerators[_currentIndex].Current;

    public bool MoveNext()
    {
        if (enumerators[_currentIndex].MoveNext())
        {
            return true;
        }

        enumerators[_currentIndex].Dispose();
        _currentIndex++;
        if (_currentIndex >= enumerators.Count)
        {
            return false;
        }

        return MoveNext();
    }

    public void Reset()
    {
    }

    protected override void Dispose(bool disposing)
    {
    }
}