using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Linq;

public static class QueryUtils
{
    private static readonly MethodInfo RegisterQueryableFactoryTemplateMethod =
        typeof(IQueryableLookup).GetMethod(nameof(IQueryableLookup.RegisterQueryableFactory),
            BindingFlags.Public | BindingFlags.Instance);

    //public static IQueryable<T> ApplyDateTimeRangeClause<T>(this IQueryable<T> queryable, DateTime? from, DateTime? upto) where T : IDated
    //{
    //    if (from != null)
    //    {
    //        queryable = queryable.Where(x => x.Date >= from.Value);
    //    }

    //    if (upto != null)
    //    {
    //        queryable = queryable.Where(x => x.Date < upto.Value);
    //    }

    //    //if (from != null && upto != null)
    //    //{
    //    //    queryable = queryable;
    //    //}
    //    return queryable;
    //}

    public static void RegisterQueryableLookupsFromObject(IQueryableLookup queryableLookup, object queryableFactory)
    {
        var methods = GetBaseQueryableMethods(queryableFactory.GetType());
        Validate(methods);
        foreach (var method in methods)
        {
            var elementType = method.ReturnType.GetGenericArguments()[0];
            //var funcType = typeof(Func<>).MakeGenericType(typeof(IQueryable<>).MakeGenericType(elementType));
            var funcType = typeof(Func<>).MakeGenericType(method.ReturnType);

            var func = Expression.Lambda(funcType, Expression.Call(Expression.Constant(queryableFactory), method))
                .Compile();

            RegisterQueryableFactoryTemplateMethod.MakeGenericMethod(elementType)
                .Invoke(queryableLookup, [func]);
        }
    }

    private static IEnumerable<MethodInfo> GetBaseQueryableMethods(Type queryableFactoryType)
    {
        foreach (var method in queryableFactoryType.GetMethods().Where(m =>
                     m.DeclaringType == queryableFactoryType && m.GetParameters().Length == 0 &&
                     m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>)))
        {
            yield return method;
        }
    }

    private static void Validate(IEnumerable<MethodInfo> methods)
    {
        var hashset = new HashSet<Type>();
        foreach (var method in methods)
        {
            var elementType = method.ReturnType.GetGenericArguments()[0];
            if (hashset.Contains(elementType))
            {
                throw new AmbiguousMatchException("There is a queryable exists already with the type " +
                                                  elementType.FullName);
            }

            hashset.Add(elementType);
        }
    }
}