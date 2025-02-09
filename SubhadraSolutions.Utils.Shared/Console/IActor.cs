using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Console;

public interface IActor
{
    Task<bool> RunAsync();
}