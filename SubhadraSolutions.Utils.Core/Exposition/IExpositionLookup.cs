using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SubhadraSolutions.Utils.Exposition;

public interface IExpositionLookup
{
    string ApiBaseUrl { get; }

    object Execute(RequestInfo requestInfo, JObject actionArguments);

    IReadOnlyDictionary<RequestInfo, Tuple<object, MethodInfo>> GetLookup();

    Type GetReturnType(RequestInfo requestInfo);

    void RegisterMethods(string path, object obj);

    void RegisterObject(string path, object obj);

    //Tuple<object, MethodInfo> GetObjectAndMethodInfo(RequestInfo requestInfo);
}