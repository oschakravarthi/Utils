using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Annotations;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public class ChartConfig
{
    public const string ChartHeight = "250px";

    public AttributesLookup AttributesLookup { get; set; } = null;
    public IDictionary<string, object> ChartAttributes { get; set; } = null;
    public ExtendedApexChartType ChartType { get; set; }
    public IReadOnlyCollection<string> DimensionPropertyNames { get; set; }

    //public string Width { get; set; }
    public string Height { get; set; }

    public bool IsDarkMode { get; set; }
    public bool KMNumberFormat { get; set; } = false;
    public int MaxItemsInChart { get; set; } = 0;
    public IReadOnlyCollection<MeasureAndSeriesType> MeasureAndSeriesTypes { get; set; }
    public IReadOnlyCollection<string> MeasurePropertyNames { get; set; }
    public object SelectedDataCallback { get; set; } = null;
    public bool ShowDataLabels { get; set; }
    public bool Stacked { get; set; } = false;
    public SortingOrder? XAxisSortingOrder { get; set; }
}