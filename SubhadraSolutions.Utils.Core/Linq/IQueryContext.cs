using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Linq
{
    public interface IQueryContext : IDisposable
    {
        IQueryable<T> Get<T>() where T : class;
    }
}