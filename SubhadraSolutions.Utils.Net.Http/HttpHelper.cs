using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http;

public static class HttpHelper
{
    public static Task DownloadFileAsync(this HttpClient client, Uri uri, string fileName,
        CancellationToken? cancellationToken = null)
    {
        return client.DownloadFileAsync(uri.ToString(), fileName, cancellationToken);
    }

    public static string GetHttpRequestHeaderValue(this HttpRequestMessage requestMessage, string headerName)
    {
        var header = requestMessage.Headers.FirstOrDefault(x => x.Key == headerName);
        string headerValue = null;
        if (!(header.Value?.Any() != true))
        {
            headerValue = header.Value.First();
        }
        return headerValue;
    }

    public static async Task DownloadFileAsync(this HttpClient client, string uri, string fileName,
        CancellationToken? cancellationToken = null)
    {
        var stream = await client.GetStreamAsync(uri,
            cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        using (stream)
        {
            using (var fs = File.OpenWrite(fileName))
            {
                await stream.CopyToAsync(fs).ConfigureAwait(false);
            }
        }
    }

    public static async Task<JObject> ReadJObjectAsync(this HttpContent content,
        CancellationToken? cancellationToken = null)
    {
        var json = await content.ReadAsStringAsync(cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        var jObject = (JObject)JsonConvert.DeserializeObject(json);
        return jObject;
    }
}