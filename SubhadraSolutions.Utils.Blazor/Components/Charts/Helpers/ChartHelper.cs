using ApexCharts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts.Helpers;

public static class ChartHelper
{
    private static readonly MethodInfo buildOptionsTemplateMethod =
        typeof(ChartHelper).GetMethod(nameof(BuildOptions), BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly MethodInfo ComponentBaseOnInitializedMethod =
        typeof(ComponentBase).GetMethod("OnInitialized", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo FrameTypeField =
        typeof(RenderTreeFrame).GetField("FrameTypeField", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly string KMFormatJavaSciptFunction = "toKMNumberFormat";

    public static void AddChart(RenderTreeBuilder builder, Sequencer sequencer, object data,
        object previousDataToCompare, ChartConfig chartConfig)
    {
        if (sequencer == null)
        {
            sequencer = new Sequencer();
        }

        var entityType = data.GetType().GetEnumerableItemType();
        var attributesLookup = chartConfig.AttributesLookup;
        var dimensionPropertyNames = chartConfig.DimensionPropertyNames;
        var measureAndSeriesTypes = chartConfig.MeasureAndSeriesTypes;
        if (attributesLookup == null)
        {
            attributesLookup = new AttributesLookup(entityType);
        }

        if (dimensionPropertyNames == null || dimensionPropertyNames.Count == 0 || measureAndSeriesTypes == null ||
            measureAndSeriesTypes.Count == 0)
        {
            var dimensionsAndMeasures = DimensionsAndMeasuresHelper.GetDimensionsAndMeasuresProperties(entityType,
                knownDimensions: dimensionPropertyNames, attributesLookup: attributesLookup);

            if (dimensionPropertyNames == null || dimensionPropertyNames.Count == 0)
            {
                dimensionPropertyNames = dimensionsAndMeasures.Key.Select(x => x.Name).ToList();
            }

            if (measureAndSeriesTypes == null || measureAndSeriesTypes.Count == 0)
            {
                measureAndSeriesTypes = dimensionsAndMeasures.Value
                    .Select(x => new MeasureAndSeriesType(x.Name, ExtendedApexSeriesType.Auto)).ToList();
            }
        }

        var dimensions = dimensionPropertyNames.Select(entityType.GetProperty).ToList();
        var xAxisType = XAxisType.Category;

        string formatString = null;
        SortAttribute sortDirectionAttribute = null;
        object dimensionsToObjectFunc = null;
        var xAxisDataType = typeof(string);
        if (dimensions.Count == 1)
        {
            var dimension = dimensions[0];
            var dimensionPropertyType = dimension.PropertyType;
            xAxisDataType = dimensionPropertyType;
            if (chartConfig.XAxisSortingOrder != null)
            {
                sortDirectionAttribute = new SortAttribute(chartConfig.XAxisSortingOrder.Value);
            }
            else
            {
                sortDirectionAttribute = attributesLookup.GetCustomAttribute<SortAttribute>(dimension, true);
                if (sortDirectionAttribute == null && dimensionPropertyType.IsNumericType())
                {
                    sortDirectionAttribute = new SortAttribute(SortingOrder.Ascending);
                }
            }

            xAxisType = GetXAxisType(dimensionPropertyType);
            formatString = dimension.GetFormatString(attributesLookup);
            formatString = GetJSFormatString(formatString);
            if (formatString == null)
            {
                if (dimension.PropertyType.IsNumericType())
                {
                    dimensionsToObjectFunc = BuildFuncForValueToObject(dimension);
                }
            }
        }

        if (dimensionsToObjectFunc == null)
        {
            dimensionsToObjectFunc = BuildFuncForDimensionsToObject(entityType, dimensions);
        }

        //var width = IsCircularChart(chartConfig.ChartType) ? "84%" : null;
        builder.OpenComponent(sequencer.Next, typeof(ExtendedApexChart<>).MakeGenericType(entityType));
        //if (width != null)
        //{
        //    builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Width), width);
        //}

        if (!string.IsNullOrWhiteSpace(chartConfig.Height))
        {
            builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Height), chartConfig.Height);
            builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Width), (string)null);
        }
        else
        {
            var width = chartConfig.ChartType.IsCircularChart() ? "80%" : "100%";
            builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Width), width);
            builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Height), (string)null);
        }

        builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Stacked), chartConfig.Stacked);
        builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.XAxisType), xAxisType);
        builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Data), data);

        if (chartConfig.SelectedDataCallback != null)
        {
            builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.OnDataPointSelection),
                chartConfig.SelectedDataCallback);
        }

        var options = (IExtendedApexChartOptions)buildOptionsTemplateMethod.MakeGenericMethod(entityType)
            .Invoke(null, [xAxisType, formatString, xAxisDataType, chartConfig]);
        builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.Options), options);

        if (chartConfig.ChartAttributes != null)
        {
            foreach (var kvp in chartConfig.ChartAttributes)
            {
                builder.AddAttribute(sequencer.Next, kvp.Key, kvp.Value);
            }
        }

        builder.AddAttribute(sequencer.Next, nameof(ExtendedApexChart<object>.ChildContent),
            (RenderFragment)(b => AddChartChildContent(b, null, data, entityType, dimensionsToObjectFunc,
                measureAndSeriesTypes, xAxisType, sortDirectionAttribute, attributesLookup, options,
                previousDataToCompare, chartConfig)));

        builder.CloseComponent();
    }

    public static IEnumerable<ExtendedApexChartType> GetAllowedChartTypes(Type entityType,
        AttributesLookup attributesLookup = null)
    {
        return GetAllowedChartTypes(entityType, null, null, attributesLookup);
    }

    public static IEnumerable<ExtendedApexChartType> GetAllowedChartTypes(Type entityType,
        IReadOnlyCollection<string> dimensions, IReadOnlyCollection<MeasureAndSeriesType> measures,
        AttributesLookup attributesLookup = null)
    {
        GetFinalDimensionsAndMeasures(entityType, dimensions, measures, out var finalDimensions, out var finalMeasures,
            attributesLookup);
        foreach (var chartType in Enum.GetValues<ExtendedApexChartType>())
        {
            if (finalMeasures.Count > 1)
            {
                if (chartType.IsSingleSeriesChartType())
                {
                    continue;
                }
            }

            //TODO: remove Bar chart if DateTime x axis.
            if (IsChartTypeSupported(chartType))
            {
                yield return chartType;
            }
        }
    }

    public static string GetChartJson(object data, object previousDataToCompare, ChartConfig chartConfig)
    {
        using var builder = new RenderTreeBuilder();
        AddChart(builder, null, data, previousDataToCompare, chartConfig);
        var stack = new Stack<object>();
        var obj = ProcessRenderTree(builder, stack);
        var chart = (IExtendedApexChart)obj;
        var json = chart.GetJson();
        return json;
    }

    public static void GetFinalDimensionsAndMeasures(Type elementType, IReadOnlyCollection<string> dimensions,
        IReadOnlyCollection<MeasureAndSeriesType> measures, out IReadOnlyCollection<string> finalDimensions,
        out IReadOnlyCollection<MeasureAndSeriesType> finalMeasures, AttributesLookup attributesLookup = null)
    {
        finalDimensions = dimensions;
        finalMeasures = measures;
        if (dimensions == null || dimensions.Count == 0 || measures == null || measures.Count == 0)
        {
            var pairs = DimensionsAndMeasuresHelper.GetDimensionsAndMeasuresProperties(elementType, null, dimensions,
                attributesLookup);

            if (dimensions == null || dimensions.Count == 0)
            {
                var temp = pairs.Key.ToList();
                temp.Sort(OrderComparer<PropertyInfo>.Instance);
                finalDimensions = temp.Select(p => p.Name).ToList();
            }
            else
            {
                finalDimensions = dimensions.ToList();
            }

            if (measures == null || measures.Count == 0)
            {
                finalMeasures = pairs.Value.Select(x => new MeasureAndSeriesType(x.Name, ExtendedApexSeriesType.Auto))
                    .ToList();
            }
            else
            {
                finalMeasures = measures.ToList();
            }
        }
    }

    public static IEnumerable<ExtendedApexChartType> GetSupportedChartTypes()
    {
        foreach (var val in Enum.GetValues(typeof(ExtendedApexChartType)))
        {
            var chartType = (ExtendedApexChartType)val;
            if (IsChartTypeSupported(chartType))
            {
                yield return chartType;
            }
        }
    }

    public static XAxisType GetXAxisType(Type propertyType)
    {
        if (propertyType.IsDateOrTimeType())
        {
            return XAxisType.Datetime;
        }

        if (propertyType.IsNumericType())
        {
            return XAxisType.Numeric;
        }

        return XAxisType.Category;
    }

    public static bool IsChartTypeSupported(ExtendedApexChartType chartType)
    {
        var apexChartType = MapExtendedChartTypeToChartType(chartType);
        var seriesType = GetSeriesClassTypeFromChartType(apexChartType);
        return seriesType == typeof(ApexPointSeries<>);
    }

    public static bool IsCircularChart(this ExtendedApexChartType chartType)
    {
        return chartType is ExtendedApexChartType.Pie or ExtendedApexChartType.Donut or ExtendedApexChartType.Radar or ExtendedApexChartType.RadialBar;
    }

    public static bool IsMixableChart(this ExtendedApexChartType chartType)
    {
        var ct = MapExtendedChartTypeToChartType(chartType);
        return ct.IsMixableChart();
    }

    public static bool IsMixableChart(this ChartType chartType)
    {
        return chartType is ChartType.Line or ChartType.Scatter or ChartType.Area or ChartType.Bubble or ChartType.Candlestick or ChartType.BoxPlot or ChartType.Bar;
    }

    public static bool IsSingleSeriesChartType(this ExtendedApexChartType chartType)
    {
        return chartType is ExtendedApexChartType.Donut or ExtendedApexChartType.Pie or ExtendedApexChartType.PolarArea or ExtendedApexChartType.RadialBar; // || chartType == ExtendedApexChartType.Comparison;
    }

    public static bool IsSingleSeriesType(SeriesType seriesType)
    {
        return seriesType is SeriesType.Donut or SeriesType.Pie or SeriesType.PolarArea or SeriesType.RadialBar; // || chartType == ExtendedApexChartType.Comparison;
    }

    public static bool IsStackable(Type entityType, IReadOnlyCollection<string> dimensions,
        IReadOnlyCollection<MeasureAndSeriesType> measures, AttributesLookup attributesLookup = null)
    {
        GetFinalDimensionsAndMeasures(entityType, dimensions, measures, out var finalDimensions, out var finalMeasures,
            attributesLookup);
        return finalMeasures.Count > 1;
    }

    public static bool SupportsDataLabels(ExtendedApexChartType chartType)
    {
        return chartType is ExtendedApexChartType.Bar or ExtendedApexChartType.Column or ExtendedApexChartType.Bubble or ExtendedApexChartType.Candlestick or ExtendedApexChartType.Heatmap or ExtendedApexChartType.RadialBar or ExtendedApexChartType.Line;
    }

    private static void AddChartChildContent(RenderTreeBuilder builder, Sequencer sequencer, object data,
        Type entityType, object dimensionsToObjectFunc, IEnumerable<MeasureAndSeriesType> measureAndSeriesTypes,
        XAxisType xAxisType, SortAttribute sortDirectionAttribute, AttributesLookup attributesLookup,
        IExtendedApexChartOptions options, object previousDataToCompare, ChartConfig chartConfig)
    {
        var chartType = chartConfig.ChartType;
        if (sequencer == null)
        {
            sequencer = new Sequencer();
        }

        var dimensionsAxisFunc = BuildFuncForAxis(entityType, "X");

        foreach (var measureAndSeriesType in measureAndSeriesTypes)
        {
            var measureProperty = entityType.GetProperty(measureAndSeriesType.PropertyName);
            AddMeasure(builder, sequencer, data, entityType, measureProperty, dimensionsToObjectFunc, chartType,
                measureAndSeriesType.SeriesType, xAxisType, dimensionsAxisFunc, sortDirectionAttribute,
                attributesLookup, options, chartConfig, null);
            if (previousDataToCompare != null)
            {
                var titleTemplate = "[Past] {0}";
                AddMeasure(builder, sequencer, previousDataToCompare, entityType, measureProperty,
                    dimensionsToObjectFunc, ExtendedApexChartType.Area, ExtendedApexSeriesType.Area, xAxisType,
                    dimensionsAxisFunc, sortDirectionAttribute, attributesLookup, options, chartConfig, titleTemplate);
            }
        }
    }

    private static void AddMeasure(RenderTreeBuilder builder, Sequencer sequencer, object data, Type entityType,
        PropertyInfo measureProperty, object dimensionsToObjectFunc, ExtendedApexChartType chartType,
        ExtendedApexSeriesType seriesType, XAxisType xAxisType, Delegate dimensionsAxisFunc,
        SortAttribute sortDirectionAttribute, AttributesLookup attributesLookup, IExtendedApexChartOptions options,
        ChartConfig chartConfig, string titleTemplate)
    {
        if (options.Yaxis == null)
        {
            options.Yaxis = [];
        }

        if (options.Yaxis.Count == 0)
        {
            options.Yaxis.Add(new YAxis());
        }

        var yAxis = options.Yaxis[0];
        if (measureProperty.PropertyType.IsNumericType())
        {
            if (chartConfig.KMNumberFormat)
            {
                if (yAxis.Labels == null)
                {
                    yAxis.Labels = new YAxisLabels();
                }

                yAxis.Labels.Formatter = KMFormatJavaSciptFunction;
            }

            if (measureProperty.PropertyType.IsIntegerType())
            {
                yAxis.DecimalsInFloat = 0;
            }
        }

        var title = measureProperty.GetMemberDisplayName(true, attributesLookup);
        if (titleTemplate != null)
        {
            title = string.Format(titleTemplate, title);
        }

        var apexChartType = MapExtendedChartTypeToChartType(chartType);
        var seriesClassTemplateType = GetSeriesClassTypeFromChartType(apexChartType);
        var seriesClassType = seriesClassTemplateType.MakeGenericType(entityType);
        builder.OpenComponent(sequencer.Next, seriesClassType);
        builder.AddAttribute(sequencer.Next, nameof(ApexBaseSeries<object>.Items), data);
        builder.AddAttribute(sequencer.Next, nameof(ApexBaseSeries<object>.Name), title);
        builder.AddAttribute(sequencer.Next, nameof(ApexBaseSeries<object>.XValue), dimensionsToObjectFunc);

        if (seriesClassType.GetProperty(nameof(ApexPointSeries<object>.YValue)) != null)
        {
            var yValueFunc = BuildFuncForMeasure(entityType, measureProperty);
            builder.AddAttribute(sequencer.Next, nameof(ApexPointSeries<object>.YValue), yValueFunc);
        }

        if (seriesClassType.GetProperty(nameof(ApexPointSeries<object>.SeriesType)) != null)
        {
            SeriesType apexSeriesType;
            if (seriesType == ExtendedApexSeriesType.Auto)
            {
                apexSeriesType = GetSeriesTypeFromChartType(apexChartType);
            }
            else
            {
                apexSeriesType = MapExtendedSeriesTypeToSeriesType(seriesType);
            }

            builder.AddAttribute(sequencer.Next, nameof(ApexPointSeries<object>.SeriesType), apexSeriesType);
        }

        if (sortDirectionAttribute != null)
        {
            builder.AddAttribute(sequencer.Next,
                sortDirectionAttribute.SortOrder == SortingOrder.Descending
                    ? nameof(ApexPointSeries<object>.OrderByDescending)
                    : nameof(ApexPointSeries<object>.OrderBy), dimensionsAxisFunc);
        }
        else
        {
            if (xAxisType is XAxisType.Datetime or XAxisType.Numeric)
            {
                builder.AddAttribute(sequencer.Next, nameof(ApexPointSeries<object>.OrderBy), dimensionsAxisFunc);
                builder.AddAttribute(sequencer.Next, nameof(ApexPointSeries<object>.OrderByDescending),
                    (Func<DataPoint<object>, object>)null);
            }
            else
            {
                builder.AddAttribute(sequencer.Next, nameof(ApexPointSeries<object>.OrderByDescending),
                    BuildFuncForAxis(entityType, "Y"));
                builder.AddAttribute(sequencer.Next, nameof(ApexPointSeries<object>.OrderBy),
                    (Func<DataPoint<object>, object>)null);
            }
        }

        builder.AddAttribute(sequencer.Next, nameof(ApexPointSeries<object>.ShowDataLabels),
            (chartConfig.ShowDataLabels && SupportsDataLabels(chartType)) || HasImplicitDataLabels(chartType));

        builder.CloseComponent();
    }

    private static Delegate BuildFuncForAxis(Type entityType, string axis)
    {
        var datapointType = typeof(DataPoint<>).MakeGenericType(entityType);
        var axisProperty = datapointType.GetProperty(axis);
        var parameterExpression = Expression.Parameter(datapointType, "e");
        Expression expression = Expression.Property(parameterExpression, axisProperty.GetGetMethod());
        expression = Expression.Convert(expression, typeof(object));
        var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(datapointType, typeof(object)), expression,
            parameterExpression);
        return lambda.Compile();
    }

    private static object BuildFuncForDimensionsToObject(Type entityType, IEnumerable<PropertyInfo> dimensions)
    {
        return PropertiesToStringValuesHelper.BuildFuncForToStringAsObject(entityType,
            dimensions.Select(d => d.Name).ToList());
    }

    private static object BuildFuncForMeasure(Type entityType, PropertyInfo measure)
    {
        var parameterExpression = Expression.Parameter(entityType, "e");
        Expression expression = Expression.Property(parameterExpression, measure.GetGetMethod());
        var decimalType = typeof(decimal?);

        if (measure.PropertyType != decimalType)
        {
            expression = Expression.Convert(expression, decimalType);
        }

        var exp = Expression.Lambda(typeof(Func<,>).MakeGenericType(entityType, decimalType), expression,
            parameterExpression);
        return exp.Compile();
    }

    private static Delegate BuildFuncForValueToObject(PropertyInfo property)
    {
        var parameterExpression = Expression.Parameter(property.DeclaringType, "e");
        Expression expression = Expression.Property(parameterExpression, property.GetGetMethod());
        expression = Expression.Convert(expression, typeof(object));
        var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(property.DeclaringType, typeof(object)),
            expression, parameterExpression);
        return lambda.Compile();
    }

    private static ApexChartOptions<T> BuildOptions<T>(XAxisType xAxisType, string format, Type xAxisDataType,
        ChartConfig chartConfig) where T : class
    {
        var chartType = chartConfig.ChartType;
        var options = new ExtendedApexChartOptions<T>
        {
            Stroke = new Stroke
            {
                Width = 2
            }
        };
        if (options.Xaxis == null)
        {
            options.Xaxis = new XAxis();
        }

        if (chartConfig.MaxItemsInChart > 0)
        {
            options.Xaxis.Range = chartConfig.MaxItemsInChart;
        }

        options.Xaxis.Type = xAxisType;

        if (format != null)
        {
            options.Xaxis.Labels = new XAxisLabels
            {
                Format = format
            };
        }

        if (xAxisDataType.IsIntegerType())
        {
            options.Xaxis.DecimalsInFloat = 0;
        }

        if (chartType is ExtendedApexChartType.Bar or ExtendedApexChartType.Column)
        {
            options.PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    Horizontal = chartType == ExtendedApexChartType.Bar
                }
            };
        }

        options.DataLabels = new DataLabels
        {
            Enabled = chartConfig.ShowDataLabels
        };
        if (chartConfig.ShowDataLabels && chartType == ExtendedApexChartType.Column)
        {
            options.DataLabels.Style = new DataLabelsStyle
            {
                Colors = [chartConfig.IsDarkMode ? "#ffffff" : "#000000"],
                FontSize = "12px"
            };
        }

        if (chartConfig.ShowDataLabels)
        {
            if (chartConfig.KMNumberFormat)
            {
                if (chartType != ExtendedApexChartType.Pie && chartType != ExtendedApexChartType.Donut &&
                    chartType != ExtendedApexChartType.PolarArea)
                {
                    options.DataLabels.Formatter = KMFormatJavaSciptFunction;
                }
            }

            if (chartType is ExtendedApexChartType.Bar or ExtendedApexChartType.Column)
            {
                options.PlotOptions.Bar.DataLabels = new PlotOptionsBarDataLabels
                {
                    Position = BarDataLabelPosition.Top,
                    HideOverflowingLabels = false,
                    Orientation = chartType == ExtendedApexChartType.Bar ? Orientation.Horizontal : Orientation.Vertical
                };
                if (chartType == ExtendedApexChartType.Column)
                {
                    options.DataLabels.OffsetY = 5;
                }
                //else
                //{
                //    options.DataLabels.OffsetX = 20;
                //}
            }
        }

        return options;
    }

    private static string GetJSFormatString(string formatString)
    {
        if (string.IsNullOrWhiteSpace(formatString))
        {
            return null;
        }

        formatString = formatString.TrimStart('{');
        formatString = formatString.TrimEnd('}');
        var index = formatString.IndexOf(':');
        if (index > -1)
        {
            formatString = formatString[(index + 1)..];
        }

        return formatString;
    }

    private static Type GetSeriesClassTypeFromChartType(ChartType chartType)
    {
        return chartType switch
        {
            ChartType.Area => typeof(ApexPointSeries<>),
            ChartType.Bar => typeof(ApexPointSeries<>),
            ChartType.BoxPlot => typeof(ApexBoxPlotSeries<>),
            ChartType.Bubble => typeof(ApexBubbleSeries<>),
            ChartType.Candlestick => typeof(ApexCandleSeries<>),
            ChartType.Donut => typeof(ApexPointSeries<>),
            ChartType.Heatmap => typeof(ApexPointSeries<>),
            //ChartType.Histogram => typeof(ApexPointSeries<>),
            ChartType.Line => typeof(ApexPointSeries<>),
            ChartType.Pie => typeof(ApexPointSeries<>),
            ChartType.PolarArea => typeof(ApexPointSeries<>),
            ChartType.Radar => typeof(ApexPointSeries<>),
            ChartType.RadialBar => typeof(ApexPointSeries<>),
            ChartType.RangeBar => typeof(ApexRangeSeries<>),
            ChartType.Scatter => typeof(ApexPointSeries<>),
            ChartType.Treemap => typeof(ApexPointSeries<>),
            _ => typeof(ApexPointSeries<>)
        };
    }

    private static SeriesType GetSeriesTypeFromChartType(ChartType chartType)
    {
        return chartType switch
        {
            ChartType.Area => SeriesType.Area,
            ChartType.Bar => SeriesType.Bar,
            ChartType.BoxPlot => SeriesType.Bar,
            ChartType.Bubble => SeriesType.Bar,
            ChartType.Candlestick => SeriesType.Bar,
            ChartType.Donut => SeriesType.Donut,
            ChartType.Heatmap => SeriesType.Heatmap,
            //ChartType.Histogram => SeriesType.Histogram,
            ChartType.Line => SeriesType.Line,
            ChartType.Pie => SeriesType.Pie,
            ChartType.PolarArea => SeriesType.PolarArea,
            ChartType.Radar => SeriesType.Radar,
            ChartType.RadialBar => SeriesType.RadialBar,
            ChartType.RangeBar => SeriesType.Bar,
            ChartType.Scatter => SeriesType.Scatter,
            ChartType.Treemap => SeriesType.Treemap,
            _ => SeriesType.Bar
        };
    }

    private static bool HasImplicitDataLabels(ExtendedApexChartType chartType)
    {
        return chartType is ExtendedApexChartType.Pie or ExtendedApexChartType.Donut or ExtendedApexChartType.PolarArea or ExtendedApexChartType.Treemap;
    }

    private static ChartType MapExtendedChartTypeToChartType(ExtendedApexChartType chartType)
    {
        return chartType switch
        {
            ExtendedApexChartType.Area => ChartType.Area,
            ExtendedApexChartType.Bar => ChartType.Bar,
            ExtendedApexChartType.Bubble => ChartType.Bubble,
            ExtendedApexChartType.Candlestick => ChartType.Candlestick,
            ExtendedApexChartType.Column => ChartType.Bar,
            ExtendedApexChartType.Donut => ChartType.Donut,
            ExtendedApexChartType.Heatmap => ChartType.Heatmap,
            //ExtendedApexChartType.Histogram => ChartType.Histogram,
            ExtendedApexChartType.Line => ChartType.Line,
            ExtendedApexChartType.Pie => ChartType.Pie,
            ExtendedApexChartType.PolarArea => ChartType.PolarArea,
            ExtendedApexChartType.Radar => ChartType.Radar,
            ExtendedApexChartType.RadialBar => ChartType.RadialBar,
            ExtendedApexChartType.RangeBar => ChartType.RangeBar,
            ExtendedApexChartType.Scatter => ChartType.Scatter,
            ExtendedApexChartType.Treemap => ChartType.Treemap,
            ExtendedApexChartType.BoxPlot => ChartType.BoxPlot,
            _ => ChartType.Bar
        };
    }

    private static SeriesType MapExtendedSeriesTypeToSeriesType(ExtendedApexSeriesType seriesType)
    {
        return seriesType switch
        {
            ExtendedApexSeriesType.Area => SeriesType.Area,
            ExtendedApexSeriesType.Bar => SeriesType.Bar,
            ExtendedApexSeriesType.Donut => SeriesType.Donut,
            ExtendedApexSeriesType.Heatmap => SeriesType.Heatmap,
            //ExtendedApexSeriesType.Histogram => SeriesType.Histogram,
            ExtendedApexSeriesType.Line => SeriesType.Line,
            ExtendedApexSeriesType.Pie => SeriesType.Pie,
            ExtendedApexSeriesType.PolarArea => SeriesType.PolarArea,
            ExtendedApexSeriesType.Radar => SeriesType.Radar,
            ExtendedApexSeriesType.RadialBar => SeriesType.RadialBar,
            ExtendedApexSeriesType.Scatter => SeriesType.Scatter,
            ExtendedApexSeriesType.Treemap => SeriesType.Treemap,
            _ => SeriesType.Bar
        };
    }

    [SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "<Pending>")]
    private static object ProcessRenderTree(RenderTreeBuilder builder, Stack<object> stack)
    {
        var frames = builder.GetFrames();
        for (var i = 0; i < frames.Count; i++)
        {
            var frame = frames.Array[i];
            var frameType = (RenderTreeFrameType)FrameTypeField.GetValue(frame);
            switch (frameType)
            {
                case RenderTreeFrameType.Component:
                    {
                        var component = Activator.CreateInstance(frame.ComponentType);
                        var properties =
                            frame.ComponentType.GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                              BindingFlags.NonPublic);
                        foreach (var property in properties)
                        {
                            if (!property.IsDefined<CascadingParameterAttribute>(true, true))
                            {
                                continue;
                            }

                            foreach (var item in stack)
                            {
                                if (property.PropertyType.IsInstanceOfType(item))
                                {
                                    property.SetValue(component, item);
                                    break;
                                }
                            }
                        }

                        stack.Push(component);
                        break;
                    }
                case RenderTreeFrameType.Attribute:
                    {
                        var component = stack.Peek();
                        if (frame.AttributeValue is RenderFragment fragment)
                        {
                            using var childBuilder = new RenderTreeBuilder();
                            fragment(childBuilder);
                            ProcessRenderTree(childBuilder, stack);
                        }
                        else
                        {
                            component.GetType().GetProperty(frame.AttributeName).SetValue(component, frame.AttributeValue);
                        }

                        break;
                    }
            }
        }

        if (stack.Count > 0)
        {
            var obj = stack.Pop();
            if (obj is ComponentBase)
            {
                ComponentBaseOnInitializedMethod.Invoke(obj, Array.Empty<object>());
                return obj;
            }
        }

        return null;
    }
}