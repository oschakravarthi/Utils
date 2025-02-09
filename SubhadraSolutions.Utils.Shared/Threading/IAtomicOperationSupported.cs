using System;

namespace SubhadraSolutions.Utils.Threading;

public interface IAtomicOperationSupported
{
    void PerformAtomicOperation(Action action);
}