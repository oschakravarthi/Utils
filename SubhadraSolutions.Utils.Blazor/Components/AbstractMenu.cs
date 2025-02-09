using Microsoft.AspNetCore.Components;

namespace SubhadraSolutions.Utils.Blazor.Components;

public abstract class AbstractMenu : ComponentBase
{
    [Parameter] public INavMenu NavMenu { get; set; }
}