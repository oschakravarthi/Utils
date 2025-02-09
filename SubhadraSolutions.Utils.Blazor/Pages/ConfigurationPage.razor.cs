using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.ApiClient.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Pages;

public partial class ConfigurationPage : AbstractSmartPage
{
    private List<KeyValuePair<string, string>> configuration;

    [Inject] private IConfigurationProvider ConfigurationProvider { get; set; }

    protected override void Nullify()
    {
        configuration = null;
    }

    protected override async Task ResetAsync()
    {
        configuration = await ConfigurationProvider.GetConfigurationAsync().ConfigureAwait(false);
    }
}