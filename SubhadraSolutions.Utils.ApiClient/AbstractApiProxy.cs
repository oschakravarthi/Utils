using SubhadraSolutions.Utils.Net.Http;

namespace SubhadraSolutions.Utils.ApiClient;

public abstract class AbstractApiProxy
{
    protected readonly string _apiPathPrefix;
    protected readonly IHttpClient _dataProvider;

    protected AbstractApiProxy(IHttpClient dataProvider, string apiPathPrefix)
    {
        _dataProvider = dataProvider;
        _apiPathPrefix = apiPathPrefix;
    }

    protected static string RemoveAsyncSuffix(string s)
    {
        var suffix = "Async";
        if (s.EndsWith(suffix))
        {
            s = s.Substring(0, s.Length - suffix.Length);
        }

        return s;
    }
}