using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace SubhadraSolutions.Utils.Web;

public static class UrlHelper
{
    /// <summary>
    /// Builds a query string from the given key value pairs.
    /// </summary>
    /// <param name="pairs"></param>
    /// <returns></returns>
    public static string BuildQueryString(params StringKeyValuePair[] pairs)
    {
        string s = null;
        for (var i = 0; i < pairs.Length; i++)
        {
            var pair = pairs[i];
            if (pair.Value == null)
            {
                continue;
            }

            if (s == null)
            {
                s += '?';
            }
            else
            {
                s += '&';
            }

            var encodedValue = HttpUtility.UrlEncode(pair.Value);
            s += $"{pair.Key}={encodedValue}";
        }

        return s;
    }

    public static string[] ConvertStringValuesToStringArray(this StringValues values)
    {
        var strings = new string[values.Count];
        for (var i = 0; i < values.Count; i++)
        {
            var value = values[i];
            if (!string.IsNullOrEmpty(value))
            {
                value = HttpUtility.UrlDecode(value);
            }
            strings[i] = value;
        }
        return strings;
    }

    public static string EnsureUrlPrefix(this string serverName, bool https = true)
    {
        if (!serverName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !serverName.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            serverName = (https ? "https://" : "http://") + serverName;
        }

        serverName = serverName.TrimEnd('/');
        return serverName;
    }

    public static string GetHostnameIfUrl(this string input)
    {
        if (input != null && (input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                              input.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                return new Uri(input).DnsSafeHost;
            }
            catch (Exception)
            {
            }
        }

        return new string(input.Select(ch => !"/\\:*?\"<>|#%".Contains(ch) ? ch : '_').ToArray());
    }

    public static string GetPathFromUrl(string url)
    {
        url = url.TrimStart('/');
        var questionmarkIndex = url.IndexOf('?');
        if (questionmarkIndex > -1)
        {
            url = url.Substring(0, questionmarkIndex);
        }

        return url;
    }

    public static NameValueCollection GetQueryFromUrl(string url)
    {
        var questionmarkIndex = url.IndexOf('?');
        if (questionmarkIndex > -1)
        {
            url = url.Substring(questionmarkIndex + 1);
        }

        return HttpUtility.ParseQueryString(url);
    }

    public static IEnumerable<KeyValuePair<string, string[]>> GetValuesFromNameValueCollection(
        this NameValueCollection collection)
    {
        for (var i = 0; i < collection.Count; i++)
            yield return new KeyValuePair<string, string[]>(collection.GetKey(i), collection.GetValues(i));
    }
}