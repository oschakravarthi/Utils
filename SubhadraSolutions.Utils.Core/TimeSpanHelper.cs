﻿using System;
using System.Text;

namespace SubhadraSolutions.Utils;

public static class TimeSpanHelper
{
    private static readonly TimeSpan[] BUCKETS = [TimeSpan.FromDays(365.25636), TimeSpan.FromDays(30), TimeSpan.FromDays(1), TimeSpan.FromHours(1), TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1)];

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
    public static int[] ToBuckets(this TimeSpan timeSpan, params TimeSpan[] buckets)
    {
        var result = new int[buckets.Length];
        var ticks = timeSpan.Ticks;
        for (int i = 0; i < buckets.Length; i++)
        {
            var b = buckets[i].Ticks;
            var v = (int)(ticks / b);
            result[i] = v;
            ticks %= b;
        }
        return result;
    }
    public static string ToYearsMonthsDaysHoursMinutesSeconds(this TimeSpan timeSpan)
    {
        return ToBucketsWithLegends(timeSpan, BUCKETS, null);
    }
    public static string ToBucketsWithLegends(this TimeSpan timeSpan, TimeSpan[] buckets, string[] legends = null)
    {
        var ticks = timeSpan.Ticks;
        var sb = new StringBuilder();
        for (int i = 0; i < buckets.Length; i++)
        {
            var b = buckets[i];
            var v = ticks / b.Ticks;
            var vs = v.ToString();
            if (i > 0)
            {
                sb.Append(legends == null ? "-" : " - ");
            }
            if (vs.Length == 1)
            {
                vs = "0" + vs;
            }
            if (legends != null)
            {
                sb.Append($"{vs} {legends[i]}");
            }
            else
            {
                sb.Append($"{vs}");
            }
            ticks %= b.Ticks;
        }
        
        return sb.ToString();
    }
}