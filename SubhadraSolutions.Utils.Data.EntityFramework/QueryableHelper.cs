//using Microsoft.EntityFrameworkCore;
//using SubhadraSolutions.Utils.Linq;
//using SubhadraSolutions.Utils.Reflection;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace SubhadraSolutions.Utils.EF
//{
//    public static class QueryableUtils
//    {
//        public static void RegisterQueryableFactoriesFromDbContext<TDbContext>(IQueryableLookup queryableLookup, Func<TDbContext> dbContextFactory) where TDbContext : DbContext
//        {
//            var dbContextType = typeof(TDbContext);
//            var registerMethod = queryableLookup.GetType().GetMethod(nameof(IQueryableLookup.RegisterQueryableFactory), BindingFlags.Public | BindingFlags.Instance);
//            foreach (var property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
//            {
//                var propertyType = property.PropertyType;
//                if (property.CanRead && propertyType is IQueryable && propertyType.IsGenericType)
//                {
//                    var genericArguments = propertyType.GetGenericArguments();
//                    if (genericArguments.Length == 1)
//                    {
//                        var entityType = genericArguments[0];
//                        var returnType = typeof(IQueryable<>).MakeGenericType(entityType);
//                        var propertyAccessorFunc = ReflectionHelper.BuildPropertyAccessor(dbContextType, property, returnType);

//                        var callExpression = Expression.Call(dbContextFactory.Method);
//                        callExpression = Expression.Call(propertyAccessorFunc.Method, callExpression);

//                        var funcType = typeof(Func<>).MakeGenericType(typeof(IQueryable<>).MakeGenericType(entityType));
//                        Delegate func = Expression.Lambda(funcType, callExpression).Compile();

//                        registerMethod.MakeGenericMethod(entityType).Invoke(queryableLookup, new object[] { func });

//                    }
//                }
//            }
//        }
//    }
//}