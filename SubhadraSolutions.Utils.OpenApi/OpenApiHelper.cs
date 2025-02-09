using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SubhadraSolutions.Utils.Exposition;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.OpenApi;

public static class OpenApiHelper
{
    public enum ParameterValueLocation
    {
        Query,
        Body
    }

    private static readonly Dictionary<Type, string> longTypeNamesLookup = [];

    public static Tuple<OpenApiOperation, Dictionary<ParameterInfo, ParameterValueLocation>> BuildApiOperation(
        object obj, MethodInfo method, RequestInfo requestInfo, IDictionary<Type, OpenApiSchema> typesLookup)
    {
        var returnType = method == null ? obj.GetType() : method.ReturnType;
        BuildOpenApiSchema(returnType, typesLookup, null);
        var mediaType = new OpenApiMediaType
        {
            Schema = BuildOpenApiSchema(returnType, typesLookup, null)
        };
        var parameters = method == null ? Array.Empty<ParameterInfo>() : method.GetParameters();
        var tuple = BuildApiParameters(parameters, requestInfo, typesLookup, null);
        var apiOperation = new OpenApiOperation
        {
            Description = method == null ? null : method.GetMemberDescription(),
            Responses = new OpenApiResponses
            {
                ["200"] = new()
                {
                    Description = "OK",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = mediaType
                    }
                }
            },

