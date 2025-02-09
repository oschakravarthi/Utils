using System;
using System.Text;

namespace SubhadraSolutions.Utils;

public static class TimeSpanHelper
{
    private static readonly long[] BUCKETS = new long[] { TimeSpan.FromDays(365).Ticks, TimeSpan.FromDays(30).Ticks, TimeSpan.FromDays(1).Ticks, TimeSpan.FromHours(1).Ticks, TimeSpan.FromMinutes(1).Ticks, TimeSpan.FromSeconds(1).Ticks };

    public static string DescribeTimeSpan(int milliseconds = 0, int seconds = 0, int minutes = 0)
    {
        return DescribeTimeSpan(new TimeSpan(0, 0, minutes, seconds, milliseconds));
    }

    public static string DescribeTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalMilliseconds < 2000.0)
        {
            return $"{timeSpan.TotalMilliseconds,4:F0} ms ";
        }

        if (timeSpan.TotalSeconds < 120.0)
        {
            return $"{timeSpan.TotalSeconds,4:G3} sec";
        }

        return $"{timeSpan.TotalMinutes,4:G3} min";
    }
    public static TimeSpan StripMilliseconds(this TimeSpan time)
    {
        return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
    }
    public static string TimeSpanToString(TimeSpan timeSpan)
    {
        if (timeSpan == TimeSpan.FromDays(1))
        {
            return "Daily";
        }

        if (timeSpan == TimeSpan.FromDays(7))
        {
            return "Weekly";
        }

        if (timeSpan == TimeSpan.FromDays(30))
        {
            return "Monthly";
        }

        return timeSpan.ToString();
    }

    public static string ToYearsMonthsDaysHoursMinutesSeconds(this TimeSpan timeSpan)
    {
        var ticks = timeSpan.Ticks;
        var sb = new StringBuilder();
        for (int i = 0; i < BUCKETS.Length; i++)
        {
            var b = BUCKETS[i];
            var v = (long)(ticks / b);
            sb.Append($"{v}-");
            ticks %= b;
        }
        if (sb.Length > 0)
        {
            sb.Length--;
        }
        return sb.ToString();
    }
}