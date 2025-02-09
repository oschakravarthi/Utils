using SubhadraSolutions.Utils.Linq;
using System;

namespace SubhadraSolutions.Utils.Data.EntityFramework
{
    public class QueryContextFactory : IQueryContextFactory
    {
        private readonly Func<IQueryContext> func;

        public QueryContextFactory(Func<IQueryContext> func)
        {
            this.func = func;
        }

        public IQueryContext CreateQueryContext()
        {
            return func();
        }
    }
}