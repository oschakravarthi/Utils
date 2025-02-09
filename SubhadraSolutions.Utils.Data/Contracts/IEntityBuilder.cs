using System;
using System.Data;

namespace SubhadraSolutions.Utils.Data.Contracts;

public interface IEntityBuilder<out T> : IDisposable
{
    T BuildEntityFromCurrent();

    void Initialize(IDataReader dataReader);
}