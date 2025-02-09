using Microsoft.AspNetCore.Components;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class MaximizableSmartComponent : AbstractMaximizableSmartComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; }
}