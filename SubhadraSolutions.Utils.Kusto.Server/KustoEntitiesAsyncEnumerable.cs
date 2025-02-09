using Kusto.Data.Common;
using SubhadraSolutions.Utils.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Kusto.Server;

public class KustoEntitiesAsyncEnumerable<T>(ICslQueryProvider cslQueryProvider, string query,
        ClientRequestProperties clientRequestProperties, IEntityBuilderFactory<T> entityBuilderFactory)
    : KustoEntitiesAsyncEnumerableAndEnumerator(cslQueryProvider, query, clientRequestProperties),
        IAsyncEnumerable<T>
{
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var entityBuilder = entityBuilderFactory.CreateBuilder();
        return new KustoEntitiesAsyncEnumerator(cslQueryProvider, query, clientRequestProperties, entityBuilder);
    }

    private sealed class KustoEntitiesAsyncEnumerator(ICslQueryProvider cslQueryProvider, string query,
            ClientRequestProperties clientRequestProperties, IEntityBuilder<T> entityBuilder)
        : KustoEntitiesAsyncEnumerableAndEnumerator(cslQueryProvider, query, clientRequestProperties),
            IAsyncEnumerator<T>
    {
        private IDataReader dataReader;
        private ICslQueryProvider queryProvider;

        // private Task<IDataReader> task = null;

        public T Current { get; private set; }

        public ValueTask DisposeAsync()
        {
            GeneralHelper.DisposeIfDisposables(dataReader, queryProvider, entityBuilder);

            dataReader = null;
            queryProvider = null;
            entityBuilder = null;

            return ValueTask.CompletedTask;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            try
            {
                if (dataReader == null)
                {
                    dataReader = await queryProvider
                        .ExecuteQueryAsync(cslQueryProvider.DefaultDatabaseName, query, clientRequestProperties)
                        .ConfigureAwait(false);
                    entityBuilder.Initialize(dataReader);
                }

                if (!dataReader.Read())
                {
                    await DisposeAsync().ConfigureAwait(false);
                    return false;
                }

                Current = entityBuilder.BuildEntityFromCurrent();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                await DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}

public abstract class KustoEntitiesAsyncEnumerableAndEnumerator(ICslQueryProvider cslQueryProvider, string query,
    ClientRequestProperties clientRequestProperties)
{
    protected readonly ClientRequestProperties clientRequestProperties = clientRequestProperties;
    protected readonly ICslQueryProvider cslQueryProvider = cslQueryProvider;
    protected readonly string query = query;
}