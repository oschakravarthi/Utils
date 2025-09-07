namespace SubhadraSolutions.Utils.Data.EntityFramework.PostgreSql.Config
{
    public class PostgresSqlConnectionConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
