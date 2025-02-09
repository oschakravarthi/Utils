using Remote.Linq;
using Remote.Linq.Expressions;
using Remote.Linq.ExpressionVisitors;
using SubhadraSolutions.Utils.Validation;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace SubhadraSolutions.Utils.ApiClient.Helpers;

internal static class RemoteQueryProviderExtensions
{
    public static Expression TranslateExpression(this System.Linq.Expressions.Expression expression,
        IExpressionToRemoteLinqContext context = null)
    {
        Guard.ArgumentShouldNotBeNull(expression, nameof(expression));
        var rexp = expression.SimplifyIncorporationOfRemoteQueryables().ToRemoteLinqExpression(context)
            .ReplaceQueryableByResourceDescriptors(context?.TypeInfoProvider)
            .ReplaceGenericQueryArgumentsByNonGenericArguments();

        return rexp;
    }

    internal static TResult InvokeAndUnwrap<TResult>(this IRemoteQueryProvider queryProvider, MethodInfo method,
        System.Linq.Expressions.Expression expression)
    {
        return method
            .MakeGenericMethod(typeof(IQueryable).IsAssignableFrom(expression.Type)
                ? typeof(object)
                : expression.Type).InvokeAndUnwrap<TResult>(queryProvider, expression);
    }

    internal static TResult InvokeAndUnwrap<TResult>(this MethodInfo methodInfo, object instance,
        params object[] args)
    {
        return (TResult)methodInfo.InvokeAndUnwrap(instance, args);
    }

    internal static object InvokeAndUnwrap(this MethodInfo methodInfo, object instance, params object[] args)
    {
        try
        {
            return methodInfo.Invoke(instance, args);
        }
        catch (TargetInvocationException ex2)
        {
            ExceptionDispatchInfo.Capture(Unwrap(ex2)).Throw();
            throw;
        }

        static Exception Unwrap(Exception ex)
        {
            while (ex.InnerException != null && ex is TargetInvocationException) ex = ex.InnerException;
            return ex;
        }
    }
}