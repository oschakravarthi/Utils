// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MudBlazor.Docs.Components;

public partial class DocsPage : ComponentBase
{
    private readonly Queue<DocsSectionLink> _bufferedSections = new();
    private readonly Dictionary<DocsPageSection, MudPageContentSection> _sectionMapper = [];
    private string _anchor = null;
    private bool _contentDrawerOpen = true;
    private MudPageContentNavigation _contentNavigation;

    private int _sectionCount;

    //private NavigationSection? _section = null;
    private Stopwatch _stopwatch = Stopwatch.StartNew();

    [Parameter] public RenderFragment HeaderContent { get; set; }
    [Parameter] public RenderFragment PageContent { get; set; }
    [Parameter] public bool EnableContentNavigation { get; set; } = false;
    [Parameter] public MaxWidth MaxWidth { get; set; } = MaxWidth.Medium;
    [Parameter] public bool PrintView { get; set; } = false;
    
    public int SectionCount
    {
        get
        {
            lock (this)
            {
                return _sectionCount;
            }
        }
    }

    [Inject] private NavigationManager NavigationManager { get; set; }

    //[Inject] private IDocsNavigationService DocsService { get; set; }
    [Inject] private IRenderQueueService RenderQueue { get; set; }

    public event Action<Stopwatch> Rendered;

    public string GetParentTitle(DocsPageSection section)
    {
        if (section == null || section.ParentSection == null)
        {
            return string.Empty;
        }

        if (!_sectionMapper.TryGetValue(section.ParentSection, out var item))
        {
            return string.Empty;
        }

        return item.Title;
    }

    public int IncrementSectionCount()
    {
        lock (this)
        {
            return _sectionCount++;
        }
    }

    internal async void AddSection(DocsSectionLink sectionLinkInfo, DocsPageSection section)
    {
        _bufferedSections.Enqueue(sectionLinkInfo);

        if (_contentNavigation != null)
        {
            while (_bufferedSections.Count > 0)
            {
                var item = _bufferedSections.Dequeue();

                if (_contentNavigation.Sections.FirstOrDefault(x => x.Id == sectionLinkInfo.Id) == default)
                {
                    MudPageContentSection parentInfo = null;
                    if (section.ParentSection != null)
                    {
                        _sectionMapper.TryGetValue(section.ParentSection, out parentInfo);
                    }

                    var info =
                        new MudPageContentSection(sectionLinkInfo.Title, sectionLinkInfo.Id, section.Level,
                            parentInfo);
                    _sectionMapper.Add(section, info);
                    _contentNavigation.AddSection(info, false);
                }
            }

            //_contentNavigation.Update();

            if (_anchor != null)
            {
                if (sectionLinkInfo.Id == _anchor)
                {
                    await _contentNavigation.ScrollToSection(new Uri(NavigationManager.Uri)).ConfigureAwait(false);
                    _anchor = null;
                }
            }
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (_stopwatch.IsRunning)
        {
            _stopwatch.Stop();
            Rendered?.Invoke(_stopwatch);
        }

        if (firstRender)
        {
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        RenderQueue.Clear();
        var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        if (relativePath.Contains('#'))
        {
            var splits = relativePath.Split(["#"], StringSplitOptions.RemoveEmptyEntries);
            _anchor = splits[splits.Length - 1];
        }
    }

    protected override void OnParametersSet()
    {
        _stopwatch = Stopwatch.StartNew();
        _sectionCount = 0;
    }
}