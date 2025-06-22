using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components;

public abstract class AbstractMaximizableSmartComponent : AbstractSmartComponent
{
    protected bool IsFullScreen => MudDialog != null;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    protected string GetMinMaxIcon()
    {
        return IsFullScreen
            ? MudBlazor.Icons.Material.Filled.FullscreenExit
            : MudBlazor.Icons.Material.Filled.Fullscreen;
    }

    protected string GetMinMaxIconTooltop()
    {
        return IsFullScreen ? "Close full-screen" : "Open in full-screen";
    }

    protected async Task ToggleMinMaxAsync()
    {
        if (!IsFullScreen)
        {
            var parameters = new DialogParameters();
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<ParameterAttribute>() != null)
                {
                    parameters.Add(property.Name, property.GetValue(this));
                }
            }

            var options = new DialogOptions
            {
                CloseOnEscapeKey = false,
                FullScreen = true,
                NoHeader = true,
                BackdropClick = false
            };
            var thisType = GetType();
            var dialog = await DialogService.ShowAsync(thisType, null, parameters, options);
            var result = await dialog.Result.ConfigureAwait(false);
            var copyToMethod = typeof(DynamicCopyHelper<,>).MakeGenericType(thisType, thisType)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "CopyTo" && m.GetParameters().Length == 2);
            copyToMethod.Invoke(null, [result.Data, this]);
            StateHasChanged();
        }
        else
        {
            MudDialog.Close(DialogResult.Ok(this));
        }
    }
}