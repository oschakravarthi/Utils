using SubhadraSolutions.Utils.Collections.Concurrent;

namespace SubhadraSolutions.Utils.Threading;

public abstract class AbstractFlyweightBasedDispatcherProvider : IDispatcherProvider
{
    private readonly GenericFlyweight<IDispatcher> flyweight;

    protected AbstractFlyweightBasedDispatcherProvider(bool useWeakReferences)
    {
        flyweight = new GenericFlyweight<IDispatcher>(GetDispatcherCore, useWeakReferences);
    }

    public IDispatcher GetDispatcher(object target)
    {
        return flyweight.GetObject(target);
    }

    protected abstract IDispatcher GetDispatcherCore(object target);
}