using ApexCharts;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;
using SubhadraSolutions.Utils.Blazor.Components.Charts.Helpers;
using SubhadraSolutions.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public class ExtendedApexChart<TItem> : ApexChart<TItem>, IExtendedApexChart, IDisposable where TItem : class
{
    public ExtendedApexChart()
    {
        Options = new ApexChartOptions<TItem>();
    }

    [Parameter] public IList<TItem> Data { get; set; }

    public IApexChartOptions ExtendedOptions => (IApexChartOptions)Options;

    [Parameter] public bool HasSpaceConstraint { get; set; }

    [Parameter] public bool Stacked { get; set; }

    [Inject] private LayoutService LayoutService { get; set; }

    void IDisposable.Dispose()
    {
        LayoutService.MajorUpdateOccured -= LayoutService_MajorUpdateOccured;
        Dispose();
    }

    //[CascadingParameter]
    //public ExtendedMudCardContent Container { get; protected set; }

    public string GetJson()
    {
        OnParametersSet();
        //var isReadyField = typeof(ApexChart<TItem>).GetField("isReady", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        //isReadyField.SetValue(this, true);

        var m = typeof(ApexChart<TItem>).GetMethod("PrepareChart", BindingFlags.Instance | BindingFlags.NonPublic);
        m.Invoke(this, Array.Empty<object>());

        var serializeMethod =
            typeof(ApexChart<TItem>).GetMethod("Serialize", BindingFlags.Instance | BindingFlags.NonPublic);
        var json = (string)serializeMethod.Invoke(this, [Options]);
        return json;
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        SetChartBehaviour();
        if (firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        return RenderAsync();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LayoutService.MajorUpdateOccured += LayoutService_MajorUpdateOccured;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        PopulateOptions();
    }

    private async void LayoutService_MajorUpdateOccured(object sender, EventArgs e)
    {
        SetTheme();
        await RenderAsync().ConfigureAwait(false);
    }

    private void PopulateOptions()
    {
        SetTheme();
        var options = Options;

        if (options.Chart.Animations == null)
        {
            options.Chart.Animations = new Animations();
        }
        //Options.Chart.Animations.Enabled = numberOfRecords <= 100;

        //options.Chart.Width = this.Width?? "100%";
        //this.Height = this.Height ?? "auto";

        if (options.Xaxis != null)
        {
            if (options.Xaxis.Labels == null)
            {
                options.Xaxis.Labels = new XAxisLabels();
            }

            if (XAxisType == ApexCharts.XAxisType.Datetime)
            {
                options.Xaxis.Labels.DatetimeUTC = false;
                options.Xaxis.Labels.Format = GlobalSettings.Instance.DateAndTimeFormat;
            }

            options.Xaxis.Labels.Show = true;
        }

        if (options.Chart == null)
        {
            options.Chart = new Chart();
        }

        if (Stacked)
        {
            options.Chart.Stacked = true;
        }

        //options.Chart.StackType = StackType.Percent100;
        if (options.Chart.Toolbar == null)
        {
            options.Chart.Toolbar = new Toolbar();
        }

        if (options.PlotOptions == null)
        {
            options.PlotOptions = new PlotOptions();
        }

        options.PlotOptions.Treemap = new PlotOptionsTreemap
        {
            Distributed = true,
            EnableShades = false
        };

        //if (options.DataLabels != null)
        //{
        //    if (options.DataLabels.Style == null)
        //    {
        //        options.DataLabels.Style = new DataLabelsStyle
        //        {
        //            Colors = new List<string> { LayoutService.IsDarkMode ? "#ffffff" : "#000000" },
        //            FontSize = "12px"
        //        };
        //    }
        //}

        // var chart = options.Chart;
        // chart.Toolbar=new Toolbar
        // {
        //    Tools=new Tools
        //    {
        //    }
        // }
        // chart.ForeColor = MudBlazor.Color.Primary.;
    }

    private void SetChartBehaviour()
    {
        var numberOfRecords = Data.Count;
        var seriesCount = Series.Count;
        var totalNumber = numberOfRecords * seriesCount;
        var options = Options;
        //options.Chart.Animations.Enabled = totalNumber <= 200;
        options.Chart.Animations.Enabled = false;

        //options.Chart.Animations.Enabled = false;

        options.Chart.Toolbar.Show = true;
        //options.Chart.Toolbar.Show = !this.HasSpaceConstraint;

        var numberOfItemsInLegend = SetColors();
        options.Legend = new Legend
        {
            Position = LegendPosition.Right,
            ShowForSingleSeries = true,
            Show = true
        };

        if (HasSpaceConstraint)
        {
            options.Legend.Position = LegendPosition.Bottom;

            if (numberOfItemsInLegend > 10 /* || apexSeries.Count > 20*/)
            {
                options.Legend.Show = false;
            }
        }
    }

    private int SetColors()
    {
        var numberOfRecords = Data.Count;
        var seriesCount = Series.Count;
        var options = Options;
        var colorsSet = false;
        var numberOfItemsInLegend = seriesCount;
        for (var i = 0; i < seriesCount; i++)
        {
            var series = Series[i];
            var seriesTypeProperty = series.GetType().GetProperty("SeriesType");
            if (seriesTypeProperty != null)
            {
                var seriesType = (SeriesType)seriesTypeProperty.GetValue(series);
                if (ChartHelper.IsSingleSeriesType(seriesType) || seriesType == SeriesType.Treemap)
                {
                    options.Colors = ColorPalettes.GetPalette((uint)numberOfRecords);
                    numberOfItemsInLegend = numberOfRecords;
                    colorsSet = true;
                    break;
                }
            }
        }

        if (!colorsSet)
        {
            var seriesColors = ColorPalettes.GetPalette((uint)seriesCount);
            for (var i = 0; i < seriesCount; i++)
            {
                var series = Series[i];
                series.Color = seriesColors[i];
            }
        }
        else
        {
            for (var i = 0; i < seriesCount; i++)
            {
                var series = Series[i];
                series.Color = null;
            }
        }

        return numberOfItemsInLegend;
    }

    private void SetTheme()
    {
        var options = Options;
        if (options.Theme == null)
        {
            options.Theme = new Theme();
        }

        options.Theme.Mode = LayoutService.IsDarkMode ? Mode.Dark : Mode.Light;
    }
}