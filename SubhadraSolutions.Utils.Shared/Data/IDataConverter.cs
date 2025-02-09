namespace SubhadraSolutions.Utils.Data;

public interface IDataConverter<out T>
{
    T Convert(object input);
}