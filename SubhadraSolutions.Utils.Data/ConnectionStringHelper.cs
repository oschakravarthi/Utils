//using SubhadraSolutions.Utils.Security.Cryptography;
//using System.Configuration;

//namespace SubhadraSolutions.Utils.Data
//{
//    public static class ConnectionStringHelper
//    {
//        public static string GetConnectionStringFromConfig(string name)
//        {
//            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[name];
//            if (connectionStringSettings == null)
//            {
//                return null;
//            }
//            return EncryptionHelper.DecryptPasswordInString(connectionStringSettings.ConnectionString);
//        }
//    }
//}