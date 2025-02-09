using SubhadraSolutions.Utils.Data.Annotations;

namespace SubhadraSolutions.Utils.Kusto.Shared.Metadata;

public class QuerySchemaItem
{
    [NotInternable] public string ColumnName { get; set; }

    public int ColumnOrdinal { get; set; }
    public string ColumnType { get; set; }
    public string DataType { get; set; }
}