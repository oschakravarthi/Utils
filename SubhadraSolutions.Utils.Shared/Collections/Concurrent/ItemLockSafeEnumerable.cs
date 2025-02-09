using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class ItemLockSafeEnumerable<T>(IEnumerable<T> enumerable) : IEnumerable<T>
    where T : class
{
    public IEnumerator<T> GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumeratorCore();
    }

    private IEnumerator<T> GetEnumeratorCore()
    {
        return new ItemLockSafeEnumerator(enumerable.GetEnumerator());
    }

    private sealed class ItemLockSafeEnumerator(IEnumerator<T> enumerator) : AbstractDisposable, IEnumerator<T>
    {
        private readonly T previous = null;

        object IEnumerator.Current => enumerator.Current;

        T IEnumerator<T>.Current => enumerator.Current;

        public bool MoveNext()
        {
            var canMove = enumerator.MoveNext();
            if (previous != null)
            {
                Monitor.Exit(previous);
            }

            if (canMove)
            {
                Monitor.Enter(enumerator.Current);
            }

            return canMove;
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        ~ItemLockSafeEnumerator()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (enumerator.Current != null)
            {
                Monitor.Exit(enumerator.Current);
            }

            enumerator.Dispose();
        }
    }
}