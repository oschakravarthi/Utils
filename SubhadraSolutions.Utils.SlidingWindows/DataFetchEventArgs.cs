using System;

namespace SubhadraSolutions.Utils.SlidingWindows;

public class DataFetchEventArgs<T>(Window<T> window, DateTime maxTimestamp) : EventArgs
{
    public DateTime MaxTimestamp { get; } = maxTimestamp;
    public Window<T> Window { get; } = window;
}