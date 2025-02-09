using System;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class SizedQueueOnFullEventArgs<T>(T item) : EventArgs
{
    public T Item { get; private set; } = item;
}