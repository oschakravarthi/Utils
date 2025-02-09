using SubhadraSolutions.Utils.Data.Metadata;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Kusto.Shared.Metadata;

public static class MetadataHelper
{
    public static IEnumerable<TableAndColumns> BuildTableAndColumnsMetadata(
        IEnumerable<DatabaseSchemaMetadataItem> sortedMetadata)
    {
        string previousTableName = null;
        var list = new List<ColumnMetaRecord>();

        foreach (var meta in sortedMetadata)
        {
            var columnMetadata = new ColumnMetaRecord
            {
                Name = meta.ColumnName,
                DataType = meta.ColumnType
            };

            if (previousTableName != null && meta.TableName != previousTableName)
            {
                yield return new TableAndColumns
                {
                    TableName = previousTableName,
                    ColumnsMetadata = list
                };

                list = [];
            }

            previousTableName = meta.TableName;
            list.Add(columnMetadata);
        }

        if (list.Count > 0)
        {
            yield return new TableAndColumns
            {
                TableName = previousTableName,
                ColumnsMetadata = list
            };
        }
    }
}