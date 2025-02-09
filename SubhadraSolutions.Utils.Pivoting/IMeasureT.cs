namespace SubhadraSolutions.Utils.Pivoting;

public interface IMeasureT<T> : IMeasure
{
    Func<IMeasureData<T>, decimal> Aggragate { get; set; }
}