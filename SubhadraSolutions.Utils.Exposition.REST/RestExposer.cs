using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.ApplicationInsights.AspNetCore.Config;
using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.IO;
using SubhadraSolutions.Utils.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using static SubhadraSolutions.Utils.OpenApi.OpenApiHelper;

namespace SubhadraSolutions.Utils.Exposition.REST;

public class RestExposer : AbstractInitializable
{
    private readonly IExpositionLookup expositionLookup;

    private readonly Dictionary<RequestInfo, Dictionary<ParameterInfo, ParameterValueLocation>>
        parametersLookup = [];

    private readonly ProductInfo productInfo;
    private OpenApiDocument openApiDocument;
    private string openApiJson;
    private readonly HttpTelemetryConfig telemetryConfig;
    private readonly Func<object, bool> isResponceAnErrorFunc;

    public RestExposer(IExpositionLookup expositionLookup, ProductInfo productInfo, HttpTelemetryConfig telemetryConfig = null, Func<object, bool> isResponceAnErrorFunc = null)
    {
        this.expositionLookup = expositionLookup;
        this.productInfo = productInfo;
        this.telemetryConfig = telemetryConfig;
        if (isResponceAnErrorFunc == null)
        {
            isResponceAnErrorFunc = (response) =>
            {
                if (response is IExceptioned exceptioned)
                {
                    return exceptioned.Exception != null;
                }
                return false;
            };
        }

        this.isResponceAnErrorFunc = isResponceAnErrorFunc;
        expositionLookup.RegisterMethods(null, this);
    }

    public IReadOnlyDictionary<ParameterInfo, ParameterValueLocation> GetParameters(RequestInfo requestInfo)
    {
        if (!parametersLookup.TryGetValue(requestInfo, out var parameters))
        {
            return null;
        }

        return parameters.AsReadOnly();
    }

