using System;

namespace SubhadraSolutions.Utils.Exposition;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ExposeAttribute : Attribute
{
    public ExposeAttribute(string subPath = null, string actionName = null,
        HttpRequestMethod httpRequestMethod = HttpRequestMethod.Get)
    {
        SubPath = subPath;
        ActionName = actionName;
        HttpRequestMethod = httpRequestMethod;
    }

    public string ActionName { get; }
    public HttpRequestMethod HttpRequestMethod { get; }
    public string SubPath { get; }
}