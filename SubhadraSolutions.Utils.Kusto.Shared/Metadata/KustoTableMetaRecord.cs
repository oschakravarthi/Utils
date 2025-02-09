using SubhadraSolutions.Utils.Data.Metadata;
using System;

namespace SubhadraSolutions.Utils.Kusto.Shared.Metadata;

public class KustoTableMetaRecord : TableMetaRecord
{
    public TimeSpan SoftDeletePeriod { get; set; }
}