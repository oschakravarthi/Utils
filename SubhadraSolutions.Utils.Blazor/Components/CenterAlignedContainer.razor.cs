using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class CenterAlignedContainer : AbstractSmartComponent
{
    public CenterAlignedContainer()
    {
        Square = false;
        Elevation = 0;
        Outlined = false;
    }

    /// <summary>
    ///     Child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Paper.Behavior)]
    public RenderFragment ChildContent { get; set; }

    protected override string GetClass()
    {
        //var cls = base.GetClass();
        //if (!string.IsNullOrWhiteSpace(this.Class))
        //{
        //    cls += (" " + "gap-2 d-flex flex-wrap");
        //}
        var cls = base.GetClass();
        if (!string.IsNullOrEmpty(cls))
        {
            cls += " ";
        }

        cls += "gap-2 d-flex flex-wrap justify-center";
        return cls;
    }

    //protected override string GetStyle()
    //{
    //    var style = base.GetStyle();
    //    //style += "opacity:0;";
    //    return style;
    //}
}