namespace SubhadraSolutions.Utils.Logging;

public interface IItemWriter<in T>
{
    void Write(T item);
}