using Aqua.Dynamic;
using Remote.Linq.Expressions;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Linq
{
    public interface IQueryDataProvider
    {
        ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression);
    }
}