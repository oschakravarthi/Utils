using SubhadraSolutions.Utils.Execution.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Agents;

public class DataTableColumnToHashSetPropertyAgent : AbstractAgent
{
    private static readonly MethodInfo BuildListTemplateMethod =
        typeof(DataTableColumnToHashSetPropertyAgent).GetMethod(nameof(BuildHashSet),
            BindingFlags.Static | BindingFlags.NonPublic);

    public string ColumnName { get; set; }
    public string DataTableName { get; set; }
    public string PropertyName { get; set; }

    public override HashSet<ParameterDefinition> GetParameterDefinitions()
    {
        return [];
    }

    protected override Task<AgentStatus> ExecuteCore(IContext context)
    {
        return Task.Factory.StartNew(() =>
        {
            context.DataTables.TryGetValue(DataTableName, out var dataTable);
            if (dataTable == null)
            {
                return AgentStatus.ExecutionCancelled;
            }

            var columnName = string.IsNullOrEmpty(ColumnName) ? PropertyName : ColumnName;
            var propertyName = string.IsNullOrEmpty(PropertyName) ? ColumnName : PropertyName;
            var columnIndex = dataTable.Columns.IndexOf(columnName);
            if (columnIndex < 0)
            {
                return AgentStatus.ExecutionCancelled;
            }

            var method = BuildListTemplateMethod.MakeGenericMethod(dataTable.Columns[columnIndex].DataType);
            var set = method.Invoke(null, [dataTable, columnIndex]);
            context.Arguments[propertyName] = set;
            return AgentStatus.ExecutedSuccessfully;
        });
    }

    private static HashSet<T> BuildHashSet<T>(DataTable dataTable, int columnIndex)
    {
        var set = new HashSet<T>();
        for (var i = 0; i < dataTable.Rows.Count; i++)
        {
            var obj = (T)dataTable.Rows[i][columnIndex];
            if (obj != null && !string.IsNullOrEmpty(Convert.ToString(obj)))
            {
                set.Add(obj);
            }
        }

        return set;
    }
}