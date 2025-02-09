using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components;

public abstract class AbstractDateRangedSmartComponent : AbstractSmartComponent
{
    protected DateRange allowedDateRange = null;

    protected AbstractDateRangedSmartComponent()
    {
        Upto = DateTime.UtcNow.Date.AddDays(1);
        From = Upto.AddDays(-7);
    }

    [Parameter] public DateTime From { get; set; }

    [Parameter] public DateTime Upto { get; set; }

    protected DateRange GetDateRange()
    {
        return new DateRange(From.Date, Upto.Date.AddDays(-1).Date);
    }

    protected Task OnDateRangeSelection(DateRange range)
    {
        From = range.Start.Value.Date;
        Upto = range.End.Value.Date.AddDays(1);
        return ResetIfParametersAreChanged(true);
    }

    protected override void Reset()
    {
        base.Reset();
        var from = From.Date;
        var upto = Upto.Date;
        Reset(from, upto);
    }

    protected virtual void Reset(DateTime from, DateTime upto)
    {
    }

    protected override Task ResetAsync()
    {
        var from = From.Date;
        var upto = Upto.Date;
        return ResetAsync(from, upto);
    }

    protected virtual Task ResetAsync(DateTime from, DateTime upto)
    {
        return null;
    }
}