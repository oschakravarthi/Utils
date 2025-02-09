namespace SubhadraSolutions.Utils.Pivoting;

public interface IValidator<T>
{
    void Validate(IEnumerable<IDimensionT<T>> dimensions, IEnumerable<IMeasureT<T>> measures);
    List<IDimensionT<T>> ValidateDimensions(IEnumerable<IDimensionT<T>> dimensions);
    List<IMeasureT<T>> ValidateMeasures(IEnumerable<IMeasureT<T>> measures);
}