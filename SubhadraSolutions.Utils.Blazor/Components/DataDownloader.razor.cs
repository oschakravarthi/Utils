using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.ApiClient.Helpers;
using SubhadraSolutions.Utils.Blazor.Helpers;
using System;
using System.Diagnostics;
using System.Linq;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class DataDownloader : AbstractSmartComponent
{
    private bool isBusy;

    [Parameter] public IQueryable Queryable { get; set; }

    [Parameter] public string SheetName { get; set; } = "Data";

    [Parameter] public string Title { get; set; }

    private async void ExportToExcel()
    {
        try
        {
            isBusy = true;
            var data = await Queryable.GetList().ConfigureAwait(false);
            ExportHelper.ExportToExcel(data, SheetName, JSRuntime);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToDetailedString());
            throw;
        }
        finally
        {
            isBusy = false;
            StateHasChanged();
        }
    }
}