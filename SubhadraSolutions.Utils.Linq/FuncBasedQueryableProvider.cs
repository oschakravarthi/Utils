using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public class FuncBasedQueryableProvider(Func<IQueryable> func) : IQueryableProvider
{
    public IQueryable GetQueryable()
    {
        return func();
    }
}