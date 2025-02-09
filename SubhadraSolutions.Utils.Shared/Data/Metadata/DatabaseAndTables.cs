using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data.Metadata;

[Serializable]
public class DatabaseAndTables
{
    public string ClusterName { get; set; }
    public string DatabaseName { get; set; }
    public List<TableAndColumns> Tables { get; set; } = [];
}