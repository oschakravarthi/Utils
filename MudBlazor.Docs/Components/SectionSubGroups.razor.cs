using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Components;

public partial class SectionSubGroups : ComponentBase
{
    [Parameter] public RenderFragment ChildContent { get; set; }
}