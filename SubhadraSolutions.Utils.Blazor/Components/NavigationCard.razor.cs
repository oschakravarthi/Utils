using Microsoft.AspNetCore.Components;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class NavigationCard : AbstractSmartComponent
{
    [Parameter] public NavigationInfo NavigationInfo { get; set; }

    [Parameter] public string Subtitle { get; set; }

    [Parameter] public string Title { get; set; }

    [Parameter] public string Tooltip { get; set; } = "Navigate to the details page";
}