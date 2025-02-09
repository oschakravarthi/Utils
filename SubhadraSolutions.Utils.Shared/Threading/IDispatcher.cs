using System;

namespace SubhadraSolutions.Utils.Threading;

public interface IDispatcher
{
    void Dispatch(Action action);
}