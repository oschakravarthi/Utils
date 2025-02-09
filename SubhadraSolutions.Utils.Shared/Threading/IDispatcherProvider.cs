namespace SubhadraSolutions.Utils.Threading;

public interface IDispatcherProvider
{
    IDispatcher GetDispatcher(object target);
}