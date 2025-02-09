namespace SubhadraSolutions.Utils.Contracts;

public interface IInitializable
{
    bool IsInitialized { get; }

    void Initialize();
}