//using SubhadraSolutions.Utils.Server.Exposition;
//using SubhadraSolutions.Utils.Data;
//using SubhadraSolutions.Utils.Reflection;

//namespace SubhadraSolutions.Utils.Server.Data
//{
//    public class ExpositionQueryDataSource : IDataSource<object>
//    {
//        private readonly IExpositionLookup expositionLookup;

//        public ExpositionQueryDataSource(IExpositionLookup expositionLookup)
//        {
//            this.expositionLookup = expositionLookup;
//            EnumerableToList = false;
//        }

//        public bool EnumerableToList { get; set; }
//        public IDataSource<string> QueryProvider { get; set; }

//        public object GetData()
//        {
//            var query = QueryProvider.GetData();
//            var obj = expositionLookup.Execute(query);
//            if (EnumerableToList)
//            {
//                obj = ReflectionHelper.BuildListIfEnumerableAndNotListOrReturnSame(obj);
//            }
//            return obj;
//        }
//    }
//}