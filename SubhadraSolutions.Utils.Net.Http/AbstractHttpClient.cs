using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http;

public abstract class AbstractHttpClient : AbstractDisposable, IHttpClient
{
    private static readonly JsonMediaTypeFormatter DefaultJsonMediaTypeFormatter;
    private static readonly JsonMediaTypeFormatter DefaultJsonMediaTypeFormatterWithRemoteLinq;
    public abstract Uri BaseAddress { get; }

    static AbstractHttpClient()
    {
        DefaultJsonMediaTypeFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings = JsonSettings.RestSerializerSettings
        };

        DefaultJsonMediaTypeFormatterWithRemoteLinq = new JsonMediaTypeFormatter
        {
            SerializerSettings = JsonSettings.LinqSerializerSettings
        };
    }

    private static HttpContent BuildHttpContent<T>(T objectToSend, bool isRemoteLinq)
    {
        var formatter = isRemoteLinq ? DefaultJsonMediaTypeFormatterWithRemoteLinq : DefaultJsonMediaTypeFormatter;
        return new ObjectContent<T>(objectToSend, formatter, (MediaTypeHeaderValue)null);

        //var json = JsonConvert.SerializeObject(objectToSend, formatter.SerializerSettings);

        //return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static HttpContent BuildHttpContentSafe<T>(T objectToSend, bool isRemoteLinq)
    {
        if (objectToSend == null)
        {
            return null;
        }
        if (objectToSend is HttpContent content)
        {
            return content;
        }
        return BuildHttpContent(objectToSend, isRemoteLinq);
    }

    public ValueTask<T> CallAsync<T>(HttpMethod httpMethod, string uri, CancellationToken? cancellationToken = null)
    {
        return CallAsync<T>(httpMethod, uri, (IDictionary<string, string>)null, cancellationToken)
;
    }

    public async ValueTask<T> CallAsync<T>(HttpMethod httpMethod, string uri, IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        var request = await CreateRequestAsync(httpMethod, uri, headers, cancellationToken).ConfigureAwait(false);
        using (request)
        {
            return await SendAsync<T>(request, cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask<HttpResponseMessage> CallAsync(HttpMethod httpMethod, string uri, CancellationToken? cancellationToken = null)
    {
        return CallAsync(httpMethod, uri, (IDictionary<string, string>)null, cancellationToken)
;
    }

    public async ValueTask<HttpResponseMessage> CallAsync(HttpMethod httpMethod, string uri,
        IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        var request = await CreateRequestAsync(httpMethod, uri, headers, cancellationToken).ConfigureAwait(false);
        using (request)
        {
            return await SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask<TGet> CallAsync<TGet, TPost>(HttpMethod httpMethod, string uri, TPost objectToPost, CancellationToken? cancellationToken = null)
    {
        return CallAsync<TGet, TPost>(httpMethod, uri, objectToPost, null, cancellationToken)
;
    }

    public async ValueTask<TGet> CallAsync<TGet, TPost>(HttpMethod httpMethod, string uri, TPost objectToPost,
        IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        if (objectToPost != null)
        {
            bool isRemoteLinq = headers?.ContainsKey(NetDefaults.RemoteLinqIdHeaderName) == true;
            var request = await CreateRequestAsync(httpMethod, uri, headers, cancellationToken).ConfigureAwait(false);
            request.Content = BuildHttpContentSafe(objectToPost, isRemoteLinq);
            using (request)
            {
                return await SendAsync<TGet>(request, cancellationToken).ConfigureAwait(false);
            }
        }

        return await CallAsync<TGet>(httpMethod, uri, headers, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<HttpResponseMessage> CallAsync<T>(HttpMethod httpMethod, string uri, T objectToPost,
        CancellationToken? cancellationToken = null)
    {
        return CallAsync(httpMethod, uri, objectToPost, null, cancellationToken);
    }

    public async ValueTask<HttpResponseMessage> CallAsync<T>(HttpMethod httpMethod, string uri, T objectToPost,
        IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        var request = await CreateRequestAsync(httpMethod, uri, headers, cancellationToken).ConfigureAwait(false);
        bool isRemoteLinq = headers?.ContainsKey(NetDefaults.RemoteLinqIdHeaderName) == true;
        var content = BuildHttpContentSafe(objectToPost, isRemoteLinq);
        if (content != null)
        {
            request.Content = content;
        }
        using (request)
        {
            return await SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask<T> DeleteAsync<T>(string uri, CancellationToken? cancellationToken = null)
    {
        return CallAsync<T>(HttpMethod.Delete, uri, cancellationToken);
    }

    public ValueTask<T> DeleteAsync<T>(string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null)
    {
        return CallAsync<T>(HttpMethod.Delete, uri, headers, cancellationToken);
    }

    public ValueTask<HttpResponseMessage> DeleteAsync(string uri, CancellationToken? cancellationToken = null)
    {
        return CallAsync(HttpMethod.Delete, uri, cancellationToken);
    }

    public ValueTask<HttpResponseMessage> DeleteAsync(string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null)
    {
        return CallAsync(HttpMethod.Delete, uri, headers, cancellationToken);
    }

    public abstract Task DownloadFileAsync(string url, string pathToSave, CancellationToken? cancellationToken = null);

    public ValueTask<T> GetAsync<T>(string uri, CancellationToken? cancellationToken = null)
    {
        return CallAsync<T>(HttpMethod.Get, uri, cancellationToken);
    }

    public ValueTask<T> GetAsync<T>(string uri, IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        return CallAsync<T>(HttpMethod.Get, uri, headers, cancellationToken);
    }

    public ValueTask<HttpResponseMessage> GetAsync(string uri, CancellationToken? cancellationToken = null)
    {
        return CallAsync(HttpMethod.Get, uri, cancellationToken);
    }

    public ValueTask<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        return CallAsync(HttpMethod.Get, uri, headers, cancellationToken);
    }

    public ValueTask<TGet> PostAsync<TGet, TPost>(string uri, TPost objectToPost, CancellationToken? cancellationToken = null)
    {
        return CallAsync<TGet, TPost>(HttpMethod.Post, uri, objectToPost, cancellationToken)
;
    }

    public ValueTask<TGet> PostAsync<TGet, TPost>(string uri, TPost objectToPost,
        IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        return CallAsync<TGet, TPost>(HttpMethod.Post, uri, objectToPost, headers, cancellationToken)
;
    }

    public ValueTask<HttpResponseMessage> PostAsync<T>(string uri, T objectToPost, CancellationToken? cancellationToken = null)
    {
        return CallAsync(HttpMethod.Post, uri, objectToPost, cancellationToken);
    }

    public ValueTask<HttpResponseMessage> PostAsync<T>(string uri, T objectToPost,
        IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        return CallAsync(HttpMethod.Post, uri, objectToPost, headers, cancellationToken)
;
    }

    private static async ValueTask<T> ReadAsAsync<T>(HttpResponseMessage response, bool isRemoteLinq, CancellationToken? cancellationToken = null)
    {
        var formatter = isRemoteLinq ? DefaultJsonMediaTypeFormatterWithRemoteLinq : DefaultJsonMediaTypeFormatter;
        return await response.Content.ReadAsAsync<T>([formatter],
            cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
    }

    public async ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null)
    {
        string correlationId = requestMessage.GetHttpRequestHeaderValue(NetDefaults.CorrelationIdHeaderName);
        if (correlationId == null)
        {
            correlationId = Guid.NewGuid().ToString();
            requestMessage.Headers.Add(NetDefaults.CorrelationIdHeaderName, correlationId);
        }
        //Console.WriteLine("CALLING: " + requestMessage.RequestUri);
        var response = await SendCoreAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        response = await ValidateHttpResponseMessageAsync(response, correlationId).ConfigureAwait(false);
        return response;
    }

    public async ValueTask<T> SendAsync<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null)
    {
        string remoteLinqId = requestMessage.GetHttpRequestHeaderValue(NetDefaults.RemoteLinqIdHeaderName);
        var response = await SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        using (response)
        {
            return await ReadAsAsync<T>(response, remoteLinqId != null, cancellationToken).ConfigureAwait(false);
        }
    }

    protected static async ValueTask<HttpResponseMessage> ValidateHttpResponseMessageAsync(HttpResponseMessage response, string correlationId,
        CancellationToken? cancellationToken = null)
    {
        //var mediaType = response.Content.Headers?.ContentType?.MediaType;
        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = response.ReasonPhrase;
            if (response.Content != null)
            {
                var errorMessageData = await response.Content
                    .ReadAsByteArrayAsync(cancellationToken ?? CancellationToken.None)
                    .ConfigureAwait(false);
                errorMessage = Encoding.UTF8.GetString(errorMessageData);

                response.Content.Dispose();
            }

            throw new HttpRequestExceptionEx(response.ReasonPhrase, errorMessage, null, response.StatusCode, correlationId);
        }

        return response;
    }

    protected virtual ValueTask AddHeadersAsync(HttpRequestMessage requestMessage, IDictionary<string, string> headers, CancellationToken? cancellationToken = null)
    {
        if (headers != null)
        {
            foreach (var kvp in headers)
            {
                requestMessage.Headers.Add(kvp.Key, kvp.Value);
            }
        }

        return new ValueTask();
    }

    protected abstract ValueTask<HttpResponseMessage> SendCoreAsync(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null);

    private async ValueTask<HttpRequestMessage> CreateRequestAsync(HttpMethod httpMethod, string uri,
        IDictionary<string, string> headers, CancellationToken? cancellationToken)
    {
        var request = new HttpRequestMessage(httpMethod, uri);
        await AddHeadersAsync(request, headers, cancellationToken).ConfigureAwait(false);
        return request;
    }
}