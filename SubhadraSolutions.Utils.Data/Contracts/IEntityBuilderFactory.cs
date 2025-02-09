namespace SubhadraSolutions.Utils.Data.Contracts;

public interface IEntityBuilderFactory<out T>
{
    IEntityBuilder<T> CreateBuilder();
}