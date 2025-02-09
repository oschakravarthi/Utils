using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Annotations;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public abstract class AbstractSmartChartAndGridCombo : AbstractMaximizableSmartComponent, ISmartChartAndGridCombo
{
    protected AbstractSmartChartAndGridCombo()
    {
        Outlined = true;
        ChartMode = true;
        ShowSequence = true;
        AllowSwitchingMode = true;
        KMNumberFormat = true;
        ComparisonMode = ComparisonMode.OnDemand;
        AllowMinMax = true;
        InternItems = false;
        PageSize = 20;
        ShowDataLabels = false;
    }

    [Parameter] public bool SimpleGridView { get; set; } = false;
    [Parameter] public List<string> Dimensions { get; set; }

    [Parameter] public List<string> DimensionsForComparison { get; set; }

    [Parameter] public bool AllowMinMax { get; set; }

    [Parameter] public bool AllowSwitchingMode { get; set; }

    [Parameter] public AttributesLookup AttributesLookup { get; set; }

    [Parameter] public bool ChartMode { get; set; }

    [Parameter] public ExtendedApexChartType ChartType { get; set; }

    [Parameter] public ComparisonMode ComparisonMode { get; set; }

    [Parameter] public string Documentation { get; set; }

    [Parameter] public ExplorerInfo ExplorerInfo { get; set; }

    [Parameter] public string Height { get; set; }

    [Parameter] public bool InternItems { get; set; }

    [Parameter] public bool KMNumberFormat { get; set; }

    [Parameter] public int MaxItemsInChart { get; set; }

    [Parameter] public NavigationInfo NavigationInfo { get; set; }

    [Parameter] public int PageSize { get; set; }

    [Parameter] public bool Readonly { get; set; }

    [Parameter] public bool ShowDataLabels { get; set; }

    [Parameter] public bool ShowSequence { get; set; }

    [Parameter] public bool Stacked { get; set; }

    [Parameter] public string SubTitle { get; set; }

    [Parameter] public string Title { get; set; }

    [Parameter] public SortingOrder? XAxisSortingOrder { get; set; }
}