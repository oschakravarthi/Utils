using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.SlidingWindows;

public class Window<T>(DateTime from, DateTime upto, IReadOnlyList<T> items)
{
    public TimeSpan Duration => Upto - From;

    public DateTime From { get; } = from;
    public IReadOnlyList<T> Items { get; } = items;
    public DateTime Upto { get; } = upto;
}