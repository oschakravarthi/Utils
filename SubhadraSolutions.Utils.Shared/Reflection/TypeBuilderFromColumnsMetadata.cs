using SubhadraSolutions.Utils.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

public static class TypeBuilderFromColumnsMetadata
{
    // public static Type BuildType(List<ColumnMetaRecord> columns)
    // {
    //    var list = new List<Tuple<string, Type>>();
    //    foreach (var meta in columns)
    //    {
    //        var tuple = new Tuple<string, Type>(meta.Name, CoreReflectionHelper.GetType(meta.DataType));
    //        list.Add(tuple);
    //    }
    //    var type = AnonymousTypeBuilder.BuildAnonymousType(list);
    //    return type;
    // }

    public static Type BuildType(ModuleBuilder mb, string className, List<ColumnMetaRecord> columns)
    {
        var list = new List<Tuple<string, Type>>();

        for (var i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var columnType = CoreReflectionHelper.GetType(column.DataType);

            if (columnType != null)
            {
                var tuple = new Tuple<string, Type>(column.Name, columnType);
                list.Add(tuple);
            }
            else
            {
                columns.RemoveAt(i);
                i--;
            }
        }

        return DynamicTypeBuilder.BuildType(mb, className, list);
    }
}