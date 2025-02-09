//using Microsoft.RAM.Reliability.Common.Client.DataAccess.Helpers;
//using SubhadraSolutions.Utils.Linq;
//using Remote.Linq.Async;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Microsoft.RAM.Reliability.Common.Client.DataAccess
//{
//    public class AsyncLinqAdapter : ILinqAdapter
//    {
//        public async ValueTask<List<T>> GetList<T>(IQueryable<T> queryable)
//        {
//            var list = await queryable.GetList<T>();
//            return list;
//            //var list = queryable.ToListAsync();
//            //return new ValueTask <List<T>>(list);
//            //return new ValueTask<List<T>>(list);
//        }
//    }
//}