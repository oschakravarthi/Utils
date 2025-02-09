using SubhadraSolutions.Utils.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Data;

namespace SubhadraSolutions.Utils.Execution.Contracts;

public interface IContext
{
    ObservableDictionary<string, IDictionary<string, object>> AgentArguments { get; }
    ObservableDictionary<string, IAgent> Agents { get; }
    ObservableDictionary<string, AgentStatus> AgentStatuses { get; }
    ObservableDictionary<string, object> Arguments { get; }
    Action<HashSet<ParameterDefinition>, IDictionary<string, object>> ArgumentsNeededCallback { get; }
    ObservableDictionary<string, object> Collections { get; }
    ObservableDictionary<string, DataTable> DataTables { get; }
    ObservableDictionary<string, object> Objects { get; }
}