using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.Blazor.Helpers;
using System;

namespace SubhadraSolutions.Utils.Blazor.Components;

public class NavigationInfo(EventCallback navigationAction)
{
    public NavigationInfo(Type navigationPageType, string target = "_blank", string tooltip = "Navigate to the details page") : this()
    {
        NavigationUrl = BlazorHelper.GetPageUrl(navigationPageType);
        Target = target;
        Tooltip = tooltip;
    }

    public NavigationInfo(string navigationUrl, string target = "_blank", string tooltip = "Navigate to the details page") : this()
    {
        NavigationUrl = navigationUrl;
        Target = target;
        Tooltip = tooltip;
    }

    private NavigationInfo() : this(EventCallback.Empty)
    {
    }

    public EventCallback NavigationAction { get; } = navigationAction;
    public string NavigationUrl { get; }
    public string Target { get; }
    public string Tooltip { get; }
}