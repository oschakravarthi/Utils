namespace SubhadraSolutions.Utils.Data;

public interface IDataSource<out T>
{
    T GetData();
}