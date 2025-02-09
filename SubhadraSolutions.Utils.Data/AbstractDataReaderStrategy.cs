using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Data.Common;
using SubhadraSolutions.Utils.Data.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data;

public abstract class AbstractDataReaderStrategy<T> : AbstractInitializableAndDisposable, IDataReaderStrategy
{
    private IList<FieldNameAndType> _fields;

    private object[] _itemArray;

    public ICollection<FieldNameAndType> Fields => _fields;

    public long TotalRecordCountIfKnown { get; protected set; }

    public object this[int i] => _itemArray[i];

    public Type GetFieldType(int i)
    {
        return _fields[i].FieldType;
    }

    public bool Read()
    {
        var current = ReadNextObject();
        if (current != null)
        {
            populateItemArray(current, _itemArray);
            for (var i = 0; i < _itemArray.Length; i++)
                if (_itemArray[i] == null)
                {
                    _itemArray[i] = DBNull.Value;
                }

            return true;
        }

        return false;
    }

    protected abstract IList<FieldNameAndType> GetFieldsCore();

    protected override void InitializeProtected()
    {
        _fields = GetFieldsCore();
        _itemArray = new object[_fields.Count];
    }

    protected abstract void populateItemArray(T obj, object[] itemArray);

    protected abstract T ReadNextObject();
}