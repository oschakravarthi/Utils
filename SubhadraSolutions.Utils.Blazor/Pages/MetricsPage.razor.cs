using Microsoft.AspNetCore.Components;
using MudBlazor;
using SubhadraSolutions.Utils.ApiClient.Contracts;
using SubhadraSolutions.Utils.Blazor.Components;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Color = MudBlazor.Color;

namespace SubhadraSolutions.Utils.Blazor.Pages;

public partial class MetricsPage : AbstractSmartPage
{
    private List<IGrouping<string, ObjectMetrics>> metricsGroups;

    [Inject] private IMetricsProvider MetricsProvider { get; set; }

    protected override void Nullify()
    {
        metricsGroups = null;
    }

    protected override async Task ResetAsync()
    {
        var metrics = await MetricsProvider.GetMetricsAsync().ConfigureAwait(false);
        metricsGroups = metrics.GroupBy(x => x.ObjectType).OrderBy(x => x.Key).ToList();
    }

    private async Task ResetMetricsAsync()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = false,
            BackdropClick = false,
            Position = DialogPosition.Center,
        };

        var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.ContentText, "This action resets metrics cache to Zeros. Do you really want to reset?" },
            { x => x.ButtonText, "Reset" },
            { x => x.Color, Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Reset Metrics cache", parameters, options).ConfigureAwait(false);
        var result = await dialog.Result.ConfigureAwait(false);

        if (!result.Canceled)
        {
            await MetricsProvider.ResetMetricsAsync().ConfigureAwait(false);
            await ResetAsync().ConfigureAwait(false);
        }
    }
}