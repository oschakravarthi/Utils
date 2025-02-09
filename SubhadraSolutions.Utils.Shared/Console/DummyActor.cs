using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Console;

public class DummyActor : IActor
{
    public Task<bool> RunAsync()
    {
        return Task.FromResult(true);
    }
}