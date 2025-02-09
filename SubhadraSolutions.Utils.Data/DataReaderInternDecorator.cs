using System;
using System.Collections.Generic;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public class DataReaderInternDecorator : AbstractDataReaderDecorator
{
    private List<int> ordinals;
    private object[] values;

    public override object this[int i]
    {
        get
        {
            if (!ordinals.Contains(i))
            {
                return Actual[i];
            }

            return values[i];
        }
    }

    public override IDataReader GetData(int i)
    {
        var data = Actual.GetData(i);
        if (data == null)
        {
            return null;
        }

        var decorator = new DataReaderInternDecorator
        {
            Actual = data
        };
        return decorator;
    }

    public override string GetString(int i)
    {
        if (!ordinals.Contains(i))
        {
            return Actual.GetString(i);
        }

        return (string)values[i];
    }

    public override object GetValue(int i)
    {
        return this[i];
    }

    public override int GetValues(object[] values)
    {
        var min = Math.Min(values.Length, Actual.FieldCount);
        for (var i = 0; i < min; i++) values[i] = this[i];
        return min;
    }

    public override bool Read()
    {
        if (!Actual.Read())
        {
            return false;
        }

        for (var i = 0; i < ordinals.Count; i++)
        {
            var index = ordinals[i];
            if (Actual.IsDBNull(index))
            {
                values[index] = DBNull.Value;
                continue;
            }

            var newValue = Actual.GetString(index);
            if (values[index] == DBNull.Value)
            {
                values[index] = newValue;
                continue;
            }

            var oldValue = (string)values[index];
            if (oldValue != newValue)
            {
                values[index] = newValue;
            }
        }

        return true;
    }

    protected override void OnActualChanged()
    {
        ordinals = [];
        values = new object[Actual.FieldCount];
        for (var i = 0; i < Actual.FieldCount; i++)
            if (Actual.GetFieldType(i) == typeof(string))
            {
                ordinals.Add(i);
            }
    }
}