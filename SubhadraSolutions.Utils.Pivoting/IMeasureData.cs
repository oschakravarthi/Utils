﻿namespace SubhadraSolutions.Utils.Pivoting;

public interface IMeasureData<T>
{
    IEnumerable<T> Measure { get; set; }
    IEnumerable<T> ColumnGroup { get; set; }
    IEnumerable<T> RowGroup { get; set; }
}