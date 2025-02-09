using Kusto.Data.Common;
using Kusto.Data.Results;
using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Net.Http;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Kusto.Server.RateLimited
{
    public class CslQueryProviderRateLimitDecorator : AbstractDisposable, ICslQueryProvider
    {
        private readonly ICslQueryProvider actual;
        private readonly RateLimiterWrapper rateLimiter;

        public CslQueryProviderRateLimitDecorator(ICslQueryProvider actual, RateLimiterWrapper rateLimiter)
        {
            this.actual = actual;
            this.rateLimiter = rateLimiter;
        }

        public string DefaultDatabaseName
        {
            get => actual.DefaultDatabaseName;
            set => actual.DefaultDatabaseName = value;
        }

        public IDataReader ExecuteQuery(string databaseName, string query, ClientRequestProperties properties)
        {
            var claim = rateLimiter.AcquireAsync(CancellationToken.None).Result;
            try
            {
                var reader = actual.ExecuteQuery(databaseName, query, properties);
                return new DataReaderDisposablesDecorator(reader, claim);
            }
            catch (Exception ex)
            {
                claim.Dispose();
                throw;
            }
        }

        public IDataReader ExecuteQuery(string query, ClientRequestProperties properties)
        {
            var claim = rateLimiter.AcquireAsync(CancellationToken.None).Result;
            try
            {
                var reader = this.actual.ExecuteQuery(query, properties);
                return new DataReaderDisposablesDecorator(reader, claim);
            }
            catch (Exception ex)
            {
                GeneralHelper.DisposeIfDisposable(claim);
                throw;
            }
        }

        public IDataReader ExecuteQuery(string query)
        {
            var claim = rateLimiter.AcquireAsync(CancellationToken.None).Result;
            try
            {
                var reader = actual.ExecuteQuery(query);
                return new DataReaderDisposablesDecorator(reader, claim);
            }
            catch (Exception ex)
            {
                GeneralHelper.DisposeIfDisposable(claim);
                throw;
            }
        }

        public async Task<IDataReader> ExecuteQueryAsync(string databaseName, string query, ClientRequestProperties properties, CancellationToken cancellationToken = default)
        {
            var claim = await rateLimiter.AcquireAsync(CancellationToken.None).ConfigureAwait(false);
            try
            {
                var reader = await actual.ExecuteQueryAsync(databaseName, query, properties, cancellationToken).ConfigureAwait(false);
                return new DataReaderDisposablesDecorator(reader, claim);
            }
            catch (Exception ex)
            {
                GeneralHelper.DisposeIfDisposable(claim);
                throw;
            }
        }

        public async Task<ProgressiveDataSet> ExecuteQueryV2Async(string databaseName, string query, ClientRequestProperties properties, CancellationToken cancellationToken = default)
        {
            var claim = await rateLimiter.AcquireAsync(CancellationToken.None).ConfigureAwait(false);
            try
            {
                var result = await actual.ExecuteQueryV2Async(databaseName, query, properties, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                GeneralHelper.DisposeIfDisposable(claim);
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            actual.Dispose();
        }
    }
}