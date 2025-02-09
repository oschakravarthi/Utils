using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq;

public interface ICloneableQueryable : IQueryable
{
    ICloneableQueryable Clone(Expression expression);
}