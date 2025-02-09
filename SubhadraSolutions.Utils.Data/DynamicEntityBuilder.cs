using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Data.Contracts;
using System;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public class DynamicEntityBuilder<T> : AbstractDisposable, IEntityBuilder<T>
{
    protected IDataReader dataReader;
    protected Func<IDataRecord, T> mapper;

    public virtual T BuildEntityFromCurrent()
    {
        return mapper(dataReader);
    }

    public virtual void Initialize(IDataReader dataReader)
    {
        this.dataReader = dataReader;
        mapper = DynamicDataReaderToEntitiesHelper<T>.BuildMapper(this.dataReader);
    }

    protected override void Dispose(bool disposing)
    {
        mapper = null;
    }
}