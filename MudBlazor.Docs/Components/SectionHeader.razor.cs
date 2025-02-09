// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Utilities;
using System;

namespace MudBlazor.Docs.Components;

public partial class SectionHeader : ComponentBase
{
    public ElementReference SectionReference;
    [Parameter] public string Class { get; set; }
    [Parameter] public RenderFragment Description { get; set; }
    [Parameter] public bool HideTitle { get; set; }
    public DocsSectionLink SectionInfo { get; set; }
    [Parameter] public RenderFragment SubTitle { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment TitleContent { get; set; }

    [Parameter] public RenderFragment HeaderActions { get; set; }

    protected string Classname =>
        new CssBuilder("docs-section-header mud-card-header")
            .AddClass(Class)
            .Build();

    [CascadingParameter] private DocsPage DocsPage { get; set; }

    [CascadingParameter] private DocsPageSection Section { get; set; }
    [CascadingParameter] private SectionSubGroups SubGroup { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender && DocsPage != null && !string.IsNullOrWhiteSpace(Title))
        {
            DocsPage.AddSection(SectionInfo, Section);
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (DocsPage == null || string.IsNullOrWhiteSpace(Title))
        {
            return;
        }

        var parentTitle = DocsPage.GetParentTitle(Section) ?? string.Empty;
        if (!string.IsNullOrEmpty(parentTitle))
        {
            parentTitle += '-';
        }

        var id = (parentTitle + Title).Replace(" ", "-").ToLowerInvariant();

        SectionInfo = new DocsSectionLink { Id = id, Title = Title };
    }

    private string GetSectionId()
    {
        return SectionInfo?.Id ?? Guid.NewGuid().ToString();
    }

    private Typo GetTitleTypo()
    {
        if (Section.Level >= 1)
        {
            return Typo.h6;
        }

        return Typo.h5;
    }
}