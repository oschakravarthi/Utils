//using SubhadraSolutions.Utils.Kusto.Server.Contracts;
//using SubhadraSolutions.Utils.Data;
//using System.Collections.Generic;
//using System.Data;

//namespace SubhadraSolutions.Utils.Kusto.Server
//{
//    public class KustoDataSource : IDataSource<IDataReader>
//    {
//        private readonly IDictionary<string, ICslQueryProviderFactory> cslQueryProviderFactories;

//        public KustoDataSource(IDictionary<string, ICslQueryProviderFactory> cslQueryProviderFactories)
//        {
//            this.cslQueryProviderFactories = cslQueryProviderFactories;
//            this.CslQueryProviderFactoryName = "gosh";
//        }

//        public string CslQueryProviderFactoryName { get; set; }

//        public IDataSource<string> QuerySource { get; set; }

//        public IDataReader GetData()
//        {
//            var cslQueryProviderFactory = this.cslQueryProviderFactories[this.CslQueryProviderFactoryName];

//            var query = this.QuerySource.GetData();
//            var queryProvider = cslQueryProviderFactory.CreateCslQueryProvider();
//            var dataReader = queryProvider.ExecuteQuery(query, KustoHelper.DefaultClientRequestProperties);
//            return dataReader;
//        }
//    }
//}