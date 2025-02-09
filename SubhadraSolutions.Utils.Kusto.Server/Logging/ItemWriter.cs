using Kusto.Ingest;
using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Collections.Concurrent;
using SubhadraSolutions.Utils.Logging;
using System;
using System.Threading;

namespace SubhadraSolutions.Utils.Kusto.Server.Logging;

public class ItemWriter<T> : AbstractDisposable, IItemWriter<T>
{
    private readonly IKustoIngestClient ingestClient;
    private readonly string databaseName;
    private readonly ConcurrentStore<T> store;
    private readonly string tableName;

    public ItemWriter(IKustoIngestClient ingestClient, string databaseName, string tableName) : this(
        ingestClient, databaseName, tableName,
        new ConcurrentStore<T>(100, ThreadPriority.Normal, TimeSpan.FromSeconds(60)))
    {
    }

    public ItemWriter(IKustoIngestClient ingestClient, string databaseName, string tableName, ConcurrentStore<T> store)
    {
        this.ingestClient = ingestClient;
        this.databaseName = databaseName;
        this.tableName = tableName;
        this.store = store;
        if (store != null)
        {
            this.store.OnStoreFlush += Store_OnStoreFlush;
        }
    }

    public void Write(T item)
    {
        CheckAndThrowDisposedException();
        if (store != null)
        {
            store.Add(item);
        }
        else
        {
            KustoIngestionHelper.Ingest(item, this.ingestClient, this.databaseName, tableName);
        }
    }

    protected override void Dispose(bool disposing)
    {
        store?.Dispose();
    }

    private void Store_OnStoreFlush(object sender, StoreFlushEventArgs<T> e)
    {
        KustoIngestionHelper.IngestObjects(e, this.ingestClient, this.databaseName, tableName);
    }
}