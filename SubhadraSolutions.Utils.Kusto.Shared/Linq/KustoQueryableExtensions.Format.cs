using SubhadraSolutions.Utils.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public static partial class KustoQueryableExtensions
{
    // private static IQueryable<T> Format<T>(Expression[] expressions)
    // {
    //    Console.WriteLine("In Dummy Format");
    //    throw new NotImplementedException();
    // }

    public static IQueryable<T> Format<T>(this IQueryable<T> source, params object[] arguments)
    {
        var method = LinqFakeMethods.GetMethodInfo(Format, source, arguments);

        return source.Provider.CreateQuery<T>(Expression.Call(null, method,
        [
            source.Expression,
            Expression.Constant(arguments)
        ]));
    }
}