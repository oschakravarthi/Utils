using System;

namespace SubhadraSolutions.Utils.Data.Metadata;

[Serializable]
public class ColumnMetaRecord : AbstractMetaRecord
{
    private string dataType;

    public string DataType
    {
        get => dataType;
        set { dataType = value == null ? null : string.Intern(value); }
    }
}