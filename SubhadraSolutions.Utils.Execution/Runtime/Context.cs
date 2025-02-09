using Newtonsoft.Json;
using SubhadraSolutions.Utils.Collections.Specialized;
using SubhadraSolutions.Utils.Execution.Contracts;
using System;
using System.Collections.Generic;
using System.Data;

namespace SubhadraSolutions.Utils.Execution.Runtime;

public class Context : IContext
{
    public Context()
    {
        Arguments = [];
        Objects = [];
        Collections = [];
        DataTables = [];
        DataReaders = [];
        Agents = [];
        AgentStatuses = [];
        AgentArguments = [];
        Arguments.Add("EnvironmentUserName", Environment.UserName);
    }

    public ObservableDictionary<string, IDataReader> DataReaders { get; }

    public ObservableDictionary<string, IDictionary<string, object>> AgentArguments { get; }

    public ObservableDictionary<string, IAgent> Agents { get; }

    public ObservableDictionary<string, AgentStatus> AgentStatuses { get; }

    public ObservableDictionary<string, object> Arguments { get; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Action<HashSet<ParameterDefinition>, IDictionary<string, object>> ArgumentsNeededCallback { get; set; }

    public ObservableDictionary<string, object> Collections { get; }
    public ObservableDictionary<string, DataTable> DataTables { get; }
    public ObservableDictionary<string, object> Objects { get; }
}