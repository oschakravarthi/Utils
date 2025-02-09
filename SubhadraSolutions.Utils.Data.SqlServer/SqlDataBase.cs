//using Microsoft.Data.SqlClient;
//using SubhadraSolutions.Utils.Diagnostics;
//using System.Data;
//using System.Text.RegularExpressions;

//namespace SubhadraSolutions.Utils.Data.SqlServer
//{
//    public class SqlDataBase : IDisposable
//    {
//        protected SqlConnection connection;

//        public SqlDataBase(string connectionInfo)
//        {
//            string connectionString = connectionInfo;
//            if (!connectionInfo.Contains(";"))
//            {
//                connectionString = ConnectionStringHelper.GetConnectionStringFromConfig(connectionInfo);
//            }
//            Initialize(connectionString);
//        }

//        public void Dispose()
//        {
//            if (connection.State != 0)
//            {
//                connection.Close();
//                connection.Dispose();
//            }
//        }

//        public DataSet ExecuteCommand(string commandText)
//        {
//            SqlDataAdapter adapter = null;
//            try
//            {
//                return ExecuteCommand(commandText, out adapter);
//            }
//            finally
//            {
//                adapter?.Dispose();
//            }
//        }

//        public DataSet ExecuteCommand(string commandText, out SqlDataAdapter adapter)
//        {
//            DataSet dataSet = new DataSet();
//            adapter = null;
//            if (!string.IsNullOrEmpty(commandText))
//            {
//                using (SqlCommand sqlCommand = connection.CreateCommand())
//                {
//                    sqlCommand.CommandText = commandText;
//                    adapter = new SqlDataAdapter(sqlCommand);
//                    adapter.Fill(dataSet);
//                    return dataSet;
//                }
//            }
//            return dataSet;
//        }

//        private static string RemovePasswordText(string input)
//        {
//            return Regex.Replace(input, "password=[^;]*", "password=********", RegexOptions.IgnoreCase);
//        }

//        private void Initialize(string connectionString)
//        {
//            connection = new SqlConnection(connectionString);
//            try
//            {
//                connection.Open();
//            }
//            catch (Exception ex)
//            {
//                string text = RemovePasswordText(connectionString);
//                TraceLogger.TraceError("{0}: Hit {1} while connecting to SQL server! Providing details, then re-throwing for caller to catch.\r\n - SQL Connection string: \"{2}\"\r\n - Exception details:\r\n{3}", ActiveCode.Current.Name, ex.GetType().Name, text, ex);
//                connection.Dispose();
//                throw;
//            }
//        }
//    }
//}