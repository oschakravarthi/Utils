using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Pages;

public abstract class AbstractDateRangedSmartPage : AbstractSmartPage
{
    protected readonly int defaultHistoryDays = 7 * 4;

    protected readonly int maxHistoryDays = 7 * 52;

    protected DateRange allowedDateRange;

    private DateTime? _from, _upto;

    protected AbstractDateRangedSmartPage()
    {
    }

    protected AbstractDateRangedSmartPage(int defaultHistoryDays, int maxHistoryDays)
    {
        this.defaultHistoryDays = defaultHistoryDays;
        this.maxHistoryDays = maxHistoryDays;
    }

    [Parameter]
    public DateTime? From
    { get => _from; set => _from = value; }

    [Parameter]
    public DateTime? Upto
    { get => _upto; set => _upto = value; }

    protected abstract Task<DateRange> GetAllowedDateRangeAsync();

    protected DateRange GetDateRange()
    {
        if (_from == null || _upto == null)
        {
            return null;
        }
        return new(_from.Value.Date, _upto.Value.Date.AddDays(-1).Date);
    }

    protected bool HasValidDateRange()
    {
        return this.From != null && this.Upto != null;
    }

    protected Task OnDateRangeSelection(DateRange range)
    {
        _from = range.Start.Value.Date;
        _upto = range.End.Value.Date.AddDays(1);
        return ResetIfParametersAreChanged(true);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        this.allowedDateRange = await GetAllowedDateRangeAsync().ConfigureAwait(false);

        PopulateParametersFromUri();
        //DateRangeHelper.EnsureSafeDateRanges(ref allowedDateRange, ref _from, ref _upto, defaultHistoryDays, maxHistoryDays);
        this._upto = this._upto.Value.AddDays(1).Date;
        if (HasValidDateRange())
        {
            await ResetIfParametersAreChanged(true).ConfigureAwait(false);
        }
    }

    protected override void Reset()
    {
        base.Reset();
        if (this.From != null && this.Upto != null)
        {
            Reset(this.From.Value, this.Upto.Value);
        }
    }

    protected override Task ResetAsync()
    {
        if (this.From != null && this.Upto != null)
        {
            return ResetAsync(this.From.Value, this.Upto.Value);
        }
        return Task.CompletedTask;
    }

    protected virtual void Reset(DateTime from, DateTime upto)
    {
    }

    protected virtual Task ResetAsync(DateTime from, DateTime upto)
    {
        return null;
    }
}