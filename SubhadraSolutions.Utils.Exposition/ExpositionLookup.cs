using Newtonsoft.Json.Linq;
using Remote.Linq.ExpressionExecution;
using SubhadraSolutions.Utils.Json;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading.Tasks;
using SubhadraSolutions.Utils.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Exposition;

public class ExpositionLookup(string apiBaseUrl) : IExpositionLookup
{
    protected readonly Dictionary<RequestInfo, Tuple<object, MethodInfo>> lookup = [];

    public string ApiBaseUrl { get; } = apiBaseUrl;

    public virtual object Execute(RequestInfo requestInfo, JObject actionArguments)
    {
        var result = ExecuteCore(requestInfo, actionArguments);
        return result;
    }

    public IReadOnlyDictionary<RequestInfo, Tuple<object, MethodInfo>> GetLookup()
    {
        return lookup.AsReadOnly();
    }

    public Type GetReturnType(RequestInfo requestInfo)
    {
        if (!lookup.TryGetValue(requestInfo, out var tuple))
        {
            return null;
        }

        var method = tuple.Item2;
        if (method != null)
        {
            return method.ReturnType;
        }
        return tuple.Item1.GetType();
    }

    public void RegisterMethods(string path, object obj)
    {
        Guard.ArgumentShouldNotBeNull(obj, nameof(obj));
        path = string.IsNullOrWhiteSpace(path) ? null : path.Trim();

        var objIsType = false;
        if (obj is Type type)
        {
            objIsType = true;
        }
        else
        {
            type = obj.GetType();
        }

        var bindingFlags = objIsType ? BindingFlags.Static : BindingFlags.Instance;
        var methods = type.GetMethods(bindingFlags | BindingFlags.Public).Where(m => !m.IsGenericMethod);

        var anyMethodsExposed = false;
        foreach (var method in methods)
        {
            var exposeAttribute = method.GetAttribute<ExposeAttribute>(true, true);
            var returnType = method.ReturnType;
            if (exposeAttribute == null)
            {
                continue;
            }

            var actionName = exposeAttribute.ActionName;
            if (actionName == null)
            {
                var suffix = "Async";
                actionName = method.Name;
                if (actionName.EndsWith(suffix))
                {
                    actionName = actionName.Substr(0, actionName.Length - suffix.Length);
                }
            }

            if (exposeAttribute.SubPath != null)
            {
                path = path == null ? exposeAttribute.SubPath : path + '/' + exposeAttribute.SubPath;
            }

            var queryableElementType = GetQueryableElementType(returnType);
            var canAdd = true;
            if (queryableElementType != null)
            {
                if (exposeAttribute.HttpRequestMethod is HttpRequestMethod.Get or HttpRequestMethod.Post)
                {
                    var ri = new RequestInfo(HttpRequestMethod.Post, path, actionName);
                    lookup.Add(ri, new Tuple<object, MethodInfo>(obj, method));
                }
                if (exposeAttribute.HttpRequestMethod == HttpRequestMethod.Post)
                {
                    canAdd = false;
                }
            }
            if (canAdd)
            {
                var requestInfo = new RequestInfo(exposeAttribute.HttpRequestMethod, path, actionName);
                lookup.Add(requestInfo, new Tuple<object, MethodInfo>(obj, method));
            }
            anyMethodsExposed = true;
        }

        if (!anyMethodsExposed)
        {
            throw new AbandonedMutexException(
                "No methods found to expose. Did you miss to decorate the method with ExposeAttribute?");
        }
    }

    private static Type GetQueryableElementType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>))
        {
            return type.GetGenericArguments()[0];
        }

        return null;
    }

    public void RegisterObject(string path, object obj)
    {
        path = string.IsNullOrWhiteSpace(path) ? null : path.Trim();
        Guard.ArgumentShouldNotBeNull(obj, nameof(obj));

        if (obj is Type)
        {
            throw new InvalidOperationException("Type cannot be registered as object.");
        }
        var requestInfo = new RequestInfo(HttpRequestMethod.Get, path, null);
        this.lookup.Add(requestInfo, new Tuple<object, MethodInfo>(obj, null));
    }

    private object ExecuteCore(RequestInfo requestInfo, JObject actionArguments)
    {
        if (!lookup.TryGetValue(requestInfo, out var tuple))
        {
            throw new ExpositionException((int)HttpStatusCode.NotFound, "No action for this request");
        }

        var obj = tuple.Item1;
        var objIsType = false;
        if (obj is Type type)
        {
            objIsType = true;
        }
        else
        {
            type = obj.GetType();
        }
        var isLinq = (bool)((JValue)actionArguments["__isLinq"]).Value;
        var serializer = isLinq ? JsonSerializationHelper.LinqSerializer : JsonSerializationHelper.Serializer;
        if (isLinq)
        {
        }
        var method = tuple.Item2;
        object result;
        if (method == null)
        {
            result = obj;
        }
        else
        {
            var args = PrepareArgumentsForMethod(requestInfo, type, method, actionArguments, serializer);
            result = method.Invoke(objIsType ? null : obj, args);

            if (requestInfo.HttpRequestMethod == HttpRequestMethod.Post)
            {
                var queryableElementType = GetQueryableElementType(method.ReturnType);
                if (queryableElementType != null)
                {
                    var body = actionArguments["__body"];
                    var remoteLinqExp = body.ToObject<Remote.Linq.Expressions.Expression>(JsonSerializationHelper.LinqSerializer);
                    result = remoteLinqExp.Execute((t) => (IQueryable)result);
                }
            }
        }
        if (TaskHelper.IsValueTask(result))
        {
            result = TaskHelper.GetObjectFromValueTask(result);
        }
        if (result is Task task)
        {
            result = TaskHelper.GetResultFromTask(task);
        }

        return result;
    }

    private object[] PrepareArgumentsForMethod(RequestInfo requestInfo, Type objectType, MethodInfo method,
        JObject actionArguments, Newtonsoft.Json.JsonSerializer serializer)
    {
        var foundArguments = new HashSet<int>();
        var parameters = method.GetParameters();
        var arguments = new object[parameters.Length];

        StringBuilder sb = null;
        for (var i = 0; i < parameters.Length; i++)
        {
            if (foundArguments.Contains(i))
            {
                continue;
            }

            var parameter = parameters[i];
            object arg;
            bool foundKey;
            JToken value = null;
            try
            {
                arg = actionArguments.BuildObjectFromJObject(parameter.Name, parameter.ParameterType, out foundKey, out value, serializer);
            }
            catch (Exception ex)
            {
                throw new ExpositionException((int)HttpStatusCode.BadRequest, $"Error while parsing {value} for argument {parameter.Name} of type {parameter.ParameterType.FullName}", ex);
            }

            if (foundKey)
            {
                arguments[i] = arg;
                foundArguments.Add(i);
            }
            else
            {
                if (parameter.HasDefaultValue)
                {
                    arguments[i] = parameter.DefaultValue;
                    foundArguments.Add(i);
                }
                else
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    sb.Append(parameter.Name);
                }
            }
        }

        if (foundArguments.Count == parameters.Length)
        {
            return arguments;
        }

        var message = $"No argument(s) provided for the parameter(s): {sb} for the action {requestInfo}";
        var typeOrServiceName = objectType == null ? null : objectType.ToString();
        throw new ExpositionException((int)HttpStatusCode.BadRequest, typeOrServiceName, requestInfo.ActionName,
            message, null);
    }
}