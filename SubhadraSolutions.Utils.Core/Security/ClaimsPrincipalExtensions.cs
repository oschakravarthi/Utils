using System;
using System.Security.Claims;

namespace SubhadraSolutions.Utils.Security;

public static class ClaimsPrincipalExtensions
{
    public static string GetFirstAsString(this ClaimsPrincipal principal, string type)
    {
        return principal.GetFirstValue<string>(type);
    }

    public static T GetFirstValue<T>(this ClaimsPrincipal principal, string type)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        var value = principal.FindFirst(type);

        if (value == null)
        {
            return default;
        }

        var returnType = typeof(T);
        //var valueType = value.GetType();
        //if (returnType.IsAssignableFrom(valueType))
        //{
        //    return (T)value;
        //}
        if (returnType == typeof(string))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        if (returnType == typeof(int) || returnType == typeof(long))
        {
            return value != null ? (T)Convert.ChangeType(value, returnType) : (T)Convert.ChangeType(0, returnType);
        }

        throw new Exception("Invalid type provided");
    }

    public static string GetLoggedInUserEmail(this ClaimsPrincipal principal)
    {
        return principal.GetFirstAsString(ClaimTypes.Email);
    }

    public static string GetLoggedInUserId(this ClaimsPrincipal principal)
    {
        return principal.GetFirstAsString(ClaimTypes.NameIdentifier);
    }

    public static string GetLoggedInUserName(this ClaimsPrincipal principal)
    {
        return principal.GetFirstAsString(ClaimTypes.Name);
    }
}