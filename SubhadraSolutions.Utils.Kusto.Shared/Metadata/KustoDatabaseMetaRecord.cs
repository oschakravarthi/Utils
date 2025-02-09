using SubhadraSolutions.Utils.Data.Metadata;
using System;

namespace SubhadraSolutions.Utils.Kusto.Shared.Metadata;

public class KustoDatabaseMetaRecord : DatabaseMetaRecord
{
    public TimeSpan SoftDeletePeriod { get; set; }
}