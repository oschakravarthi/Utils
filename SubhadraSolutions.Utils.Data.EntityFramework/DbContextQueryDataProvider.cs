using Aqua.Dynamic;
using Microsoft.EntityFrameworkCore;
using Remote.Linq.EntityFrameworkCore;
using Remote.Linq.Expressions;
using SubhadraSolutions.Utils.Exposition;
using SubhadraSolutions.Utils.Linq;
using System;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Data.EntityFramework
{
    public class DbContextQueryDataProvider<T> : IQueryDataProvider where T : DbContext
    {
        private readonly Func<T> dbContextFactory;

        public DbContextQueryDataProvider(Func<T> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        [Expose(httpRequestMethod: HttpRequestMethod.Post)]
        public async ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression)
        {
            using var dbContext = this.dbContextFactory();
            try
            {
                return await queryExpression.ExecuteWithEntityFrameworkCoreAsync(dbContext).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                throw;
            }
        }
    }
}