            Parameters = tuple.Item1,
            RequestBody = tuple.Item2
        };
        return new Tuple<OpenApiOperation, Dictionary<ParameterInfo, ParameterValueLocation>>(apiOperation,
            tuple.Item3);
    }

    public static Dictionary<RequestInfo, Dictionary<ParameterInfo, ParameterValueLocation>> PopulateApiPathsAndSchemas(
        IEnumerable<Tuple<RequestInfo, object, MethodInfo>> actions, OpenApiPaths apiPaths,
        IDictionary<string, OpenApiSchema> schemas)
    {
        var result = new Dictionary<RequestInfo, Dictionary<ParameterInfo, ParameterValueLocation>>();
        var typesLookup = new Dictionary<Type, OpenApiSchema>();
        var groups = actions.GroupBy(x => x.Item1.GetFullPath(null));
        foreach (var group in groups)
        {
            var apiOperations = new Dictionary<OperationType, OpenApiOperation>();

            foreach (var action in group)
            {
                var requestInfo = action.Item1;
                var obj = action.Item2;
                var method = action.Item3;
                var tuple = BuildApiOperation(obj, method, requestInfo, typesLookup);

                var actionName = requestInfo.ActionName;
                var operationType = MapHttpRequestMethod(requestInfo.HttpRequestMethod);

                result.Add(requestInfo, tuple.Item2);
                var apiOperation = tuple.Item1;
                apiOperations.Add(operationType, apiOperation);
                //var actionPath = '/' + actionName;
                //if (requestInfo.Path != null)
                //{
                //    actionPath = $"/{requestInfo.Path}/{actionName}";
                //}
            }
            var apiPathItem = new OpenApiPathItem
            {
                Operations = apiOperations
            };

            apiPaths.Add(group.Key, apiPathItem);
        }
        foreach (var kvp1 in typesLookup)
        {
            var typeName = Encode(kvp1.Key);
            if (!schemas.ContainsKey(typeName))
            {
                schemas.Add(typeName, kvp1.Value);
            }
        }

        return result;
    }

    private static Tuple<List<OpenApiParameter>, OpenApiRequestBody, Dictionary<ParameterInfo, ParameterValueLocation>>
        BuildApiParameters(ParameterInfo[] parameters, RequestInfo requestInfo, IDictionary<Type, OpenApiSchema> typesLookup,
            MemberInfo member)
    {
        var locations = new Dictionary<ParameterInfo, ParameterValueLocation>();
        var apiParameters = new List<OpenApiParameter>();
        var inList = new List<ParameterInfo>();
        var parametersForBody = new List<ParameterInfo>();
        var maybeList = new List<ParameterInfo>();
        foreach (var parameter in parameters)
        {
            // if (parameterType.IsValueType)
            // {
            //    required = true;

            //    if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            //    {
            //        required = false;
            //    }
            // }

            if (requestInfo.HttpRequestMethod is HttpRequestMethod.Post or HttpRequestMethod.Put)
            {
                var canBeInQuery = CanBeInQuery(parameter.ParameterType);
                if (canBeInQuery == null)
                {
                    maybeList.Add(parameter);
                    continue;
                }

                if (!canBeInQuery.Value)
                {
                    parametersForBody.Add(parameter);
                    continue;
                    //place=ParameterLocation.
                }
            }

            inList.Add(parameter);
        }

        if (parametersForBody.Count > 0)
        {
            inList.AddRange(maybeList);
        }
        else
        {
            parametersForBody.AddRange(maybeList);
        }

        foreach (var parameter in inList)
        {
            var required = !parameter.HasDefaultValue;

            var apiParameter = new OpenApiParameter
            {
                Name = parameter.Name,
                In = ParameterLocation.Query,
                Schema = BuildOpenApiSchema(parameter.ParameterType, typesLookup, member),
                Required = required
            };

            apiParameters.Add(apiParameter);
            locations.Add(parameter, ParameterValueLocation.Query);
        }

        if (parametersForBody.Count == 0)
        {
            return new Tuple<List<OpenApiParameter>, OpenApiRequestBody,
                Dictionary<ParameterInfo, ParameterValueLocation>>(apiParameters, null, locations);
        }

        Type bodyType = null;
        if (parametersForBody.Count == 1)
        {
            bodyType = parametersForBody[0].ParameterType;
            locations.Add(parametersForBody[0], ParameterValueLocation.Body);
        }
        else
        {
            bodyType = AnonymousTypeBuilder.BuildAnonymousType(
                parametersForBody.Select(x => new Tuple<string, Type>(x.Name, x.ParameterType)));

            foreach (var p in parametersForBody)
            {
                locations.Add(p, ParameterValueLocation.Body);
            }
        }

        var mediaType = new OpenApiMediaType
        {
            Schema = BuildOpenApiSchema(bodyType, typesLookup, null)
        };
        var body = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>()
        };
        body.Content.Add("application/json", mediaType);

        return new Tuple<List<OpenApiParameter>, OpenApiRequestBody, Dictionary<ParameterInfo, ParameterValueLocation>>(
            apiParameters, body, locations);
    }

    private static OpenApiSchema BuildBasicSchema(Type parameterType, MemberInfo member)
    {
        var isNullable = false;
        var schema = new OpenApiSchema();

        if (parameterType.IsGenericType)
        {
            var genericTypeDefinition = parameterType.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(Task<>) || genericTypeDefinition == typeof(Nullable<>))
            {
                parameterType = parameterType.GetGenericArguments()[0];

                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    isNullable = true;
                }
            }
        }

        if (isNullable)
        {
            schema.Nullable = true;
        }

        if (parameterType == typeof(string) || parameterType == typeof(char))
        {
            schema.Type = "string";

            if (member == null)
            {
                return schema;
            }

            var minLengthAttribute = member.GetCustomAttribute<MinLengthAttribute>(true);

            if (minLengthAttribute != null)
            {
                schema.MinLength = minLengthAttribute.Length;
            }

            var maxLengthAttribute = member.GetCustomAttribute<MaxLengthAttribute>(true);

            if (maxLengthAttribute != null)
            {
                schema.MaxLength = maxLengthAttribute.Length;
            }

            if (member.IsDefined(typeof(EmailAddressAttribute), true))
            {
                schema.Format = "email";
                return schema;
            }

            if (member.IsDefined(typeof(UrlAttribute), true))
            {
                schema.Format = "uri";
                return schema;
            }

            return schema;
        }

        if (parameterType == typeof(Guid))
        {
            schema.Type = "string";
            schema.Format = "uuid";
            return schema;
        }

        if (parameterType.IsNumericType())
        {
            if (member != null)
            {
                var rangeAttribute = member.GetCustomAttribute<RangeAttribute>(true);

                if (rangeAttribute != null)
                {
                    schema.Minimum = (decimal?)rangeAttribute.Minimum;
                    schema.Maximum = (decimal?)rangeAttribute.Maximum;
                }
            }

            if (parameterType.IsIntegerType())
            {
                schema.Type = "integer";

                if (parameterType == typeof(int))
                {
                    schema.Format = "int32";
                    return schema;
                }

                if (parameterType == typeof(long))
                {
                    schema.Format = "int64";
                }

                return schema;
            }

            schema.Type = "number";

            if (parameterType == typeof(float))
            {
                schema.Format = "float";
                return schema;
            }

            if (parameterType == typeof(double))
            {
                schema.Format = "double";
                return schema;
            }

            return schema;
        }

        if (parameterType == typeof(bool))
        {
            schema.Type = "boolean";
            return schema;
        }

        if (parameterType.IsEnumerableType())
        {
            schema.Type = "array";
            return schema;
        }

        if (parameterType.IsDateOrTimeType())
        {
            schema.Type = "string";
            schema.Format = "date";
            return schema;
        }

        schema.Type = "object";
        return schema;
    }

    private static OpenApiReference BuildComponentReference(Type type)
    {
        var id = Encode(type);
        //string id = type.ToHumanReadableString();
        return new OpenApiReference
        {
            Id = id,
            Type = ReferenceType.Schema
        };
    }

    //    return new()
    //    {
    //        Id = id,
    //        Type = ReferenceType.Schema
    //    };
    //}
    private static OpenApiSchema BuildOpenApiSchema(Type type, IDictionary<Type, OpenApiSchema> typesLookup,
        MemberInfo member)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            type = type.GetGenericArguments()[0];
        }

        if (typesLookup.TryGetValue(type, out var schema))
        // return schema;
        {
            return new OpenApiSchema
            {
                Type = schema.Type,
                Reference = BuildComponentReference(type)
            };
        }

        return BuildOpenApiSchemaCore(type, typesLookup, member);
    }

    //private static OpenApiReference BuildComponentReference(Type type, string ns)
    //{
    //    string id = string.Format("{0}.{1}", ns, type.Name);
    private static OpenApiSchema BuildOpenApiSchemaCore(Type type, IDictionary<Type, OpenApiSchema> typesLookup,
        MemberInfo member)
    {
        if (typesLookup.TryGetValue(type, out var schema))
        {
            return schema;
        }

        if (type.IsEnum)
        {
            return BuildSchemaForEnum(type);
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            schema = new OpenApiSchema
            {
                Type = "object",
            };
            if (type.IsGenericType)
            {
                var genericTypeArguments = type.GenericTypeArguments;

                if (genericTypeArguments.Length == 2)
                {
                    //var keyType = genericTypeArguments[0];
                    var valueType = genericTypeArguments[1];
                    schema.AdditionalProperties = BuildOpenApiSchema(valueType, typesLookup, null);
                }
            }
            return schema;
        }

        schema = BuildBasicSchema(type, member);

        if (schema.Type == "array")
        {
            var elementType = type.GetEnumerableItemType();
            BuildOpenApiSchema(elementType, typesLookup, member);
            schema.Items = BuildOpenApiSchema(elementType, typesLookup, member);
            return schema;
        }

        if (schema.Type != "object")
        {
            return schema;
        }

        if (typeof(Exception).IsAssignableFrom(type))
        {
            return schema;
        }

        typesLookup.Add(type, schema);

        var requiredProperties = new HashSet<string>();
        foreach (var property in type.GetProperties())
        {
            var propertySchema = BuildOpenApiSchema(property.PropertyType, typesLookup, property);
            schema.Properties.Add(property.Name, propertySchema);

            if (property.IsDefined(typeof(RequiredAttribute), true))
            {
                requiredProperties.Add(property.Name);
            }
        }

        schema.Required = requiredProperties;
        return schema;
    }

    private static OpenApiSchema BuildSchemaForEnum(Type enumType)
    {
        var schema = new OpenApiSchema
        {
            Enum = [],
            Type = "string"
        };

        foreach (var name in Enum.GetNames(enumType))
        {
            schema.Enum.Add(new OpenApiString(name));
        }

        return schema;
    }

    private static bool? CanBeInQuery(Type parameterType)
    {
        if (parameterType.IsPrimitiveOrExtendedPrimitive())
        {
            return true;
        }

        var itemType = parameterType.GetEnumerableItemType();

        if (itemType == null)
        {
            return false;
        }

        if (itemType.IsPrimitiveOrExtendedPrimitive())
        {
            return null;
        }

        return false;
    }

    private static string Encode(Type type)
    {
        var name = type.FullName;
        if (!type.IsGenericType)
        {
            return name;
        }

        if (longTypeNamesLookup.TryGetValue(type, out var alias))
        {
            return alias;
        }

        if (name.Length > 100)
        {
            name = type.Name + GeneralHelper.Identity;
        }

        var chars = name.ToCharArray();
        var modified = false;
        for (var i = 0; i < chars.Length; i++)
        {
            var c = chars[i];
            if (!char.IsDigit(c) && !char.IsAsciiLetter(c) && c != '-' && c != '.' && c != '_')
            {
                chars[i] = '_';
                modified = true;
            }
        }

        if (modified)
        {
            name = new string(chars);
        }

        longTypeNamesLookup.Add(type, name);

        return name;
        //return HttpUtility.UrlEncode(type.FullName);
    }

    private static OperationType MapHttpRequestMethod(HttpRequestMethod httpRequestMethod)
    {
        switch (httpRequestMethod)
        {
            case HttpRequestMethod.Get:
                return OperationType.Get;

            case HttpRequestMethod.Put:
                return OperationType.Put;

            case HttpRequestMethod.Post:
                return OperationType.Post;

            default:
                return OperationType.Delete;
        }
    }
}