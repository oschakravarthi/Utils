using SubhadraSolutions.Utils.Data.Contracts;
using System;

namespace SubhadraSolutions.Utils.Data;

public sealed class GenericDataReader : AbstractDataReader
{
    private IDataReaderStrategy _strategy;

    public GenericDataReader(IDataReaderStrategy strategy)
    {
        Strategy = strategy;
    }

    public IDataReaderStrategy Strategy
    {
        get => _strategy;

        set
        {
            _strategy = value;
            Fields.Clear();
            Fields.AddRange(_strategy.Fields);
        }
    }

    public override object this[int i] => _strategy[i];

    public override Type GetFieldType(int i)
    {
        var fieldType = _strategy.GetFieldType(i);
        return fieldType ?? base.GetFieldType(i);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _strategy.Dispose();
    }

    protected override bool ReadCore()
    {
        return _strategy.Read();
    }
}