using Aqua.Dynamic;
using Remote.Linq.Expressions;
using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.ApiClient.Contracts;
using SubhadraSolutions.Utils.Net;
using SubhadraSolutions.Utils.Net.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Providers;

public class LinqDataProvider : AbstractDisposable, ILinqDataProvider
{
    private readonly IHttpClient _linqHttpClient;

    public LinqDataProvider(IHttpClient linqHttpClient)
    {
        _linqHttpClient = linqHttpClient;
    }

    public ValueTask<DynamicObject> PostAsync(string url, Expression expression)
    {
        return PostAsync(url, expression, CancellationToken.None);
    }

    public ValueTask<DynamicObject> PostAsync(string url, Expression expression, CancellationToken cancellationToken)
    {
        return PostAsync(url, expression, null, cancellationToken);
    }

    public ValueTask<DynamicObject> PostAsync(string url, Expression expression, IDictionary<string, string> headers)
    {
        return PostAsync(url, expression, headers, CancellationToken.None);
    }

    public ValueTask<DynamicObject> PostAsync(string url, Expression expression, IDictionary<string, string> headers, CancellationToken cancellationToken)
    {
        if (headers == null)
        {
            headers = new Dictionary<string, string>();
        }
        headers.Add(NetDefaults.RemoteLinqIdHeaderName, Guid.NewGuid().ToString());
        return _linqHttpClient.PostAsync<DynamicObject, Expression>(url, expression, headers, cancellationToken)
;
    }

    protected override void Dispose(bool disposing)
    {
        _linqHttpClient?.Dispose();
    }
}