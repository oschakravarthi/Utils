// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.Docs.Components;

public partial class SectionContent
{
    [Parameter] public bool Block { get; set; } = true;
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public bool DarkenBackground { get; set; }
    [Parameter] public bool FullWidth { get; set; } = true;
    [Parameter] public string HighLight { get; set; }
    [Parameter] public bool Outlined { get; set; } = true;

    protected string Classname =>
        new CssBuilder("docs-section-content")
            .AddClass("outlined", Outlined && ChildContent != null)
            .AddClass("darken", DarkenBackground)
            .AddClass(Class)
            .Build();

    protected string InnerClassname =>
        new CssBuilder("docs-section-content-inner")
            .AddClass("relative d-flex flex-grow-1 flex-wrap justify-center align-center", !Block)
            .AddClass("d-block mx-auto", Block)
            .AddClass("mud-width-full", Block && FullWidth)
            .Build();

    [Inject] protected IJsApiService JsApiService { get; set; }

    protected string SourceClassname =>
        new CssBuilder("docs-section-source")
            .AddClass("outlined", Outlined && ChildContent != null)
            .Build();

    protected string ToolbarClassname =>
        new CssBuilder("docs-section-content-toolbar")
            .AddClass("outlined", Outlined && ChildContent != null)
            .Build();
}