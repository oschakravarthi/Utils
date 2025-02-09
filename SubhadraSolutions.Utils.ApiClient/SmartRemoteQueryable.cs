using Aqua.TypeSystem;
using Remote.Linq;
using System;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.ApiClient;

public static class SmartRemoteQueryable
{
    //
    // Summary:
    //     Gets a factory for creating System.Linq.IQueryable`1 (or System.Linq.IQueryable
    //     respectively) suited for remote execution.
    //
    //public static SmartRemoteQueryableFactory Factory { get; } = new();

    internal static ExpressionTranslatorContext GetExpressionTranslatorContextOrNull(
        ITypeInfoProvider typeInfoProvider, Func<Expression, bool> canBeEvaluatedLocally)
    {
        if (typeInfoProvider != null || canBeEvaluatedLocally != null)
        {
            return new ExpressionTranslatorContext(typeInfoProvider, canBeEvaluatedLocally);
        }

        return null;
    }
}