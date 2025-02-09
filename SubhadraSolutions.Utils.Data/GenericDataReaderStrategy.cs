using SubhadraSolutions.Utils.Data.Common;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data;

public delegate void DisposeDelegate();

public delegate void PopulateItemArrayDelegate<in T>(T obj, object[] itemArray);

public delegate T ReadNextCallback<out T>();

public class GenericDataReaderStrategy<T>(IList<FieldNameAndType> fields, ReadNextCallback<T> readNextCallback,
        PopulateItemArrayDelegate<T> populateItemArrayDelegate, DisposeDelegate disposeCallback)
    : AbstractDataReaderStrategy<T>
    where T : class
{
    protected override void Dispose(bool disposing)
    {
        if (disposeCallback != null)
        {
            disposeCallback();
        }
    }

    protected override IList<FieldNameAndType> GetFieldsCore()
    {
        return fields;
    }

    protected override void populateItemArray(T obj, object[] itemArray)
    {
        populateItemArrayDelegate(obj, itemArray);
    }

    protected override T ReadNextObject()
    {
        return readNextCallback();
    }
}