using Kusto.Data.Common;
using Kusto.Data.Ingestion;
using Kusto.Ingest;
using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace SubhadraSolutions.Utils.Kusto.Server;

public static class KustoIngestionHelper
{
    private static readonly MethodInfo IngestTemplateMethod =
        typeof(KustoIngestionHelper).GetMethod(nameof(Ingest), BindingFlags.Public | BindingFlags.Static);

    [DynamicallyInvoked]
    public static IKustoIngestionResult Ingest<T>(T obj, IKustoIngestClient ingestClient,
        string databaseName, string tableName)
    {
        return IngestObjects(new[] { obj }, ingestClient, databaseName, tableName);
    }

    public static IKustoIngestionResult IngestObject(object obj, IKustoIngestClient ingestClient,
        string databaseName, string tableName)
    {
        var type = obj.GetType();
        var method = IngestTemplateMethod.MakeGenericMethod(type);
        return (IKustoIngestionResult)method.Invoke(null, [obj, ingestClient, databaseName, tableName]);
    }

    public static IKustoIngestionResult IngestObjects<T>(IEnumerable<T> objects,
        IKustoIngestClient ingestClient, string databaseName, string tableName)
    {
        //string mappingName = CreateMappingIfNotExists<T>(cslQueryProviderFactory, tableName, IngestionMappingKind.Json);

        var mappings = BuildColumnMappings<T>();
        var ingestProperties = new KustoIngestionProperties(databaseName, tableName)
        {
            Format = DataSourceFormat.json,
            IngestionMapping = new IngestionMapping
            {
                IngestionMappingKind = IngestionMappingKind.Json,
                //IngestionMappingReference = mappingName,
                IngestionMappings = mappings
            }
        };

        using var ms = new MemoryStream();

        using (var writer = new Utf8JsonWriter(ms))
        {
            foreach (var obj in objects)
            {
                JsonSerializer.Serialize(writer, obj);
            }
        }

        ms.Seek(0, SeekOrigin.Begin);
        return ingestClient.IngestFromStream(ms, ingestProperties);
    }

    private static ColumnMapping[] BuildColumnMappings<T>()
    {
        var properties = ReflectionHelper.GetPublicProperties<T>(false);
        var columnMappings = new ColumnMapping[properties.Count];

        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var fieldName = property.Name;

            var fieldType = KustoHelper.GetKustoScalarTypeName(property.PropertyType);

            //if (property.IsDefined(typeof(JsonAttribute), true))
            //{
            //    fieldType = "string";
            //}

            if (fieldType == "dynamic")
            {
                fieldType = "string";
            }

            //columnMappings[i] = new ColumnMapping(fieldName, fieldType, new Dictionary<string, string> { { MappingConsts.Path, "$." + fieldName }, { MappingConsts.TransformationMethod, "0" }, { MappingConsts.Ordinal, i.ToString() } });

            columnMappings[i] = new ColumnMapping(fieldName, fieldType,
                new Dictionary<string, string> { { MappingConsts.Path, "$." + fieldName } });
        }

        return columnMappings;
    }

    //private static string CreateMappingIfNotExists<T>(ICslQueryProviderFactory cslQueryProviderFactory, string tableName, IngestionMappingKind ingestionMappingKind)
    //{
    //    var mappingName = tableName + "_" + ingestionMappingKind.ToString();
    //    //string mappingName = tableName;
    //    using ICslAdminProvider adminClient = cslQueryProviderFactory.CreateCslAdminProvider();
    //    string showMappingsCommand = null;
    //    switch (ingestionMappingKind)
    //    {
    //        case IngestionMappingKind.Csv:
    //            showMappingsCommand = CslCommandGenerator.GenerateTableCsvMappingsShowCommand(tableName);
    //            break;
    //        case IngestionMappingKind.Json:
    //            showMappingsCommand = CslCommandGenerator.GenerateTableJsonMappingsShowCommand(tableName);
    //            break;
    //        default:
    //            throw new NotSupportedException();
    //    }
    //    List<IngestionMappingShowCommandResult> existingMappings = adminClient.ExecuteControlCommand<IngestionMappingShowCommandResult>(cslQueryProviderFactory.DatabaseName, showMappingsCommand).ToList();

    //    if (existingMappings.Find(m => string.Equals(m.Name, mappingName, StringComparison.Ordinal)) != null)
    //    {
    //        return mappingName;
    //    }

    //    ColumnMapping[] columnMappings = BuildColumnMappings<T>();
    //    string createMappingCommand = CslCommandGenerator.GenerateTableMappingCreateCommand(ingestionMappingKind, tableName, mappingName, columnMappings);
    //    adminClient.ExecuteControlCommand(cslQueryProviderFactory.DatabaseName, createMappingCommand);

    //    return mappingName;
    //}
}