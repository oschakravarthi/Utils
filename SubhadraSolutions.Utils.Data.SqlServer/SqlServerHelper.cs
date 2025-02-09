using Azure.Core;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Data.SqlServer
{
    public static class SqlServerHelper
    {
        private static readonly TokenRequestContext tokenRequestContext = new(["https://database.windows.net/.default"]);

        public static string BuildConnectionStringForTokenBasedAuthentication(string server, string databaseName, bool isReadonly = false)
        {
            Dictionary<string, string> additionalParameters = null;
            if (isReadonly)
            {
                additionalParameters = new Dictionary<string, string>
                {
                    {"ApplicationIntent", "READONLY" }
                };
            }
            return BuildConnectionStringForTokenBasedAuthenticationWithAdditionalParameters(server, databaseName, additionalParameters);
        }

        public static string BuildConnectionStringForTokenBasedAuthenticationWithAdditionalParameters(string server, string databaseName, IReadOnlyDictionary<string, string> additionalParameters = null)
        {
            var builder = new SqlConnectionStringBuilder
            {
                ["server"] = server,
                ["database"] = databaseName
            };
            if (additionalParameters != null)
            {
                foreach (var kvp in additionalParameters)
                {
                    builder[kvp.Key] = kvp.Value;
                }
            }

            return builder.ConnectionString;
        }

        public static string BuildConnectionString(string server, string databaseName, SecureString userName, SecureString password)
        {
            var un = userName.SecureStringToString();
            var pwd = password.SecureStringToString();
            var useIntegratedSecurity = string.IsNullOrEmpty(un) && string.IsNullOrEmpty(pwd);

            var builder = new SqlConnectionStringBuilder();

            if (useIntegratedSecurity)
            {
                builder["Integrated Security"] = "SSPI";
                builder["Persist Security Info"] = false;
                builder["Data Source"] = server;
                builder["Initial Catalog"] = databaseName;
                builder["UID"] = "{}";
                builder["PWD"] = "{}";
            }
            else
            {
                builder["server"] = server;
                builder["UID"] = un;
                builder["PWD"] = pwd;
                builder["database"] = databaseName;
            }

            return builder.ConnectionString;
        }

        public static async Task<SqlConnection> GetOpenedSqlConnectionAsync(string connectionString, TokenCredential tokenCredential = null)
        {
            var sqlConnection = new SqlConnection(connectionString);
            if (tokenCredential != null)
            {
                var token = await tokenCredential.GetTokenAsync(tokenRequestContext, CancellationToken.None);
                sqlConnection.AccessToken = token.Token;
            }
            await sqlConnection.OpenAsync().ConfigureAwait(false);

            return sqlConnection;
        }
    }
}