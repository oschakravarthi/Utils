using Remote.Linq.Async;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.ApiClient.Helpers;

public static class AsyncQueryableHelper
{
    //private static readonly MethodInfo RegisterQueryableLookupTemplateMethod =
    //    typeof(AsyncQueryableHelper).GetMethod(nameof(RegisterQueryableLookup),
    //        BindingFlags.Static | BindingFlags.Public);

    private static readonly MethodInfo ToListAsyncTemplateMethod =
        typeof(AsyncQueryableExtensions).GetMethod(nameof(AsyncQueryableExtensions.ToListAsync));

    //

    public static async Task<IList> GetList(this IQueryable queryable, bool internItems = false)
    {
        var elementType = LinqHelper.GetLinqElementType(queryable);
        var toListAsyncMethod = ToListAsyncTemplateMethod.MakeGenericMethod(elementType);

        var data = await InvokeAsync(toListAsyncMethod, null, queryable, null).ConfigureAwait(false);
        if (internItems)
        {
            InternItems(data);
        }

        return (IList)data;
    }

    public static async Task<List<T>> GetList<T>(this IQueryable queryable)
    {
        var list = await queryable.GetList().ConfigureAwait(false);
        return (List<T>)list;
    }

    //public static void RegisterQueryableLookup<T>(IQueryableLookup queryableLookup,
    //    Func<Expression, ValueTask<DynamicObject>> postDataAsync)
    //{
    //    queryableLookup.RegisterQueryableFactory(() =>
    //        SmartRemoteQueryable.Factory.CreateAsyncQueryable<T>(postDataAsync));
    //}

    //public static void RegisterQueryableLookupsFromQueryableFactory(IQueryableLookup queryableLookup,
    //    object queryableFactory, Func<Expression, ValueTask<DynamicObject>> postDataAsync)
    //{
    //    foreach (var method in queryableFactory.GetType().GetMethods().Where(m =>
    //                 m.GetParameters().Length == 0 && m.ReturnType.IsGenericType &&
    //                 m.ReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>)))
    //    {
    //        var elementType = method.ReturnType.GetGenericArguments()[0];
    //        RegisterQueryableLookupTemplateMethod.MakeGenericMethod(elementType)
    //            .Invoke(null, new object[] { queryableLookup, postDataAsync });
    //    }
    //}

    private static void InternItems(object data)
    {
        if (data == null)
        {
            return;
        }

        var elementType = data.GetType().GetEnumerableItemType();
        if (elementType == null)
        {
            return;
        }

        var method = InternExtensions.InternItemsTemplateMethod.MakeGenericMethod(elementType);
        method.Invoke(null, [data]);
    }

    private static async Task<object> InvokeAsync(MethodInfo method, object obj, params object[] parameters)
    {
        var valueTask = method.Invoke(obj, parameters);
        var data = await TaskHelper.GetObjectFromValueTask(valueTask).ConfigureAwait(false);
        return data;
    }

    //public static async Task<object> GetList(this IQueryable queryable, bool internItems = false)
    //{
    //    Type elementType = LinqHelper.GetLinqElementType(queryable);
    //    MethodInfo toListAsyncMethod = ToListAsyncTemplateMethod.MakeGenericMethod(elementType);

    //    object valueTask = toListAsyncMethod.Invoke(null, new[] { queryable, null });

    //    object data = await valueTask.GetObject().ConfigureAwait(false);
    //    if (internItems)
    //    {
    //        InternItems(data);
    //    }

    //    return data;
    //}
    //public static async Task<TTo> GetListAs<T, TTo>(this IQueryable<T> queryable, Func<List<T>, TTo> mapper)
    //{
    //    Type elementType = LinqHelper.GetLinqElementType(queryable);
    //    MethodInfo toListAsyncMethod = ToListAsyncTemplateMethod.MakeGenericMethod(elementType);

    //    object data = await InvokeAsync(toListAsyncMethod, null, new[] { queryable, null });

    //    var typed = (List<T>)data;

    //    var mapped = mapper(typed);
    //    return mapped;
    //}
    //public static async Task<object> GetList(this IQueryable queryable, bool internItems = false)
    //{
    //    Type elementType = LinqHelper.GetLinqElementType(queryable);
    //    MethodInfo toListAsyncMethod = ToListAsyncTemplateMethod.MakeGenericMethod(elementType);

    //    object data = await InvokeAsync(toListAsyncMethod, null, new[] { queryable, null });
    //    if (internItems)
    //    {
    //        InternItems(data);
    //    }

    //    return data;
    //}
    //public static async Task<List<T>> GetListMappedAs<T>(this IQueryable queryable) where T : new()
    //{
    //    var list = await GetList(queryable);
    //    var elementType = ReflectionHelper.GetEnumerableItemType(list.GetType());
    //    var method = typeof(DynamicCopyHelperExtensions).GetMethod(nameof(DynamicCopyHelperExtensions.MapItems)).MakeGenericMethod(elementType, typeof(T));
    //    var mapped = (IEnumerable<T>)method.Invoke(null, new object[] { list });
    //    var result = mapped.ToList();
    //    return result;
    //}
}