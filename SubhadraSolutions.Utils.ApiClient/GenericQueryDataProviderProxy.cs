using Aqua.Dynamic;
using Remote.Linq;
using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.ApiClient.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient
{
    public class GenericQueryDataProviderProxy : AbstractDisposable
    {
        private readonly Func<Remote.Linq.Expressions.Expression, CancellationToken, ValueTask<DynamicObject>> _asyncDataProvider;

        public GenericQueryDataProviderProxy(ILinqDataProvider linqDataProvider, string apiPathPrefix)
        {
            this._asyncDataProvider = (e, c) =>
            {
                return linqDataProvider.PostAsync(apiPathPrefix + "/ExecuteQuery", e, c);
            };
        }

        public IQueryable<T> Get<T>() where T : class
        {
            return RemoteQueryable.Factory.CreateAsyncQueryable<T>(_asyncDataProvider);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}