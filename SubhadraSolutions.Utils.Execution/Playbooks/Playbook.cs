using SubhadraSolutions.Utils.Execution.Contracts;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Execution.Playbooks;

public class Playbook
{
    public List<IAgent> Agents { get; set; } = [];
    public AgentTreeNode AgentTree { get; set; }

    //public static Playbook Merge(Playbook a, Playbook b)
    //{
    //    var playbook=new Playbook();
    //    playbook.Agents.AddRange(a.Agents);
    //    playbook.Agents.AddRange(b.Agents);
    //}
}