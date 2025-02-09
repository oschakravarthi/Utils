using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class FlushEventArgs<T>(ICollection<T> items) : EventArgs
{
    public ICollection<T> Items { get; } = items;
}