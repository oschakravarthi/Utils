namespace SubhadraSolutions.Utils.EventProcessing;

public interface IEventSubscriber
{
    string[] Topics { get; }
}