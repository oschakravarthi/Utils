namespace SubhadraSolutions.Utils.Pooling;

public interface IAdaptedObjectSelectionStrategy<in T>
{
    bool CanSelectThisAdaptedObject(T adaptedObject, bool isNewlyCreatedAdaptedObject, object tag);
}