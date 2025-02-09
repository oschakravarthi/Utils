using SubhadraSolutions.Utils.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public abstract class AbstractDataRecord : IDataRecord
{
    private Dictionary<string, int> ordinalsDictionary;

    protected List<FieldNameAndType> Fields { get; } = [];

    public int FieldCount => Fields.Count;

    public abstract object this[int i] { get; }

    public object this[string name]
    {
        get
        {
            var index = GetOrdinal(name);
            return this[index];
        }
    }

    public virtual bool GetBoolean(int i)
    {
        return GetValueCore<bool>(i);
    }

    public virtual byte GetByte(int i)
    {
        return GetValueCore<byte>(i);
    }

    public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length)
    {
        return 0;
    }

    public virtual char GetChar(int i)
    {
        return GetValueCore<char>(i);
    }

    public virtual long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length)
    {
        return 0;
    }

    public virtual IDataReader GetData(int i)
    {
        return null;
    }

    public virtual string GetDataTypeName(int i)
    {
        var t = GetFieldType(i);
        if (t == null)
        {
            return null;
        }

        return t.ToString();
    }

    public virtual DateTime GetDateTime(int i)
    {
        return GetValueCore<DateTime>(i);
    }

    public virtual decimal GetDecimal(int i)
    {
        return GetValueCore<decimal>(i);
    }

    public virtual double GetDouble(int i)
    {
        return GetValueCore<double>(i);
    }

    public virtual Type GetFieldType(int i)
    {
        return Fields[i].FieldType;
    }

    public virtual float GetFloat(int i)
    {
        return GetValueCore<float>(i);
    }

    public virtual Guid GetGuid(int i)
    {
        return GetValueCore<Guid>(i);
    }

    public virtual short GetInt16(int i)
    {
        return GetValueCore<short>(i);
    }

    public virtual int GetInt32(int i)
    {
        return GetValueCore<int>(i);
    }

    public virtual long GetInt64(int i)
    {
        return GetValueCore<long>(i);
    }

    public string GetName(int i)
    {
        return Fields[i].FieldName;
    }

    public int GetOrdinal(string name)
    {
        if (ordinalsDictionary == null)
        {
            ordinalsDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < FieldCount; i++) ordinalsDictionary.Add(Fields[i].FieldName, i);
        }

        return ordinalsDictionary[name];
    }

    public virtual string GetString(int i)
    {
        return (string)this[i];
    }

    public object GetValue(int i)
    {
        return this[i];
    }

    public virtual int GetValues(object[] values)
    {
        for (var i = 0; i < FieldCount; i++) values[i] = this[i];

        return 0;
    }

    public virtual bool IsDBNull(int i)
    {
        if (GetFieldType(i).IsValueType)
        {
            return false;
        }

        return this[i] == null || this[i] == DBNull.Value;
    }

    protected virtual T GetValueCore<T>(int i)
    {
        return (T)this[i];
    }
}