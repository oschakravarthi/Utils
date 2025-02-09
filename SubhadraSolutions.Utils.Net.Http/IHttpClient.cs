using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http;

public interface IHttpClient : IDisposable
{
    Uri BaseAddress { get; }

    ValueTask<T> CallAsync<T>(HttpMethod httpMethod, string uri, CancellationToken? cancellationToken = null);

    ValueTask<T> CallAsync<T>(HttpMethod httpMethod, string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> CallAsync(HttpMethod httpMethod, string uri,
        CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> CallAsync(HttpMethod httpMethod, string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    ValueTask<TGet> CallAsync<TGet, TPost>(HttpMethod httpMethod, string uri, TPost objectToPost,
        CancellationToken? cancellationToken = null);

    ValueTask<TGet> CallAsync<TGet, TPost>(HttpMethod httpMethod, string uri, TPost objectToPost,
        IDictionary<string, string> headers, CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> CallAsync<T>(HttpMethod httpMethod, string uri, T objectToPost,
        CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> CallAsync<T>(HttpMethod httpMethod, string uri, T objectToPost,
        IDictionary<string, string> headers, CancellationToken? cancellationToken = null);

    ValueTask<T> DeleteAsync<T>(string uri, CancellationToken? cancellationToken = null);

    ValueTask<T> DeleteAsync<T>(string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> DeleteAsync(string uri, CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> DeleteAsync(string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    Task DownloadFileAsync(string url, string pathToSave, CancellationToken? cancellationToken = null);

    ValueTask<T> GetAsync<T>(string uri, CancellationToken? cancellationToken = null);

    ValueTask<T> GetAsync<T>(string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> GetAsync(string uri, CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    ValueTask<TGet> PostAsync<TGet, TPost>(string uri, TPost objectToPost,
        CancellationToken? cancellationToken = null);

    ValueTask<TGet> PostAsync<TGet, TPost>(string uri, TPost objectToPost, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> PostAsync<T>(string uri, T objectToPost,
        CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> PostAsync<T>(string uri, T objectToPost, IDictionary<string, string> headers,
        CancellationToken? cancellationToken = null);

    ValueTask<T> SendAsync<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null);

    ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
        CancellationToken? cancellationToken = null);
}