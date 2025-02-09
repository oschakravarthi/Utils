using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public class MetadataSupportedQueryableLookupImpl(IQueryableLookup queryableLookup) : IMetadataSupportedQueryableLookup
{
    private readonly List<DatabaseAndTables> allDatabasesAndTables = [];

    private readonly Dictionary<Type, LookupEntry> lookup = [];

    private readonly Dictionary<DatabaseAndTable, LookupEntry> reverseLookup = [];

    public Type GetElementType(DatabaseAndTable databaseAndTable)
    {
        reverseLookup.TryGetValue(databaseAndTable, out var entry);
        if (entry == null)
        {
            return null;
        }

        return entry.ElementType;
    }

    public IEnumerator<DatabaseAndTables> GetEnumerator()
    {
        return allDatabasesAndTables.GetEnumerator();
    }

    public IQueryable<T> GetQueryable<T>()
    {
        var type = typeof(T);
        var queryable = GetQueryable(type);
        return (IQueryable<T>)queryable;
    }

    public IQueryable GetQueryable(Type entityType)
    {
        if (!lookup.TryGetValue(entityType, out var entry))
        {
            return null;
        }

        return entry.QueryableCreator();
    }

    public IQueryable GetQueryable(DatabaseAndTable databaseAndTable)
    {
        reverseLookup.TryGetValue(databaseAndTable, out var entry);
        if (entry == null)
        {
            return null;
        }

        return entry.QueryableCreator();
    }

    [DynamicallyInvoked]
    public void RegisterQueryableFactory(Func<IQueryable> queryableFactory, Type elementType, DatabaseAndTable table,
        List<ColumnMetaRecord> columns)
    {
        var method = typeof(MetadataSupportedQueryableLookupImpl).GetMethods()
            .First(m => m.Name == nameof(RegisterQueryableFactory) && m.IsGenericMethod);
        var genericMethod = method.MakeGenericMethod(elementType);
        genericMethod.Invoke(this, [queryableFactory, table, columns]);
    }

    [DynamicallyInvoked]
    public void RegisterQueryableFactory<T>(Func<IQueryable<T>> queryableFactory, DatabaseAndTable table,
        List<ColumnMetaRecord> columns)
    {
        //var databaseMetaRecord = new DatabaseMetaRecord { Cluster = table.ClusterName, Name = table.DatabaseName };
        var databaseMetaRecord = new DatabaseMetaRecord { Name = table.DatabaseName };
        var tableAndColumns = new TableAndColumns { TableName = table.TableName, ColumnsMetadata = columns };

        var entry = new LookupEntry(databaseMetaRecord, tableAndColumns, queryableFactory, typeof(T));

        var type = typeof(T);
        lookup[type] = entry;
        queryableLookup.RegisterQueryableFactory(queryableFactory);
        var databaseAndTable = new DatabaseAndTable
        {
            //ClusterName = databaseMetaRecord.Cluster, DatabaseName = databaseMetaRecord.Name,
            TableName = tableAndColumns.TableName
        };
        reverseLookup.Add(databaseAndTable, entry);

        for (var i = 0; i < allDatabasesAndTables.Count; i++)
        {
            var db = allDatabasesAndTables[i];
            var existingDatabaseMetaRecord = new DatabaseMetaRecord
            { Cluster = db.ClusterName, Name = db.DatabaseName };

            if (existingDatabaseMetaRecord.Equals(databaseMetaRecord))
            {
                db.Tables.Add(tableAndColumns);
                return;
            }
        }

        var newDb = new DatabaseAndTables
        {
            ClusterName = databaseMetaRecord.Cluster,
            DatabaseName = databaseMetaRecord.Name
        };

        newDb.Tables.Add(tableAndColumns);
        allDatabasesAndTables.Add(newDb);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private sealed class LookupEntry(DatabaseMetaRecord databaseMetadata, TableAndColumns tableAndColumns,
        Func<IQueryable> queryableCreator, Type elementType)
    {
        public DatabaseMetaRecord DatabaseMetadata { get; } = databaseMetadata;
        public Type ElementType { get; } = elementType;
        public Func<IQueryable> QueryableCreator { get; } = queryableCreator;
        public TableAndColumns TableAndColumns { get; } = tableAndColumns;
    }
}