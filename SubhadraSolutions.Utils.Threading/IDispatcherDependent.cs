namespace SubhadraSolutions.Utils.Threading;

public interface IDispatcherDependent
{
    IDispatcher GetDispatcher();
}