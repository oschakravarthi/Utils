using System;

namespace SubhadraSolutions.Utils.Kusto.Server;

[Serializable]
public class KustoDatabaseInfo
{
    public string Cluster { get; set; }
    public string ConnectionStringName { get; set; }
    public string Name { get; set; }
}