using System;

namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public class KustoExpressionException : ApplicationException
{
    public KustoExpressionException(string message) : base(message)
    {
    }

    public KustoExpressionException()
    {
    }

    public KustoExpressionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}