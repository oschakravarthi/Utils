using SubhadraSolutions.Utils.Pivoting.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace SubhadraSolutions.Utils.Pivoting.BlazorDemo.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddLeaderPivot();
        await builder.Build().RunAsync();
    }
}
