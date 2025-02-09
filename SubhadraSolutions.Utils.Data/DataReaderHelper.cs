using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace SubhadraSolutions.Utils.Data;

public static class DataReaderHelper
{
    public static AnonymousListContainer BuildAnonymousListContainer(IDataReader dataReader)
    {
        var list = BuildList(dataReader, out var _, out var properties);
        return new AnonymousListContainer
        {
            List = list,
            Properties = properties
        };
    }

    public static IEnumerator BuildEnumerator(IDataReader dataReader)
    {
        return BuildEnumerator(dataReader, out var _);
    }

    public static IEnumerator BuildEnumerator(IDataReader dataReader, out Type dtoType)
    {
        return BuildEnumerator(dataReader, out dtoType, out var _);
    }

    public static IEnumerator BuildEnumerator(IDataReader dataReader, out Type dtoType,
        out List<Tuple<string, Type>> properties)
    {
        dtoType = dataReader.CreateTypeForDTO(out properties);
        var enumerator = Activator.CreateInstance(typeof(DataReaderToEnumeratorAdapter<>).MakeGenericType(dtoType),
            dataReader, null);
        return (IEnumerator)enumerator;
    }

    public static IList BuildList(IDataReader dataReader, out Type dtoType)
    {
        return BuildList(dataReader, out dtoType, out var _);
    }

    public static IList BuildList(IDataReader dataReader, out Type dtoType, out List<Tuple<string, Type>> properties)
    {
        var enumerator = BuildEnumerator(dataReader, out dtoType, out properties);
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dtoType));
        while (enumerator.MoveNext()) list.Add(enumerator.Current);
        return list;
    }
}