using SubhadraSolutions.Utils.Data.Contracts;

namespace SubhadraSolutions.Utils.Data.Common;

public sealed class GenericEntityBuilderFactory<TEntity, TBuilder> : IEntityBuilderFactory<TEntity>
    where TBuilder : IEntityBuilder<TEntity>, new()
{
    private GenericEntityBuilderFactory()
    {
    }

    public static GenericEntityBuilderFactory<TEntity, TBuilder> Instance { get; } = new();

    public IEntityBuilder<TEntity> CreateBuilder()
    {
        return new TBuilder();
    }
}