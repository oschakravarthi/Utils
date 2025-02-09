using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Annotations;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public interface ISmartChartAndGridCombo
{
    bool SimpleGridView { get; }
    bool AllowMinMax { get; }
    bool AllowSwitchingMode { get; }
    AttributesLookup AttributesLookup { get; }
    bool ChartMode { get; set; }
    ExtendedApexChartType ChartType { get; set; }
    public ComparisonMode ComparisonMode { get; set; }
    string Documentation { get; }
    ExplorerInfo ExplorerInfo { get; }
    string Height { get; }
    bool InternItems { get; }
    bool KMNumberFormat { get; set; }
    int MaxItemsInChart { get; }
    NavigationInfo NavigationInfo { get; }
    int PageSize { get; }
    bool Readonly { get; }
    bool ShowDataLabels { get; set; }
    bool ShowSequence { get; }
    bool Stacked { get; set; }
    string SubTitle { get; }
    string Title { get; }

    SortingOrder? XAxisSortingOrder { get; }
}