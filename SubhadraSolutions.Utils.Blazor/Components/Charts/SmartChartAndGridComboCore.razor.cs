using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Services;
using SubhadraSolutions.Utils.ApiClient.Helpers;
using SubhadraSolutions.Utils.Blazor.Components.Charts.Helpers;
using SubhadraSolutions.Utils.Blazor.Helpers;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Linq;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static SubhadraSolutions.Utils.Blazor.Components.Charts.SmartChartAndGridComboDecorator;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public partial class SmartChartAndGridComboCore : AbstractMaximizableSmartComponent
{
    private AttributesLookup attributesLookup = null;

    private SmartChartAndGridComboDecorator chartDecorator = null;

    //private static readonly Func<decimal, string> minifyDecimalFunc = NumberUtils.MinifyDecimal;
    private bool isInDataFetchingMode;

    private bool isStackable = false;
    [Inject] protected LayoutService LayoutService { get; set; }

    [Parameter]
    public ISmartChartAndGridCombo Chart
    {
        get => chartDecorator;
        set
        {
            if (value is SmartChartAndGridComboDecorator d)
            {
                chartDecorator = d;
            }
            else
            {
                chartDecorator = new SmartChartAndGridComboDecorator(value);
            }
        }
    }

    [Parameter] public ISmartChartAndGridComboCoreStrategy Strategy { get; set; }

    [Parameter] public RenderFragment HeaderContent { get; set; }

    [Parameter] public RenderFragment HeaderActions { get; set; }

    [Parameter] public bool LoadOnStart { get; set; } = true;

    public async Task Reload()
    {
        await ReloadCore().ConfigureAwait(false);
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        isStackable = Chart.Stacked;
        attributesLookup = Chart.AttributesLookup;

        await base.OnInitializedAsync().ConfigureAwait(false);
        if (LoadOnStart)
        {
            var measures = Strategy.GetMeasures();
            if (measures?.Count > 0)
            {
                await ReloadCore().ConfigureAwait(false);
            }
        }
    }

    private async Task ReloadCore()
    {
        chartDecorator.Records = null;

        if (!Strategy.CanFetchData())
        {
            return;
        }

        var data = Strategy.GetData();
        var queryable = Strategy.BuildQueryable();
        if (data == null && queryable == null)
        {
            return;
        }

        if (data == null)
        {
            await FetchDataAsync(false, queryable).ConfigureAwait(false);
            await PopulatePreviousRecordsIfRequired().ConfigureAwait(false);
        }
        else
        {
            chartDecorator.Records = await GetRecords(data).ConfigureAwait(false);
            chartDecorator.PreviousRecords = await GetRecords(Strategy.GetDataToCompare()).ConfigureAwait(false);
        }
    }

    private async Task PopulatePreviousRecordsIfRequired()
    {
        if (Chart.ComparisonMode == ComparisonMode.Enabled)
        {
            if (chartDecorator.PreviousRecords == null)
            {
                var previousData = Strategy.GetDataToCompare();
                if (previousData != null)
                {
                    chartDecorator.PreviousRecords = await GetRecords(previousData).ConfigureAwait(false);
                }
                else
                {
                    var previousQueryable = Strategy.BuildQueryableToCompare();

                    if (previousQueryable != null)
                    {
                        await FetchDataAsync(true, previousQueryable).ConfigureAwait(false);
                    }
                }
            }
        }
    }

    private async Task<IList> GetRecords(object data)
    {
        if (data == null)
        {
            return null;
        }

        if (TaskHelper.IsValueTask(data))
        {
            isInDataFetchingMode = true;
            try
            {
                return (IList)await TaskHelper.GetObjectFromValueTask(data).ConfigureAwait(false);
            }
            finally
            {
                isInDataFetchingMode = false;
            }
        }

        return (IList)data;
    }

    private async Task FetchDataAsync(bool previousData, IQueryable queryable)
    {
        if (previousData)
        {
            if (queryable == null)
            {
                if (previousData)
                {
                    chartDecorator.PreviousRecords = null;
                }
                else
                {
                    chartDecorator.Records = null;
                }

                return;
            }
        }

        try
        {
            isInDataFetchingMode = true;
            var data = await queryable.GetList(Chart.InternItems).ConfigureAwait(false);
            var al = Chart.AttributesLookup;
            if (al == null)
            {
                al = GetAttributesLookup(queryable);
            }

            attributesLookup = al;

            if (previousData)
            {
                chartDecorator.PreviousRecords = data;
            }
            else
            {
                chartDecorator.Records = data;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            if (previousData)
            {
                chartDecorator.PreviousRecords = null;
            }
            else
            {
                chartDecorator.Records = null;
            }

            throw;
        }
        finally
        {
            isInDataFetchingMode = false;
        }
    }

    private string GetGridOrChartToggleIcon()
    {
        return Chart.ChartMode ? MudBlazor.Icons.Material.Filled.ListAlt : MudBlazor.Icons.Material.Filled.InsertChart;
    }

    private void HandleChartAndTableModeToggle()
    {
        Chart.ChartMode = !Chart.ChartMode;
        StateHasChanged();
    }

    private void ExportToExcel()
    {
        var sheetName = Chart.Title;
        if (string.IsNullOrWhiteSpace(sheetName))
        {
            sheetName = "Data";
        }

        if (chartDecorator.PreviousRecords == null)
        {
            ExportHelper.ExportToExcel(chartDecorator.Records, sheetName, JSRuntime);
        }
        else
        {
            ExportHelper.ExportToExcel(JSRuntime, Chart.Title,
                new KeyValuePair<IEnumerable, string>(chartDecorator.Records, sheetName),
                new KeyValuePair<IEnumerable, string>(chartDecorator.PreviousRecords, "PAST-" + sheetName));
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync().ConfigureAwait(false);
        chartDecorator.Records = null;
        chartDecorator.PreviousRecords = null;
        await ReloadCore().ConfigureAwait(false);
    }

    private bool IsStackable()
    {
        if (!isStackable)
        {
            return false;
        }

        if (chartDecorator.Records == null)
        {
            return false;
        }

        var elementType = CoreReflectionHelper.GetEnumerableItemTypeIfEnumerable(chartDecorator.Records);
        var measures = Strategy.GetMeasures();
        var dimensions = Strategy.GetDimensions();
        return ChartHelper.IsStackable(elementType, dimensions, measures, attributesLookup);
    }

    private List<ExtendedApexChartType> GetChartTypes()
    {
        if (chartDecorator.Records == null)
        {
            return [];
        }

        var elementType = CoreReflectionHelper.GetEnumerableItemTypeIfEnumerable(chartDecorator.Records);
        var measures = Strategy.GetMeasures();
        var dimensions = Strategy.GetDimensions();
        var validChartTypes = ChartHelper.GetAllowedChartTypes(elementType, dimensions, measures, attributesLookup)
            .Where(x => x != Chart.ChartType);

        if (Chart.ComparisonMode == ComparisonMode.Enabled)
        {
            validChartTypes = validChartTypes.Where(x => x.IsMixableChart());
        }

        return validChartTypes.ToList();
    }

    private void OnChartTypeChanged(string chartType)
    {
        var newChartType = (ExtendedApexChartType)Enum.Parse(typeof(ExtendedApexChartType), chartType);

        if (newChartType != Chart.ChartType)
        {
            Chart.ChartType = newChartType;
            StateHasChanged();
        }
    }

    private RenderFragment RenderCoreComponents()
    {
        return BuildRenderTreeCore;
    }

    private void BuildRenderTreeCore(RenderTreeBuilder builder)
    {
        //#if DEBUG
        //        System.Console.WriteLine($"Rendering {Chart.GetType().Name}:{Chart.Title}");
        //#endif
        if (chartDecorator.Records == null)
        {
            return;
        }

        var sequencer = new Sequencer();
        if (Chart.ChartMode)
        {
            var chartAttributes = new Dictionary<string, object>();

            var hasSpaceConstraints = Chart.AllowMinMax && !IsFullScreen;
            chartAttributes.Add("HasSpaceConstraint", hasSpaceConstraints);
            //chartAttributes.Add("FormatYAxisLabel", this.Chart.KMNumberFormat ? minifyDecimalFunc : null);

            var measures = Strategy.GetMeasures();
            IReadOnlyCollection<string> dimensions = null;

            if (Chart.ComparisonMode == ComparisonMode.Enabled && chartDecorator.PreviousRecords != null)
            {
                dimensions = Strategy.GetDimensionsForComparison();
            }

            if (dimensions == null || dimensions.Count == 0)
            {
                dimensions = Strategy.GetDimensions();
            }

            var primary = chartDecorator.Records;
            var secondary = chartDecorator.PreviousRecords;

            var entityType = CoreReflectionHelper.GetEnumerableItemTypeIfEnumerable(primary);

            if (dimensions == null || dimensions.Count == 0 || measures == null || measures.Count == 0)
            {
                var dimensionsAndMeasures = DimensionsAndMeasuresHelper.GetDimensionsAndMeasuresProperties(entityType,
                    knownDimensions: dimensions, attributesLookup: attributesLookup);

                if (dimensions == null || dimensions.Count == 0)
                {
                    dimensions = dimensionsAndMeasures.Key.Select(x => x.Name).ToList();
                }

                if (measures == null || measures.Count == 0)
                {
                    measures = dimensionsAndMeasures.Value
                        .Select(x => new MeasureAndSeriesType(x.Name, ExtendedApexSeriesType.Auto)).ToList();
                }
            }

            var callback = Strategy.GetChartItemClickCallback(entityType);
            if (callback == null)
            {
                var seriesLookup = new Dictionary<string, string>(measures
                    .Select(x => entityType.GetProperty(x.PropertyName)).Select(x =>
                        new KeyValuePair<string, string>(x.GetMemberDisplayName(true, attributesLookup), x.Name)));
                callback = GetChartItemClickCallback(entityType, dimensions, seriesLookup);
            }

            var chartType = Chart.ChartType;
            var validChartTypes = ChartHelper.GetAllowedChartTypes(entityType, dimensions, measures, attributesLookup);
            if (!validChartTypes.Contains(chartType))
            {
                if (validChartTypes.Contains(ExtendedApexChartType.Column))
                {
                    chartType = ExtendedApexChartType.Column;
                }
                else
                {
                    chartType = validChartTypes.FirstOrDefault();
                }
            }

            var chartHeight = hasSpaceConstraints ? Chart.Height ?? ChartConfig.ChartHeight : null;
            if (chartHeight == "0")
            {
                chartHeight = null;
            }

            var chartConfig = new ChartConfig
            {
                ChartType = chartDecorator.Records.Count < 2 && !Chart.Stacked && !chartType.IsCircularChart()
                    ? ExtendedApexChartType.Column
                    : chartType,
                DimensionPropertyNames = dimensions,
                MeasureAndSeriesTypes = measures,
                SelectedDataCallback = callback,
                Stacked = Chart.Stacked,
                AttributesLookup = attributesLookup,
                ChartAttributes = chartAttributes,
                KMNumberFormat = Chart.KMNumberFormat,
                MaxItemsInChart = Chart.MaxItemsInChart,
                XAxisSortingOrder = Chart.XAxisSortingOrder,
                Height = chartHeight,
                ShowDataLabels = Chart.ShowDataLabels,
                IsDarkMode = LayoutService.IsDarkMode
            };
            var previousDataToCompare = Chart.ComparisonMode == ComparisonMode.Enabled ? secondary : null;

            ChartHelper.AddChart(builder, sequencer, primary, previousDataToCompare, chartConfig);
        }
        else
        {
            var childRowContent = Strategy.GetChildRowContent();
            var templateColumnContent = Strategy.GetTemplateColumnContent();
            DataGridHelper.AddDataGrid(builder, sequencer, chartDecorator.Records, Chart.ShowSequence, Chart.PageSize,
                attributesLookup, Chart.KMNumberFormat, Chart.SimpleGridView, childRowContent: childRowContent, templateColumnContent: templateColumnContent);
        }
    }

    private AttributesLookup GetAttributesLookup(IQueryable queryable)
    {
        if (Chart.AttributesLookup != null)
        {
            return Chart.AttributesLookup;
        }

        if (queryable == null)
        {
            return null;
        }

        var toSeed = LinqHelper.ExploreToSeed(queryable);
        return new AttributesLookup(toSeed);
    }

    protected override string GetClass()
    {
        var cls = base.GetClass();
        if (!string.IsNullOrEmpty(cls))
        {
            cls += " ";
        }

        cls += "justify-center";
        return cls;
    }

    private async void ComparisonChanged(bool enabled)
    {
        Chart.ComparisonMode = enabled ? ComparisonMode.Enabled : ComparisonMode.OnDemand;
        if (Chart.ComparisonMode == ComparisonMode.Enabled)
        {
            chartDecorator.PreviousSettings = new ChartSettings
            {
                ChartType = Chart.ChartType,
                Stacked = Chart.Stacked
            };
            Chart.Stacked = false;
            if (Chart.ChartType.IsSingleSeriesChartType())
            {
                Chart.ChartType = ExtendedApexChartType.Line;
            }
        }
        else
        {
            if (chartDecorator.PreviousSettings != null)
            {
                Chart.ChartType = chartDecorator.PreviousSettings.ChartType;
                Chart.Stacked = chartDecorator.PreviousSettings.Stacked;
                chartDecorator.PreviousSettings = null;
            }
        }

        await PopulatePreviousRecordsIfRequired().ConfigureAwait(false);
        StateHasChanged();
    }

    private bool CanCompare()
    {
        if (Chart.ComparisonMode == ComparisonMode.Disabled)
        {
            return false;
        }

        var queryable = Strategy.BuildQueryableToCompare();
        if (queryable != null)
        {
            return true;
        }

        var data = Strategy.GetDataToCompare();
        return data != null;
    }

    private object GetChartItemClickCallback(Type entityType, IReadOnlyCollection<string> dimensions,
        IDictionary<string, string> seriesLookup)
    {
        if (Chart.ExplorerInfo == null)
        {
            return null;
        }

        var callbackProxy =
            (IChartCallbackProxy)Activator.CreateInstance(typeof(ChartCallbackHandler<>).MakeGenericType(entityType));
        callbackProxy.OnObjectSelected += (s, e) => { OnChartDataPointSelected(e, dimensions, seriesLookup); };
        return callbackProxy.GetCallbackAction();
    }

    private async void OnChartDataPointSelected(ChartCallbackEventArgs e, IReadOnlyCollection<string> dimensions,
        IDictionary<string, string> seriesLookup)
    {
        if (!seriesLookup.TryGetValue(e.SeriesName, out var seriesProperty))
        {
            return;
        }

        var dimensionsValue = (string)e.SelectedObject;

        var values = PropertiesToStringValuesHelper.GetWhereArguments(dimensionsValue);
        var queryParams = new Dictionary<string, object>((IEnumerable<KeyValuePair<string, object>>)Chart.ExplorerInfo.QueryParameters ?? Array.Empty<KeyValuePair<string, object>>());
        var i = 0;
        foreach (var dimension in dimensions)
        {
            queryParams[dimension] = values[i];
            i++;
        }

        queryParams["__measure"] = seriesProperty;

        var url = BlazorHelper.BuildQueryForPageTranfer(Chart.ExplorerInfo.ExplorerPageType, queryParams);
        await NavigateToUrl(url, false, true).ConfigureAwait(false);
    }
}