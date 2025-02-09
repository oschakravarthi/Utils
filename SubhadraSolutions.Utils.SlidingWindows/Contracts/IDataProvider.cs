using SubhadraSolutions.Utils.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.SlidingWindows.Contracts;

public interface IDataProvider<out T> : IUnique
{
    IReadOnlyList<T> GetData(DateTime from, DateTime upto);
}