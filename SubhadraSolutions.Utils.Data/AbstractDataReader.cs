using System;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public abstract class AbstractDataReader : AbstractDataRecord, IDataReader
{
    public long RecordsReadSoFar { get; private set; }
    public virtual int Depth { get; protected set; }

    public virtual bool IsClosed => false;

    public virtual int RecordsAffected { get; protected set; }

    public virtual void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public override IDataReader GetData(int i)
    {
        return this;
    }

    public virtual DataTable GetSchemaTable()
    {
        return null;
    }

    public virtual bool NextResult()
    {
        return true;
    }

    public bool Read()
    {
        var couldRead = ReadCore();
        if (couldRead)
        {
            RecordsReadSoFar++;
        }

        return couldRead;
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected abstract bool ReadCore();
}