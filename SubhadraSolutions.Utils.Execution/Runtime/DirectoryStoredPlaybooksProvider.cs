using SubhadraSolutions.Utils.Execution.Contracts;
using SubhadraSolutions.Utils.Execution.Playbooks;
using SubhadraSolutions.Utils.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.Execution.Runtime;

public class DirectoryStoredPlaybooksProvider : IPlaybooksProvider
{
    public DirectoryStoredPlaybooksProvider()
    {
        var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        PlaybooksDirectory = directory + Path.DirectorySeparatorChar + "Playbooks";
    }

    public string PlaybooksDirectory { get; set; }

    public void ExportPlaybooks(IEnumerable<Playbook> playbooks)
    {
        ExportPlaybooks(playbooks, PlaybooksDirectory);
    }

    public List<Playbook> GetPlaybooks()
    {
        var playbooks = new List<Playbook>();
        if (!Directory.Exists(PlaybooksDirectory))
        {
            return playbooks;
        }

        var files = Directory.GetFiles(PlaybooksDirectory, "*.json");
        foreach (var file in files)
        {
            var playbook = JsonSerializationHelper.DeserializeFromFile<Playbook>(file);
            playbooks.Add(playbook);
        }

        return playbooks;
    }

    public static void ExportPlaybooks(IEnumerable<Playbook> playbooks, string directory)
    {
        if (!playbooks.Any())
        {
            return;
        }

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        foreach (var playbook in playbooks)
        {
            var file = $"{directory}{Path.DirectorySeparatorChar}{playbook.AgentTree.Name}.json";
            JsonSerializationHelper.SerializeToFile(playbook, file);
        }
    }
}