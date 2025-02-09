using MudBlazor;
using System;

namespace SubhadraSolutions.Utils.Blazor.Helpers;

public static class DateRangeHelper
{
    public static void EnsureSafeDateRanges(ref DateRange allowedDateRange, ref DateTime? from, ref DateTime? upto,
        int defaultHistoryDays, int maxHistoryDays)
    {
        var today = GlobalSettings.Instance.DateTimeNow.Date;
        if (allowedDateRange == null)
        {
            allowedDateRange = new DateRange(today.AddDays(-maxHistoryDays), today);
        }
        else
        {
            if (allowedDateRange.Start == null || allowedDateRange.End == null)
            {
                allowedDateRange.Start = today.AddDays(-maxHistoryDays);
                allowedDateRange.End = today;
            }
        }

        from = allowedDateRange.End.Value.Date.AddDays(-defaultHistoryDays + 1);
        upto = allowedDateRange.End.Value;
    }
}