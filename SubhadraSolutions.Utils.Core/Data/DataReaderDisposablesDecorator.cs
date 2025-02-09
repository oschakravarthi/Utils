using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Data;

namespace SubhadraSolutions.Utils.Data
{
    public class DataReaderDisposablesDecorator : AbstractDisposable, IDataReader
    {
        private readonly IDataReader inner;
        private readonly IDisposable[] disposables;

        public DataReaderDisposablesDecorator(IDataReader inner, params IDisposable[] disposables)
        {
            this.inner = inner;
            this.disposables = disposables;
        }

        public object this[int i] => inner[i];

        public object this[string name] => inner[name];

        public int Depth => inner.Depth;

        public bool IsClosed => inner.IsClosed;

        public int RecordsAffected => inner.RecordsAffected;

        public int FieldCount => inner.FieldCount;

        public void Close()
        {
            inner.Close();
        }

        public bool GetBoolean(int i)
        {
            return inner.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return inner.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return inner.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return inner.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return inner.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return inner.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return inner.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return inner.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return inner.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return inner.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return inner.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return inner.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return inner.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return inner.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return inner.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return inner.GetInt64(i);
        }

        public string GetName(int i)
        {
            return inner.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return inner.GetOrdinal(name);
        }

        public DataTable GetSchemaTable()
        {
            return inner.GetSchemaTable();
        }

        public string GetString(int i)
        {
            return inner.GetString(i);
        }

        public object GetValue(int i)
        {
            return inner.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return inner.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return inner.IsDBNull(i);
        }

        public bool NextResult()
        {
            return inner.NextResult();
        }

        public bool Read()
        {
            return inner.Read();
        }

        protected override void Dispose(bool disposing)
        {
            this.inner.Dispose();
            GeneralHelper.DisposeIfDisposables(disposables);
        }
    }
}