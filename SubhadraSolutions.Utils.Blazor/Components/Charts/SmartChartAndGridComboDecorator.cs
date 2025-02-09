using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Annotations;
using System.Collections;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

internal class SmartChartAndGridComboDecorator(ISmartChartAndGridCombo actual) : ISmartChartAndGridCombo
{
    public IList PreviousRecords { get; set; }

    public ChartSettings PreviousSettings { get; set; }

    public IList Records { get; set; }

    public bool AllowMinMax => actual.AllowMinMax;

    public bool AllowSwitchingMode => actual.AllowSwitchingMode;

    public AttributesLookup AttributesLookup => actual.AttributesLookup;

    public bool ChartMode
    {
        get => actual.ChartMode;
        set => actual.ChartMode = value;
    }

    public ExtendedApexChartType ChartType
    {
        get => actual.ChartType;
        set => actual.ChartType = value;
    }

    public ComparisonMode ComparisonMode
    {
        get => actual.ComparisonMode;
        set => actual.ComparisonMode = value;
    }

    public string Documentation => actual.Documentation;

    public ExplorerInfo ExplorerInfo => actual.ExplorerInfo;

    public string Height => actual.Height;

    public bool InternItems => actual.InternItems;

    public bool KMNumberFormat
    {
        get => actual.KMNumberFormat;
        set => actual.KMNumberFormat = value;
    }

    public bool SimpleGridView => actual.SimpleGridView;
    public int MaxItemsInChart => actual.MaxItemsInChart;

    public NavigationInfo NavigationInfo => actual.NavigationInfo;

    public int PageSize => actual.PageSize;

    public bool Readonly => actual.Readonly;

    public bool ShowDataLabels
    {
        get => actual.ShowDataLabels;
        set => actual.ShowDataLabels = value;
    }

    public bool ShowSequence => actual.ShowSequence;

    public bool Stacked
    {
        get => actual.Stacked;
        set => actual.Stacked = value;
    }

    public string SubTitle => actual.SubTitle;

    public string Title => actual.Title;

    public SortingOrder? XAxisSortingOrder => actual.XAxisSortingOrder;

    public class ChartSettings
    {
        public ExtendedApexChartType ChartType { get; set; }
        public bool Stacked { get; set; }
    }
}