using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data.Metadata;

public class TableAndColumns
{
    public List<ColumnMetaRecord> ColumnsMetadata { get; set; } = [];
    public string TableName { get; set; }
}