// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;
using System.Collections.Generic;

namespace MudBlazor.Docs.Components;

public partial class DocsPageSection : ComponentBase
{
    private bool _renderImmediately = true;
    [Parameter] public RenderFragment ChildContent { get; set; }
    public int Level { get; private set; }
    [CascadingParameter] public DocsPageSection ParentSection { get; protected set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> UserAttributes { get; set; } = [];

    [CascadingParameter] private DocsPage DocsPage { get; set; }
    [Inject] private IRenderQueueService QueueService { get; set; }

    [Parameter]
    public string PrintViewStyle { get; set; }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var count = DocsPage.IncrementSectionCount();
        _renderImmediately = count < QueueService.Capacity;

        var level = (ParentSection?.Level ?? -1) + 1;
        Level = level;
    }
}