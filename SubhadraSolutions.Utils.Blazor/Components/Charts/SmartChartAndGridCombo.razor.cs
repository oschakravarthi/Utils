using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public partial class SmartChartAndGridCombo : AbstractSmartChartAndGridCombo
{
    private readonly SmartChartAndGridComboStrategy strategy;

    public SmartChartAndGridCombo()
    {
        strategy = new SmartChartAndGridComboStrategy(this);
    }

    [Parameter] public object Data { get; set; }

    [Parameter] public object DataToCompare { get; set; }

    [Parameter] public List<MeasureAndSeriesType> Measures { get; set; }

    [Parameter] public IQueryable Queryable { get; set; }
    [Parameter] public object ChildRowContent { get; set; }
    [Parameter] public object TemplateColumnContent { get; set; }
    [Parameter] public IQueryable QueryableToCompare { get; set; }
}