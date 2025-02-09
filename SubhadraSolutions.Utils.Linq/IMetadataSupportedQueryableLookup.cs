using SubhadraSolutions.Utils.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public interface IMetadataSupportedQueryableLookup : IEnumerable<DatabaseAndTables>
{
    public Type GetElementType(DatabaseAndTable databaseAndTable);

    public IQueryable<T> GetQueryable<T>();

    public IQueryable GetQueryable(Type entityType);

    public IQueryable GetQueryable(DatabaseAndTable databaseAndTable);

    public void RegisterQueryableFactory(Func<IQueryable> queryableFactory, Type elementType, DatabaseAndTable table,
        List<ColumnMetaRecord> columns);

    public void RegisterQueryableFactory<T>(Func<IQueryable<T>> queryableFactory, DatabaseAndTable table,
        List<ColumnMetaRecord> columns);
}