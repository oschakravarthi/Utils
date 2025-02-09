using SubhadraSolutions.Utils.Data.Annotations;

namespace SubhadraSolutions.Utils.Kusto.Shared.Metadata;

public class DatabaseSchemaMetadataItem
{
    [NotInternable] public string ColumnName { get; set; }

    public string ColumnType { get; set; }
    public string TableName { get; set; }
}