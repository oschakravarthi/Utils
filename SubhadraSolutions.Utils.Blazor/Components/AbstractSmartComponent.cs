using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components;

public abstract class AbstractSmartComponent : AbstractSmartComponentBase
{
    /// <summary>
    ///     The higher the number, the heavier the drop-shadow. 0 for no shadow.
    /// </summary>

    [Parameter]
    public int Elevation { set; get; }

    /// <summary>
    ///     If true, card will be outlined.
    /// </summary>
    [Parameter]
    public bool Outlined { get; set; }

    /// <summary>
    ///     If true, border-radius is set to 0.
    /// </summary>
    [Parameter]
    public bool Square { get; set; }

    protected virtual string GetClass()
    {
        return Class;
    }

    protected virtual string GetStyle()
    {
        return Style;
    }

    protected async Task<DialogResult> ShowTaskDialog<T>(string title, IDictionary<string, object> payload, Func<ValueTask<T>> taskFunc, string icon = null)
    {
        var parameters = new DialogParameters<TaskDialog<T>>
        {
            { x => x.Title, title },
            { x => x.Payload, payload },
            { x => x.TaskFunc, taskFunc },
        };
        if (icon != null)
        {
            parameters.Add(x => x.Icon, icon);
        }
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            FullScreen = false,
            NoHeader = true,
            FullWidth = true,

            BackdropClick = false
        };

        var dialog = await DialogService.ShowAsync<TaskDialog<T>>(title, parameters, options).ConfigureAwait(false);
        var result = await dialog.Result.ConfigureAwait(false);
        return result;
    }

    //protected virtual string GetStyle()
    //{
    //    if (!string.IsNullOrWhiteSpace(this.Style))
    //    {
    //        return this.Style;
    //    }
    //    var color = ColorPalettes.GetRandomLightColor();
    //    var style = string.Format("background-color: {0};", color);
    //    return style;
    //}
}