using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public interface IGenericQueryableFactory<out T>
{
    IQueryable<T> GetQueryable();
}