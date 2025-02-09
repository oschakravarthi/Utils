using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using Remote.Linq.SimpleQuery;
using SubhadraSolutions.Utils.Exposition;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Linq
{
    public class GenericQueryDataProvider<T> : IQueryDataProvider where T : IQueryContext
    {
        private readonly T context;
        private static readonly MethodInfo QueryContextGetTemplateMethod = typeof(IQueryContext).GetMethod(nameof(IQueryContext.Get));

        public GenericQueryDataProvider(T context)
        {
            this.context = context;
        }

        [Expose(httpRequestMethod: HttpRequestMethod.Post)]
        public async ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression)
        {
            var result = queryExpression.Execute(GetQueryable);
            return result;
        }

        private IQueryable GetQueryable(Type type)
        {
            var queryable = QueryContextGetTemplateMethod.MakeGenericMethod(type).Invoke(this.context, Array.Empty<object>());
            return (IQueryable)queryable;
        }
    }
}