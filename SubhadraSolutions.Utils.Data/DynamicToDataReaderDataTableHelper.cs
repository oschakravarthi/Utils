using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Common;
using SubhadraSolutions.Utils.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Data;

public static class DynamicToDataReaderDataTableHelper<T>
    where T : class
{
    private static readonly List<FieldNameAndType> _columns = [];

    private static readonly PopulateItemArrayDelegate<T> _populateItemArrayDelegate;

    private static readonly DataTable _schema;

    static DynamicToDataReaderDataTableHelper()
    {
        var objectType = typeof(T);
        var dynamicMethod = new DynamicMethod("populateItemArray", typeof(void), [objectType, typeof(object[])],
            typeof(DynamicToDataReaderDataTableHelper<T>));
        var ilGen = dynamicMethod.GetILGenerator();
        var properties = ReflectionHelper.GetPublicProperties<T>(true);
        var propertyIndex = 0;
        foreach (var property in properties)
        {
            if (property.CanRead && (!objectType.IsInterface || !doesColumnExist(property.Name)))
            {
                ilGen.Emit(OpCodes.Ldarg_1);
                ilGen.Emit(OpCodes.Ldc_I4_S, propertyIndex);
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Callvirt, property.GetMethod);
                if (property.PropertyType.IsEnum)
                {
                    ilGen.Emit(OpCodes.Box, property.PropertyType);
                    ilGen.Emit(OpCodes.Call, SharedReflectionInfo.ObjectToStringMethod);
                }
                else
                {
                    if (property.PropertyType.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Box, property.PropertyType);
                    }
                }

                ilGen.Emit(OpCodes.Callvirt, SharedReflectionInfo.ArrayIndexerSetMethod);
                propertyIndex++;
                var mapping = new FieldNameAndType
                (
                    property.Name,
                    ToDataReaderDataTableHelper.DataTableSupportedDataTypes.Contains(property.PropertyType)
                        ? property.PropertyType
                        : typeof(object)
                );
                _columns.Add(mapping);
            }
        }

        ilGen.Emit(OpCodes.Ret);
        _populateItemArrayDelegate =
            (PopulateItemArrayDelegate<T>)dynamicMethod.CreateDelegate(typeof(PopulateItemArrayDelegate<T>));
        _schema = ToDataTableSchema();
    }

    public static IDataReader ExportAsDataReader(IEnumerable<T> enumerable)
    {
        var strategy =
            new EnumeratorBasedDataReaderStrategy<T>(enumerable.GetEnumerator(), _columns, _populateItemArrayDelegate);
        strategy.Initialize();
        var reader = new GenericDataReader(strategy);
        return reader;
    }

    public static IEnumerable<DataRow> ExportAsDataRows(IEnumerable<T> enumerable)
    {
        if (enumerable != null)
        {
            var itemArray = new object[_columns.Count];
            foreach (var item in enumerable)
            {
                _populateItemArrayDelegate(item, itemArray);
                var row = _schema.NewRow();
                row.ItemArray = itemArray;
                yield return row;
            }
        }
    }

    [DynamicallyInvoked]
    public static DataTable ExportAsDataTable(IEnumerable<T> enumerable)
    {
        var table = ToDataTableSchema();
        var itemArray = new object[table.Columns.Count];
        if (enumerable != null)
        {
            foreach (var item in enumerable)
            {
                _populateItemArrayDelegate(item, itemArray);
                table.Rows.Add(itemArray);
            }
        }

        return table;
    }

    public static List<FieldNameAndType> GetColumnTypes()
    {
        var list = new List<FieldNameAndType>();
        foreach (var kvp in _columns)
        {
            list.Add(kvp);
        }

        return list;
    }

    [DynamicallyInvoked]
    public static DataTable ToDataTableSchema()
    {
        var table = new DataTable();
        foreach (var kvp in _columns)
        {
            table.Columns.Add(kvp.FieldName, kvp.FieldType);
        }

        return table;
    }

    private static bool doesColumnExist(string columnName)
    {
        foreach (var kvp in _columns)
        {
            if (kvp.FieldName == columnName)
            {
                return true;
            }
        }

        return false;
    }
}