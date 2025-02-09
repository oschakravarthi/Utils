using Kusto.Data.Common;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Metadata;
using SubhadraSolutions.Utils.Kusto.Shared.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Kusto.Server.Metadata;

public class KustoMetadataProvider(ICslQueryProvider cslQueryProvider)
{
    // private static readonly string GetFunctionsInDatabaseQuery = ".show functions | project Name, Parameters, DocString, Body";
    private const string GetColumnsOfTableQueryTemplate =
        ".show table [{0}] cslschema | extend columnswithdatatype = split(Schema, ',') | mv-expand ['columnswithdatatype'] | extend Name = tostring(split(columnswithdatatype, ':')[0]), DataType = tostring(split(columnswithdatatype, ':')[1]) | project Name, DataType";

    private const string GetDatabasesInClusterQuery =
        ".show databases details | project Name=DatabaseName, SoftDeletePeriod = totimespan(parse_json(RetentionPolicy).SoftDeletePeriod)";

    private const string GetDatabaseTablesAndColumnsQueryTemplate =
        ".show database [{0}] schema | project TableName, ColumnName, ColumnType | where isnotempty(TableName) and isnotempty(ColumnName)| order by TableName, ColumnName";

    private const string GetTablesInDatabaseQuery =
        ".show tables details | project Name= TableName, Description = DocString, SoftDeletePeriod = totimespan(parse_json(RetentionPolicy).SoftDeletePeriod)";

    public IEnumerable<DatabaseSchemaMetadataItem> GetDatabaseTablesAndColumns(string databaseName)
    {
        var query = string.Format(GetDatabaseTablesAndColumnsQueryTemplate, databaseName);
        return new KustoEntitiesEnumerable<DatabaseSchemaMetadataItem>(cslQueryProvider, query, null,
            DynamicEntityBuilderFactory<DatabaseSchemaMetadataItem>.Instance);
    }

    public List<ColumnMetaRecord> GetListOfColumns(string tableName)
    {
        var query = string.Format(GetColumnsOfTableQueryTemplate, tableName);
        IEnumerable<ColumnMetaRecord> enumerable =
            new KustoEntitiesEnumerable<ColumnMetaRecord>(cslQueryProvider, query, null,
                DynamicEntityBuilderFactory<ColumnMetaRecord>.Instance);
        enumerable = GlobalInterner<ColumnMetaRecord>.WrapIntern(enumerable);
        return enumerable.ToList();
    }

    public List<KustoDatabaseMetaRecord> GetListOfDatabases()
    {
        var enumerable = new KustoEntitiesEnumerable<KustoDatabaseMetaRecord>(cslQueryProvider,
            GetDatabasesInClusterQuery, null, DynamicEntityBuilderFactory<KustoDatabaseMetaRecord>.Instance);
        return enumerable.ToList();
    }

    public List<KustoTableMetaRecord> GetListOfTables()
    {
        var enumerable = new KustoEntitiesEnumerable<KustoTableMetaRecord>(cslQueryProvider,
            GetTablesInDatabaseQuery, null, DynamicEntityBuilderFactory<KustoTableMetaRecord>.Instance);
        return enumerable.ToList();
    }

    // public List<FunctionMetadata> GetListOfFunctions()
    // {
    //    var enumerable = new KustoEntitiesEnumerable<FunctionMetadata>(this.cslQueryProviderFactory, GetFunctionsInDatabaseQuery, DynamicEntityBuilderFactory<FunctionMetadata>.Instance);
    //    return enumerable.ToList();
    // }
}