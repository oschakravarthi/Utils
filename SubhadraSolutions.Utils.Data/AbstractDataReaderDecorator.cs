using Newtonsoft.Json;
using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public abstract class AbstractDataReaderDecorator : AbstractDisposable, IDataReaderDecorator
{
    private IDataReader actual;

    [JsonIgnore]
    public IDataReader Actual
    {
        get => actual;
        set
        {
            actual = value;
            OnActualChanged();
        }
    }

    [JsonIgnore] public virtual int Depth => actual.Depth;

    [JsonIgnore] public virtual int FieldCount => actual.FieldCount;

    [JsonIgnore] public virtual bool IsClosed => actual.IsClosed;

    [JsonIgnore] public virtual int RecordsAffected => actual.RecordsAffected;

    [JsonIgnore] public virtual object this[string name] => actual[name];

    [JsonIgnore] public virtual object this[int i] => actual[i];

    public virtual void Close()
    {
        actual.Close();
    }

    public virtual bool GetBoolean(int i)
    {
        return actual.GetBoolean(i);
    }

    public virtual byte GetByte(int i)
    {
        return actual.GetByte(i);
    }

    public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length)
    {
        return actual.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
    }

    public virtual char GetChar(int i)
    {
        return actual.GetChar(i);
    }

    public virtual long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length)
    {
        return actual.GetChars(i, fieldOffset, buffer, bufferOffset, length);
    }

    public virtual IDataReader GetData(int i)
    {
        return actual.GetData(i);
    }

    public virtual string GetDataTypeName(int i)
    {
        return actual.GetDataTypeName(i);
    }

    public virtual DateTime GetDateTime(int i)
    {
        return actual.GetDateTime(i);
    }

    public virtual decimal GetDecimal(int i)
    {
        return actual.GetDecimal(i);
    }

    public virtual double GetDouble(int i)
    {
        return actual.GetDouble(i);
    }

    public virtual Type GetFieldType(int i)
    {
        return actual.GetFieldType(i);
    }

    public virtual float GetFloat(int i)
    {
        return actual.GetFloat(i);
    }

    public virtual Guid GetGuid(int i)
    {
        return actual.GetGuid(i);
    }

    public virtual short GetInt16(int i)
    {
        return actual.GetInt16(i);
    }

    public virtual int GetInt32(int i)
    {
        return actual.GetInt32(i);
    }

    public virtual long GetInt64(int i)
    {
        return actual.GetInt64(i);
    }

    public virtual string GetName(int i)
    {
        return actual.GetName(i);
    }

    public virtual int GetOrdinal(string name)
    {
        return actual.GetOrdinal(name);
    }

    public virtual DataTable GetSchemaTable()
    {
        return actual.GetSchemaTable();
    }

    public virtual string GetString(int i)
    {
        return actual.GetString(i);
    }

    public virtual object GetValue(int i)
    {
        return actual.GetValue(i);
    }

    public virtual int GetValues(object[] values)
    {
        return actual.GetValues(values);
    }

    public virtual bool IsDBNull(int i)
    {
        return actual.IsDBNull(i);
    }

    public virtual bool NextResult()
    {
        return actual.NextResult();
    }

    public virtual bool Read()
    {
        return actual.Read();
    }

    protected override void Dispose(bool disposing)
    {
        actual?.Dispose();
    }

    protected virtual void OnActualChanged()
    {
    }
}