using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public interface IQueryableProvider
{
    IQueryable GetQueryable();
}