    public OpenApiDocument BuildOpenApiDocument()
    {
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Version = productInfo.Version,
                Title = productInfo.Name
            },
        };
        CopyToOpenApiDocument(document);

        return document;
    }

    public void CopyToOpenApiDocument(OpenApiDocument document)
    {
        var apiPaths = new OpenApiPaths();
        var schemas = new Dictionary<string, OpenApiSchema>();

        var shouldPopulateParametersLookup = parametersLookup.Count == 0;
        var lookup = expositionLookup.GetLookup();
        var enumerable = lookup.Select(x => new Tuple<RequestInfo, object, MethodInfo>(x.Key, x.Value.Item1, x.Value.Item2))
            .OrderBy(x => x.Item1.ToString());
        var tuple = PopulateApiPathsAndSchemas(enumerable, apiPaths, schemas);
        if (shouldPopulateParametersLookup)
        {
            foreach (var kvp1 in tuple)
            {
                parametersLookup.Add(kvp1.Key, kvp1.Value);
            }
        }
        if (document.Servers == null)
        {
            document.Servers = [];
        }
        document.Servers.Add(new() { Url = expositionLookup.ApiBaseUrl });
        if (document.Paths == null)
        {
            document.Paths = [];
        }
        foreach (var apiPath in apiPaths)
        {
            document.Paths.Add(apiPath.Key, apiPath.Value);
        }
        if (document.Components == null)
        {
            document.Components = new OpenApiComponents();
        }
        if (document.Components.Schemas == null)
        {
            document.Components.Schemas = new Dictionary<string, OpenApiSchema>();
        }
        foreach (var kvp in schemas)
        {
            document.Components.Schemas.Add(kvp.Key, kvp.Value);
        }
    }

    [Expose(actionName: "openapi")]
    public string GetOpenApiSpecification()
    {
        if (openApiJson == null)
        {
            Initialize();
            openApiJson = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
        }

        return openApiJson;
    }

    public void MapRoutes(IRouteBuilder routeBuilder)
    {
        var apiBaseUrl = expositionLookup.ApiBaseUrl;
        var lookup = expositionLookup.GetLookup();
        foreach (var kvp in lookup)
        {
            var requestInfo = kvp.Key;
            var path = requestInfo.GetFullPath(apiBaseUrl);
            var httpMethod = requestInfo.HttpRequestMethod.MapToHttpMethod();
            routeBuilder.MapVerb(httpMethod, path, PerformOperationAsync);
        }
    }

    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var apiBaseUrl = expositionLookup.ApiBaseUrl;
        var lookup = expositionLookup.GetLookup();
        foreach (var kvp in lookup)
        {
            var requestInfo = kvp.Key;
            var path = requestInfo.GetFullPath(apiBaseUrl);
            var httpMethod = requestInfo.HttpRequestMethod.MapToHttpMethod();
            routeBuilder.MapMethods(path, [httpMethod], PerformOperationAsync);
        }
    }

    private RequestInfo BuildRequestInfo(HttpRequest request)
    {
        var path = request.Path.Value;
        path = path.Substring(expositionLookup.ApiBaseUrl.Length + 1);
        var index = path.LastIndexOf('/');
        var action = path.Substr(index + 1);
        if (string.IsNullOrEmpty(action))
        {
            action = null;
        }
        path = index > -1 ? path.Substring(0, index) : null;

        var httpRequestMethod = HttpMethodHelper.MapToHttpRequestMethod(request.Method);
        var requestInfo = new RequestInfo(httpRequestMethod, path, action);
        return requestInfo;
    }

    protected virtual async Task PerformOperationAsync(HttpContext httpContext)
    {
        Initialize();
        var request = httpContext.Request;
        var response = httpContext.Response;
        object objectToReturn;
        RequestTelemetry requestTelemetry = null;
        if (this.telemetryConfig != null && (this.telemetryConfig.LogRequestBody || this.telemetryConfig.LogResponseBody))
        {
            requestTelemetry = httpContext.Features.Get<RequestTelemetry>();
        }
        bool isLinq = request.Headers.ContainsKey(Net.NetDefaults.RemoteLinqIdHeaderName);
        JToken body = null;
        bool isResponseAnError = false;
        int statusCode = (int)HttpStatusCode.OK;
        try
        {
            var requestInfo = BuildRequestInfo(request);
            var httpRequestMethod = requestInfo.HttpRequestMethod;
            var returnType = this.expositionLookup.GetReturnType(requestInfo);
            //isLinq = httpRequestMethod == HttpRequestMethod.Post && returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IQueryable<>);

            body = await LogAndGetRequestBodyAsync(request, httpRequestMethod, requestTelemetry).ConfigureAwait(false);

            var jObject = JsonSerializationHelper.BuildJObjectFromQuery(request.Query);
            PopulateJObjectFromBody(requestInfo, jObject, body);

            jObject["__isLinq"] = isLinq;

            objectToReturn = expositionLookup.Execute(requestInfo, jObject);
            if (this.isResponceAnErrorFunc != null)
            {
                isResponseAnError = this.isResponceAnErrorFunc(objectToReturn);
            }
            if (objectToReturn is IExceptioned { Exception: not null } exceptioned)
            {
                var sc = ExceptionStatusCodeLookup.Get(exceptioned.Exception.GetType());
                if (sc != null)
                {
                    statusCode = sc.Value;
                }
            }
            response.StatusCode = statusCode;
        }
        catch (Exception ex)
        {
            response.StatusCode = GetStatusCodeForException(ex);
            objectToReturn = ex;
            isResponseAnError = true;
            isLinq = false;
        }
        await LogAndWriteResponseAsync(response, objectToReturn, isResponseAnError, isLinq, body, requestTelemetry).ConfigureAwait(false);
    }

    private async Task LogAndWriteResponseAsync(HttpResponse response, object objectToReturn, bool isResponseAnError, bool isLinq, JToken body, RequestTelemetry requestTelemetry)
    {
        bool isStringJson = false;
        if (this.telemetryConfig != null && requestTelemetry != null)
        {
            if (body != null && this.telemetryConfig.LogRequestBody && this.telemetryConfig.Condition == AlwaysOrOnError.OnError && isResponseAnError)
            {
                requestTelemetry.Context.GlobalProperties.Add("requestBody", body.ToString());
            }
            if (objectToReturn != null && this.telemetryConfig.LogResponseBody)
            {
                if (this.telemetryConfig.Condition == AlwaysOrOnError.Always || (this.telemetryConfig.Condition == AlwaysOrOnError.OnError && isResponseAnError))
                {
                    if (!(objectToReturn is string stringToReturn))
                    {
                        stringToReturn = JsonSerializationHelper.Serialize(objectToReturn, isLinq ? JsonSerializationHelper.LinqSerializer : JsonSerializationHelper.Serializer);
                        isStringJson = true;
                    }
                    requestTelemetry.Context.GlobalProperties.Add("responseBody", stringToReturn);
                    objectToReturn = stringToReturn;
                }
            }
        }
        if (objectToReturn != null)
        {
            if (objectToReturn is string asString)
            {
                if (isStringJson)
                {
                    response.ContentType = "application/json";
                }
                await response.WriteAsync(asString).ConfigureAwait(false);
            }
            else
            {
                await response.WriteAsJsonAsync(objectToReturn, objectToReturn.GetType(), isLinq ? JsonSettings.LinqJsonSerializerOptions : JsonSettings.RestJsonSerializerOptions).ConfigureAwait(false);
            }
        }
    }

    private async Task<JToken> LogAndGetRequestBodyAsync(HttpRequest request, HttpRequestMethod httpRequestMethod, RequestTelemetry requestTelemetry)
    {
        if (httpRequestMethod is HttpRequestMethod.Put or HttpRequestMethod.Post)
        {
            if (request.HasJsonContentType())
            {
                if (requestTelemetry != null && this.telemetryConfig is { LogRequestBody: true, Condition: AlwaysOrOnError.Always })
                {
                    var bodyString = await request.Body.ReadAsStringAsync().ConfigureAwait(false);
                    requestTelemetry.Context.GlobalProperties.Add("requestBody", bodyString);
                    return JToken.Parse(bodyString);
                }
                return await JsonSerializationHelper.BuildJTokenFromStreamAsync(request.Body, closeInput: true).ConfigureAwait(false);
            }
        }
        return null;
    }

    private static int GetStatusCodeForException(Exception ex)
    {
        if (ex == null)
        {
            return (int)HttpStatusCode.OK;
        }
        var statisCode = ExceptionStatusCodeLookup.Get(ex.GetType());
        if (statisCode != null)
        {
            return statisCode.Value;
        }
        if (ex is ExpositionException exp)
        {
            if (exp.StatusCode != null)
            {
                return exp.StatusCode.Value;
            }
        }
        else
        {
            if (ex is ArgumentException ae)
            {
                return (int)HttpStatusCode.BadRequest;
            }
            if (ex is HttpRequestException re)
            {
                if (re.StatusCode != null)
                {
                    return (int)re.StatusCode.Value;
                }
                return (int)HttpStatusCode.BadRequest;
            }
        }
        return (int)HttpStatusCode.InternalServerError;
    }

    private void PopulateJObjectFromBody(RequestInfo requestInfo, JObject jObject, JToken body)
    {
        parametersLookup.TryGetValue(requestInfo, out var lookup);
        if (body != null)
        {
            var bodyParameters = lookup.Where(x => x.Value == ParameterValueLocation.Body).ToList();
            if (bodyParameters.Count == 0)
            {
                jObject["__body"] = body;
            }
            else
            {
                if (bodyParameters.Count == 1)
                {
                    var bodyParameter = bodyParameters[0].Key;
                    jObject[bodyParameter.Name] = body;
                }
                else
                {
                    var bodyAsJObject = body as JObject;

                    foreach (var kvp in bodyParameters)
                    {
                        var bodyParameter = kvp.Key;
                        var parameterName = bodyParameter.Name;
                        if (bodyAsJObject.TryGetValue(parameterName, out var value))
                        {
                            jObject[parameterName] = value;
                        }
                    }
                }
            }
        }
    }

    protected override void InitializeProtected()
    {
        openApiDocument = BuildOpenApiDocument();
    }
}