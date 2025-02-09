using MudBlazor;

namespace SubhadraSolutions.Utils.Blazor.Components;

public class ExtendedMudField : MudField
{
    public ExtendedMudField()
    {
        Variant = Variant.Outlined;
        Margin = Margin.Dense;
        FullWidth = true;
        InnerPadding = false;
    }
}