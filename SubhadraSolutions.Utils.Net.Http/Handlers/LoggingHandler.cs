using Microsoft.Extensions.Logging;
using SubhadraSolutions.Utils.Json;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Net.Http.Handlers
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger logger;
        private readonly bool logRequestContent;
        private readonly bool logResponseContent;
        private readonly bool prettyPrintContent;

        public LoggingHandler(bool logRequestContent, bool logResponseContent, bool prettyPrintContent, ILogger logger)
            : this(logRequestContent, logResponseContent, prettyPrintContent, logger, new HttpClientHandler())
        {
        }

        public LoggingHandler(bool logRequestContent, bool logResponseContent, bool prettyPrintContent, ILogger logger, HttpMessageHandler httpMessageHandler)
            : base(httpMessageHandler)
        {
            this.logRequestContent = logRequestContent;
            this.logResponseContent = logResponseContent;
            this.prettyPrintContent = prettyPrintContent;
            this.logger = logger;
        }

        public bool EnableLogging { get; set; } = true;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            StringBuilder sb = null;

            if (this.EnableLogging)
            {
                sb = new StringBuilder();
                //sb.AppendLine("___________________________________________");
                sb.AppendLine($"Making Http request to {request.Method} {request.RequestUri}");
                string correlationId = request.GetHttpRequestHeaderValue(NetDefaults.CorrelationIdHeaderName);
                if (correlationId != null)
                {
                    sb.AppendLine($"With CorrelationId: {correlationId}");
                }
                //if (request.Headers != null)
                //{
                //    sb.AppendLine($"With Headers:");
                //    foreach (var header in request.Headers)
                //    {
                //        sb.AppendLine($"{header.Key}:{header.Value}");
                //    }
                //}
                if (this.logRequestContent && request.Content != null)
                {
                    var body = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (this.prettyPrintContent)
                    {
                        body = JsonHelper.JsonPrettify(body);
                    }
                    sb.AppendLine("With request body");
                    sb.AppendLine(body);
                }
                logger.Log(LogLevel.Information, sb.ToString());
                sb.Clear();

                sb.AppendLine($"Received Http response for {request.Method} {request.RequestUri}");
                if (correlationId != null)
                {
                    sb.AppendLine($"With CorrelationId: {correlationId}");
                }
            }
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (this.EnableLogging)
            {
                if (this.logResponseContent && response.Content != null)
                {
                    var responseAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (this.prettyPrintContent)
                    {
                        responseAsString = JsonHelper.JsonPrettify(responseAsString);
                    }
                    sb.AppendLine("With response body:");
                    sb.AppendLine(responseAsString);
                }
                //sb.AppendLine("___________________________________________");
                logger.Log(LogLevel.Information, sb.ToString());
            }
            return response;
        }
    }
}