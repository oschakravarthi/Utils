using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Net.Http;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.EventProcessing;

public class HttpGetComponent(IHttpClient httpClient) : IDataSource<string>
{
    //public Type DTOType { get; set; }

    public Dictionary<string, string> Headers { get; set; }
    public IDataSource<string> UrlProvider { get; set; }

    public string GetData()
    {
        var url = UrlProvider.GetData();
        var task = httpClient.GetAsync<string>(url, Headers);
        var result = task.IsCompleted ? task.Result : task.GetAwaiter().GetResult();
        return result;
    }
}