using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public interface ISmartChartAndGridComboCoreStrategy
{
    object GetChildRowContent();

    object GetTemplateColumnContent();

    IQueryable BuildQueryable();

    IQueryable BuildQueryableToCompare();

    bool CanFetchData();

    object GetChartItemClickCallback(Type entityType);

    object GetData();

    object GetDataToCompare();

    IReadOnlyCollection<string> GetDimensions();

    IReadOnlyCollection<string> GetDimensionsForComparison();

    IReadOnlyCollection<MeasureAndSeriesType> GetMeasures();
}