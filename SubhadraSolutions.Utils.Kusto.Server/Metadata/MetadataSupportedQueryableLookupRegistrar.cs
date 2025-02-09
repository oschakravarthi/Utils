using Kusto.Data.Common;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Metadata;
using SubhadraSolutions.Utils.Kusto.Server.Contracts;
using SubhadraSolutions.Utils.Kusto.Shared.Metadata;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Reflection;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Kusto.Server.Metadata;

public class MetadataSupportedQueryableLookupRegistrar(IMetadataSupportedQueryableLookup queryableLookup,
    IKustoQueryableFactory kustoQueryableFactory)
{
    [DynamicallyInvoked]
    public void Register(ICslQueryProvider cslQueryProvider)
    {
        var method = typeof(MetadataSupportedQueryableLookupRegistrar).GetMethod(nameof(Register),
            BindingFlags.NonPublic | BindingFlags.Instance);

        var metadataProvider = new KustoMetadataProvider(cslQueryProvider);
        var records = metadataProvider.GetDatabaseTablesAndColumns(cslQueryProvider.DefaultDatabaseName).ToList();
        var tablesAndColumns = MetadataHelper.BuildTableAndColumnsMetadata(records).ToList();

        if (tablesAndColumns.Count > 0)
        {
            var aName = "MetadataSupportedQueryableLookupRegistrar.Dynamic." + cslQueryProvider.DefaultDatabaseName;
            var assemblyName = new AssemblyName(aName);
            var ab = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule(assemblyName.Name);

            foreach (var tableAndColumns in tablesAndColumns)
            {
                var hasNumericColumns = tableAndColumns.ColumnsMetadata
                    .Select(c => CoreReflectionHelper.GetType(c.DataType)).Any(t => t?.IsNumericType() == true);
                if (!hasNumericColumns)
                {
                    continue;
                }

                var type = TypeBuilderFromColumnsMetadata.BuildType(mb, aName + "." + tableAndColumns.TableName,
                    tableAndColumns.ColumnsMetadata);
                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(this, [cslQueryProvider, tableAndColumns]);
            }
        }
    }

    private void Register<T>(ICslQueryProvider cslQueryProvider, TableAndColumns tableAndColumns)
    {
        var databaseAndTable = new DatabaseAndTable
        {
            //ClusterName = cslQueryProvider.Cluster,
            DatabaseName = cslQueryProvider.DefaultDatabaseName,
            TableName = tableAndColumns.TableName
        };

        var queryable =
            kustoQueryableFactory.CreateKustoQueryable<T>(cslQueryProvider, tableAndColumns.TableName);
        queryableLookup.RegisterQueryableFactory(() => queryable, databaseAndTable, tableAndColumns.ColumnsMetadata);
    }
}