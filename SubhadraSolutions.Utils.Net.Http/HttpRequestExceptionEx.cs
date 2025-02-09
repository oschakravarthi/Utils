using System;
using System.Net;
using System.Net.Http;

namespace SubhadraSolutions.Utils.Net.Http;

[Serializable]
public class HttpRequestExceptionEx : HttpRequestException
{
#if NETFRAMEWORK
        public HttpRequestExceptionEx(string message, string serverResponse, Exception inner, HttpStatusCode? statusCode, string correlationId)
            : base(message, inner)
        {
            ServerResponse = serverResponse;
            this.StatusCode = statusCode;
            this.CorrelationId = correlationId;
        }
        public HttpStatusCode? StatusCode { get; }
#else

    public HttpRequestExceptionEx(string message, string serverResponse, Exception inner, HttpStatusCode? statusCode, string correlationId)
        : base(message, inner, statusCode)
    {
        ServerResponse = serverResponse;
        this.CorrelationId = correlationId;
    }

#endif
    public string ServerResponse { get; }
    public string CorrelationId { get; }
}