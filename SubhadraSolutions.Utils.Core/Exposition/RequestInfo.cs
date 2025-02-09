using System;

namespace SubhadraSolutions.Utils.Exposition;

public class RequestInfo : IEquatable<RequestInfo>
{
    private readonly int hashcode;

    public RequestInfo(HttpRequestMethod httpRequestMethod, string path, string actionName)
    {
        HttpRequestMethod = httpRequestMethod;
        if (string.IsNullOrWhiteSpace(path))
        {
            path = null;
        }
        if (string.IsNullOrWhiteSpace(actionName))
        {
            actionName = null;
        }
        Path = path;
        ActionName = actionName;
        hashcode = ToString().ToLower().GetHashCode();
    }

    public string ActionName { get; }
    public HttpRequestMethod HttpRequestMethod { get; }
    public string Path { get; }

    public bool Equals(RequestInfo other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return HttpRequestMethod == other.HttpRequestMethod && string.Equals(this.GetFullPath(null), other.GetFullPath(null), StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as RequestInfo);
    }

    public override int GetHashCode()
    {
        return hashcode;
    }

    public string GetFullPath(string apiBasePath)
    {
        var s = apiBasePath;
        if (Path != null)
        {
            s += "/" + Path;
        }

        if (this.ActionName != null)
        {
            s += "/" + ActionName;
        }

        return s;
    }

    public override string ToString()
    {
        var s = HttpRequestMethod + " ";
        if (Path != null)
        {
            s += "/" + Path;
        }

        if (this.ActionName != null)
        {
            s += "/" + ActionName;
        }

        return s;
    }
}