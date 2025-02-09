namespace SubhadraSolutions.Utils.Pooling;

public class GenericPoolItem<T> : PoolItem<T>
{
    public T Object => AdaptedObjectProtected;
}