using SubhadraSolutions.Utils.Execution.Playbooks;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Execution.Contracts;

public interface IPlaybooksProvider
{
    void ExportPlaybooks(IEnumerable<Playbook> playbooks);

    List<Playbook> GetPlaybooks();
}