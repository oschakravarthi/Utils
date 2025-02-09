using System;
using System.Data;
using System.Reflection;

namespace SubhadraSolutions.Utils.Data;

public class DataReaderToObjectsDataConverter : IDataSource<object>
{
    public IDataSource<IDataReader> DataSource { get; set; }

    public Type DTOType { get; set; }

    public object GetData()
    {
        var dataReader = DataSource.GetData();
        if (DTOType == null)
        {
            DTOType = dataReader.CreateTypeForDTO();
        }

        var list = GetList(dataReader);
        return list;
    }

    private object GetList(IDataReader dataReader)
    {
        var method = typeof(DynamicDataReaderToEntitiesHelper<>).MakeGenericType(DTOType)
            .GetMethod("MapToEntitiesList", BindingFlags.Public | BindingFlags.Static);
        return method.Invoke(null, [dataReader]);
    }
}