using Microsoft.AspNetCore.Http;
using System;

namespace SubhadraSolutions.Utils.Exposition.REST;

public static class HttpMethodHelper
{
    public static string MapToHttpMethod(this HttpRequestMethod method)
    {
        switch (method)
        {
            case HttpRequestMethod.Get:
                return HttpMethods.Get;

            case HttpRequestMethod.Put:
                return HttpMethods.Put;

            case HttpRequestMethod.Post:
                return HttpMethods.Post;

            case HttpRequestMethod.Delete:
                return HttpMethods.Delete;

            default:
                throw new InvalidOperationException();
        }
    }

    public static HttpRequestMethod MapToHttpRequestMethod(string method)
    {
        if (method == HttpMethods.Get)
        {
            return HttpRequestMethod.Get;
        }

        if (method == HttpMethods.Put)
        {
            return HttpRequestMethod.Put;
        }

        if (method == HttpMethods.Post)
        {
            return HttpRequestMethod.Post;
        }

        if (method == HttpMethods.Delete)
        {
            return HttpRequestMethod.Delete;
        }

        throw new InvalidOperationException();
    }
}