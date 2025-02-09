using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Data.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public class DataReaderToEnumeratorAdapter<T> : AbstractDisposable, IEnumerator<T>
{
    private readonly IDataReader dataReader;
    private readonly IEntityBuilder<T> entityBuilder;

    public DataReaderToEnumeratorAdapter(IDataReader dataReader, IEntityBuilderFactory<T> entityBuilderFactory)
    {
        if (entityBuilderFactory == null)
        {
            entityBuilderFactory = DynamicEntityBuilderFactory<T>.Instance;
        }

        this.dataReader = dataReader;
        entityBuilder = entityBuilderFactory.CreateBuilder();
        entityBuilder.Initialize(dataReader);
    }

    public T Current { get; private set; }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        Current = default;
        var canMove = dataReader.Read();
        if (canMove)
        {
            Current = entityBuilder.BuildEntityFromCurrent();
        }

        return canMove;
    }

    public void Reset()
    {
        // Method intentionally left empty.
    }

    protected override void Dispose(bool disposing)
    {
        dataReader.Dispose();
    }
}