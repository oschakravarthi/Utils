using Kusto.Data.Common;
using Kusto.Data.Exceptions;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SubhadraSolutions.Utils.Kusto.Server;

public class KustoEntitiesEnumerable<T> : IEnumerable<T>
{
    private readonly ClientRequestProperties clientRequestProperties;
    private readonly ICslQueryProvider cslQueryProvider;
    private readonly IEntityBuilderFactory<T> entityBuilderFactory;
    private readonly string query;

    public KustoEntitiesEnumerable(ICslQueryProvider cslQueryProvider, string query,
        ClientRequestProperties clientRequestProperties, IEntityBuilderFactory<T> entityBuilderFactory)
    {
        this.cslQueryProvider = cslQueryProvider;
        this.query = query;
        if (clientRequestProperties == null)
        {
            clientRequestProperties = KustoHelper.DefaultClientRequestProperties;
        }

        this.clientRequestProperties = clientRequestProperties;
        this.entityBuilderFactory = entityBuilderFactory;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var entityBuilder = entityBuilderFactory.CreateBuilder();
        try
        {
            var dataReader = this.cslQueryProvider.ExecuteQuery(query, clientRequestProperties);
            entityBuilder.Initialize(dataReader);
            return new DataReaderToEnumeratorAdapter<T>(dataReader, entityBuilderFactory);
        }
        catch (KustoRequestThrottledException e)
        {
            Debug.WriteLine("QUERY: {0}\r\nException:{1}", query, e);
            throw;
        }
        catch (KustoClientException e)
        {
            Debug.WriteLine("QUERY: {0}\r\nException:{1}", query, e);
            throw;
        }
        catch (Exception e)
        {
            Debug.WriteLine("QUERY: {0}\r\nException:{1}", query, e);
            throw;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}