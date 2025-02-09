using ApexCharts;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts
{
    public partial class DateTimeRangeChart : AbstractMaximizableSmartComponent
    {
        [Parameter]
        public List<DateTimeRanged<string>> Data { get; set; }

        [Parameter]
        public string Height { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public IDictionary<string, string> ColorsLookup { get; set; }

        private ApexChartOptions<DateTimeRanged<string>> options;

        public DateTimeRangeChart()
        {
            Outlined = true;
        }

        protected override void Reset()
        {
            options = new ApexChartOptions<DateTimeRanged<string>>
            {
                PlotOptions = new PlotOptions
                {
                    Bar = new PlotOptionsBar
                    {
                        Horizontal = true,
                    }
                },
                Tooltip = new Tooltip
                {
                    X = new TooltipX
                    {
                        Format = "dd MMM yyyy HH:mm"
                    }
                },
                Xaxis = new XAxis
                {
                    Min = this.Data[0].From.ToUnixTimeMilliseconds(),
                    Max = this.Data[this.Data.Count - 1].Upto.ToUnixTimeMilliseconds(),
                }
            };
        }

        private string GetColor(DateTimeRanged<string> e)
        {
            if (this.ColorsLookup == null)
            {
                return null;
            }
            if (e.Info == null)
            {
                return "#000000";
            }
            this.ColorsLookup.TryGetValue(e.Info, out var color);
            return color;
        }
    }
}