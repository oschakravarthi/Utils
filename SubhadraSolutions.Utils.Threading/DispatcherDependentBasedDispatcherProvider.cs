namespace SubhadraSolutions.Utils.Threading;

public sealed class DispatcherDependentBasedDispatcherProvider : IDispatcherProvider
{
    private DispatcherDependentBasedDispatcherProvider()
    {
    }

    public static DispatcherDependentBasedDispatcherProvider Instance { get; } = new();

    public IDispatcher GetDispatcher(object target)
    {
        if (target is IDispatcherDependent dispatcherDependent)
        {
            return dispatcherDependent.GetDispatcher();
        }

        return null;
    }
}