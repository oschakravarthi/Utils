using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Linq;

public interface ICacheKeyBuilder
{
    string BuildKey(Expression expression);
}