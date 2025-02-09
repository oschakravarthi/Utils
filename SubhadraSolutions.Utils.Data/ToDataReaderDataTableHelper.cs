using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data;

internal static class ToDataReaderDataTableHelper
{
    internal static readonly HashSet<Type> DataTableSupportedDataTypes;

    static ToDataReaderDataTableHelper()
    {
        DataTableSupportedDataTypes =
        [
            typeof(bool),
            typeof(byte),
            typeof(char),
            typeof(DateTime),
            typeof(decimal),
            typeof(double),
            typeof(Guid),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(float),
            typeof(string),
            typeof(TimeSpan),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(byte[])
        ];
    }
}