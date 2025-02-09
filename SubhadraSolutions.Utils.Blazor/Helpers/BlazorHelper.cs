using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SubhadraSolutions.Utils.Blazor.Helpers;

public static class BlazorHelper
{
    public static string BuildQueryForPageTranfer(object fromPage, Type toPageType, IDictionary<string, object> parameters = null, Func<PropertyInfo, bool> propertySelector = null)
    {
        var fromPageType = fromPage.GetType();
        var dictionary = new Dictionary<string, object>();
        foreach (var toProperty in toPageType.GetProperties())
        {
            if (!toProperty.IsDefined(typeof(ParameterAttribute), true))
            {
                continue;
            }
            if(propertySelector != null && !propertySelector(toProperty))
            {
                continue;
            }
            var propertyName = toProperty.Name;
            var fromProperty = fromPageType.GetProperty(propertyName);

            if (fromProperty == null || fromProperty.PropertyType != toProperty.PropertyType)
            {
                continue;
            }

            object propertyValue = null;
            if (parameters == null || !parameters.ContainsKey(propertyName))
            {
                propertyValue = fromProperty.GetValue(fromPage);
            }
            if (propertyValue == null)
            {
                continue;
            }

            dictionary.Add(propertyName, propertyValue);
        }
        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                if (kvp.Value != null)
                {
                    dictionary.Add(kvp.Key, kvp.Value);
                }
            }
        }

        var query = GetPageUrl(toPageType);
        var jObject = JObject.FromObject(dictionary, JsonSerializationHelper.Serializer);
        foreach (var kvp in jObject)
        {
            var propertyName=kvp.Key;
            var propertyValue = kvp.Value;
            var token = JToken.FromObject(propertyValue);

            if (token is JArray jArray)
            {
                var propertyArrayName = propertyName + "[]";

                foreach (var jItem in jArray)
                {
                    query = QueryHelpers.AddQueryString(query, propertyArrayName, jItem.ToString());
                }
            }
            else
            {
                query = QueryHelpers.AddQueryString(query, propertyName, token.ToString());
            }
        }
        return query;
    }

    public static string BuildQueryForPageTranfer(Type toPageType, IDictionary<string, object> queryParams)
    {
        var route = toPageType.GetCustomAttribute<RouteAttribute>();
        var query = route.Template;
        if (queryParams == null)
        {
            return query;
        }

        var pairs = new Dictionary<string, string>();
        foreach (var kvp in queryParams)
        {
            pairs.Add(kvp.Key, Convert.ToString(kvp.Value));
        }

        query = QueryHelpers.AddQueryString(query, pairs);
        return query;
    }

    public static string GetPageUrl(Type pageType)
    {
        var route = pageType.GetCustomAttribute<RouteAttribute>();
        var url = route.Template;
        return url;
    }

    public static void SetParametersFromUri(Uri uri, object obj)
    {
        var kvps = QueryHelpers.ParseQuery(uri.Query);
        var jObject = JsonSerializationHelper.BuildJObjectFromQuery(kvps);
        foreach (var property in obj.GetType().GetProperties())
        {
            if (!property.IsDefined(typeof(ParameterAttribute), true))
            {
                continue;
            }

            var propertyValue = jObject.BuildObjectFromJObject(property.Name, property.PropertyType, out var foundKey, out _);
            if (!foundKey)
            {
                continue;
            }

            property.SetValue(obj, propertyValue);
        }
    }

    public static void SetPropertiesFromUri(Uri uri, object obj)
    {
        var kvps = QueryHelpers.ParseQuery(uri.Query);
        var jObject = JsonSerializationHelper.BuildJObjectFromQuery(kvps);
        foreach (var property in obj.GetType().GetProperties())
        {
            var propertyValue = jObject.BuildObjectFromJObject(property.Name, property.PropertyType, out var foundKey, out _);
            if (!foundKey)
            {
                continue;
            }

            property.SetValue(obj, propertyValue);
        }
    }
}