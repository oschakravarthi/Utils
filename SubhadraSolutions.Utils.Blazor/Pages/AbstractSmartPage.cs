using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.Blazor.Components;
using SubhadraSolutions.Utils.Blazor.Helpers;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Pages;

public abstract class AbstractSmartPage : AbstractSmartComponentBase
{
    private string title;

    public string Title
    {
        get => title;
        set
        {
            title = value;
            if (Mainlayout != null)
            {
                Mainlayout.Title = value;
            }
        }
    }

    [CascadingParameter] private IMainLayout Mainlayout { get; set; }

    protected override void OnInitialized()
    {
        Mainlayout.Title = title;
        base.OnInitialized();
        PopulateParametersFromUri();
    }

    protected override Task OnInitializedAsync()
    {
        Mainlayout.Title = title;
        PopulateParametersFromUri();
        return base.OnInitializedAsync();
    }

    protected virtual void PopulateParametersFromUri()
    {
        var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
        BlazorHelper.SetParametersFromUri(uri, this);
    }

    protected virtual void PopulatePropertiesFromUri(object target)
    {
        var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
        BlazorHelper.SetPropertiesFromUri(uri, target);
    }
}