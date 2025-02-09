using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public interface IQueryableLookup
{
    public IQueryable<T> GetQueryable<T>();

    public IQueryable GetQueryable(Type entityType);

    [DynamicallyInvoked]
    public void RegisterQueryableFactory<T>(Func<IQueryable<T>> queryableFactory);
}