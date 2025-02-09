using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class SmartDateRangePicker
{
    private ExtendedDateRangePicker _picker;

    public SmartDateRangePicker()
    {
        Label = "Date Range";
        Margin = Margin.Dense;
        Variant = Variant.Outlined;
        InnerPadding = false;
    }

    [Parameter]
    public DateRange AllowedDateRange { get; set; }

    [Parameter]
    public DateRange DateRange { get; set; }

    [Parameter]
    public EventCallback<DateRange> DateRangeChanged { get; set; }

    //[Parameter]
    //public int? MaxNumberOfDaysAllowed { get; set; }

    [Parameter]
    public int? StaleDays { get; set; }

    private DateRange GetDataRangeForDateRangePicker()
    {
        return new DateRange(this.DateRange.Start.Value.Date, this.DateRange.End.Value.Date);
    }

    private async void OnDateRangeSelection(DateRange dateRange)
    {
        //_picker.ResetValidation();
        if (dateRange is { Start: not null, End: not null })
        {
            //var diff = dateRange.End.Value - dateRange.Start.Value;
            if (DateRange != dateRange)
            {
                DateRange = dateRange;
                var arg = new DateRange(dateRange.Start.Value.Date, dateRange.End.Value.Date);
                await DateRangeChanged.InvokeAsync(arg).ConfigureAwait(false);
            }
        }
    }
}