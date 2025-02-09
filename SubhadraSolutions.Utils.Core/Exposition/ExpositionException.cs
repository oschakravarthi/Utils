using System;

namespace SubhadraSolutions.Utils.Exposition;

[Serializable]
public sealed class ExpositionException : ApplicationException
{
    public ExpositionException(int? statusCode, string entityTypeOrServiceName, string actionName, string message)
        : base(message)
    {
        SetProperties(entityTypeOrServiceName, actionName, statusCode);
    }

    public ExpositionException(int? statusCode, string entityTypeOrServiceName, string actionName, string message,
        Exception innerException)
        : base(message, innerException)
    {
        SetProperties(entityTypeOrServiceName, actionName, statusCode);
    }

    public ExpositionException(int? statusCode)
    {
        StatusCode = statusCode;
    }

    public ExpositionException(int? statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public ExpositionException(int? statusCode, string message, Exception innerException) : base(message,
        innerException)
    {
        StatusCode = statusCode;
    }

    public string ActionName { get; private set; }

    public string EntityTypeOrServiceName { get; private set; }
    public int? StatusCode { get; private set; }

    private void SetProperties(string entityTypeOrServiceName, string actionName, int? statusCode)
    {
        // Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(entityTypeOrServiceName, nameof(entityTypeOrServiceName));
        EntityTypeOrServiceName = entityTypeOrServiceName;
        ActionName = actionName;
        StatusCode = statusCode;
    }
}