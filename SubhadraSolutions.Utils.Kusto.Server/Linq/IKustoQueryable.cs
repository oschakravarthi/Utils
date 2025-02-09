using SubhadraSolutions.Utils.Linq;
using System.Linq;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

public interface IKustoQueryable<out T> : IOrderedQueryable<T>, ICloneableQueryable, IAsyncQueryable<T>
{
}