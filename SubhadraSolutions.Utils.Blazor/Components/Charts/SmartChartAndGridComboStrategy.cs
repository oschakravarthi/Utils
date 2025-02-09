using SubhadraSolutions.Utils.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

internal class SmartChartAndGridComboStrategy(SmartChartAndGridCombo chart) : ISmartChartAndGridComboCoreStrategy
{
    public IQueryable BuildQueryable()
    {
        return chart.Queryable;
    }

    public object GetChildRowContent()
    {
        return chart.ChildRowContent;
    }

    public object GetTemplateColumnContent()
    {
        return chart.TemplateColumnContent;
    }

    public IQueryable BuildQueryableToCompare()
    {
        //return this.chart.QueryableToCompare;
        if (chart.QueryableToCompare != null)
        {
            return chart.QueryableToCompare;
        }

        var queryable = BuildQueryable();
        if (queryable == null)
        {
            return null;
        }

        var visitor = new PreviousDateTimeRangeExpressionVisitor(queryable);
        return visitor.RewrittenQueryable;
    }

    public bool CanFetchData()
    {
        return true;
    }

    public object GetChartItemClickCallback(Type entityType)
    {
        return null;
    }

    public object GetData()
    {
        return chart.Data;
    }

    public object GetDataToCompare()
    {
        return chart.DataToCompare;
    }

    public IReadOnlyCollection<string> GetDimensions()
    {
        return chart.Dimensions;
    }

    public IReadOnlyCollection<string> GetDimensionsForComparison()
    {
        return chart.DimensionsForComparison;
    }

    public IReadOnlyCollection<MeasureAndSeriesType> GetMeasures()
    {
        return chart.Measures;
    }
}