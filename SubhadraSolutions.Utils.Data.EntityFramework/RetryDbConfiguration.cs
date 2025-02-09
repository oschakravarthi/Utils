//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;

//namespace SubhadraSolutions.Utils.Data.EntityFramework
//{
//    public class RetryDbConfiguration : DbConfiguration
//    {
//        public RetryDbConfiguration()
//        {
//            SetExecutionStrategy("System.Data.SqlClient", () => SuspendExecutionStrategy ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
//              : new SqlAzureExecutionStrategy(3, TimeSpan.FromSeconds(10)));
//        }

//        public static bool SuspendExecutionStrategy
//        {
//            get
//            {
//                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
//            }
//            set
//            {
//                CallContext.LogicalSetData("SuspendExecutionStrategy", value);
//            }
//        }
//    }

//}