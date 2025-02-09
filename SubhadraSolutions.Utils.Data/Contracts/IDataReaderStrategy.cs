using SubhadraSolutions.Utils.Data.Common;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data.Contracts;

public interface IDataReaderStrategy : IDisposable
{
    ICollection<FieldNameAndType> Fields { get; }

    long TotalRecordCountIfKnown { get; }

    object this[int i] { get; }

    Type GetFieldType(int i);

    bool Read();
}