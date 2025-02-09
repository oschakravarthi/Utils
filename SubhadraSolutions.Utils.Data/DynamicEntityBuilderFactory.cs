using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Contracts;

namespace SubhadraSolutions.Utils.Data;

public sealed class DynamicEntityBuilderFactory<T> : IEntityBuilderFactory<T>
{
    private DynamicEntityBuilderFactory()
    {
    }

    [DynamicallyInvoked]
    public static DynamicEntityBuilderFactory<T> Instance { get; } = new();

    public IEntityBuilder<T> CreateBuilder()
    {
        return new DynamicEntityBuilder<T>();
    }
}