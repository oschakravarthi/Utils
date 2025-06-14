using System;
using System.Globalization;
using System.Reflection;

namespace SubhadraSolutions.Utils;

public static class DateTimeHelper
{
    public static readonly MethodInfo DateTimeToStringMethodInfo =
        typeof(DateTimeHelper).GetMethod(nameof(DateTimeToString), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo StringToDateTimeMethodInfo =
        typeof(DateTimeHelper).GetMethod(nameof(StringToDateTime), BindingFlags.Public | BindingFlags.Static);

    /// <summary>
    ///     Adds the given number of days while adjusting for any offset changes.
    ///     Useful for capturing changes caused by daylight savings.
    ///     <see cref="DateTime.Kind" /> must be <see cref="DateTimeKind.Local" />
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><see cref="DateTime.Kind" /> != <see cref="DateTimeKind.Local" /></exception>
    public static DateTime AddDaysOffsetAdjusted(this DateTime dateTime, double days)
    {
        if (dateTime.Kind != DateTimeKind.Local)
        {
            throw new ArgumentException($"{nameof(DateTime.Kind)} must be {nameof(DateTimeKind.Local)}.");
        }

        return dateTime.ToUniversalTime().AddDays(days).ToLocalTime();
    }

    public static double ToJulianDayUTC(this DateTime dateTimeUTC)
    {
        return ToJulianDateTime(dateTimeUTC.Year, dateTimeUTC.Month, dateTimeUTC.Day, 0, 0, 0);
    }
    public static double ToJulianDateUTC(this DateTime dateTimeUTC)
    {
        //if (dateTimeUTC.Kind == DateTimeKind.Local)
        //{
        //    dateTimeUTC = dateTimeUTC.ToUniversalTime();
        //}
        return ToJulianDateTime(dateTimeUTC.Year, dateTimeUTC.Month, dateTimeUTC.Day, dateTimeUTC.Hour, dateTimeUTC.Minute, dateTimeUTC.Second);
    }

    public static double ToJulianDateTime(int year, int month, int day, int hour, int minute, int second)
    {
        return GetJulianDay(year, month, day, hour, minute, second, true);
    }
    private static double GetJulianDay(int year, int month, int day, int hour, int minute, int second, bool isGregorian)
    {
        var time = hour + minute / 60.0 + second / 3600.0;

        double jd;
        double u, u0, u1, u2;
        u = year;
        if (month < 3) u -= 1;
        u0 = u + 4712.0;
        u1 = month + 1.0;
        if (u1 < 4) u1 += 12.0;
        jd = Math.Floor(u0 * 365.25)
           + Math.Floor(30.6 * u1 + 0.000001)
           + day + time / 24.0 - 63.5;
        if (isGregorian)
        {
            u2 = Math.Floor(Math.Abs(u) / 100) - Math.Floor(Math.Abs(u) / 400);
            if (u < 0.0) u2 = -u2;
            jd = jd - u2 + 2;
            if ((u < 0.0) && (u / 100 == Math.Floor(u / 100)) && (u / 400 != Math.Floor(u / 400)))
                jd -= 1;
        }
        return jd;
    }
    public static DateTime ToLocalMeanTime(this DateTime dateTimeUTC, double timeZone)
    {
        return ToLocalMeanTime(dateTimeUTC, 0, timeZone);
    }
    public static DateTime ToLocalMeanTime(this DateTime dateTime, double fromTimeZone, double toTimeZone)
    {
        var diff = (toTimeZone - fromTimeZone) / 360.0;
        var result = new DateTime(dateTime.AddDays(diff).Ticks, toTimeZone == 0 ? DateTimeKind.Utc : DateTimeKind.Local);
        return result;
    }

    //public static TimeOnly ToLMST(this DateTime dateTimeUTC, double longitude)
    //{
    //    var julianDay = JulDay(dateTimeUTC.Day, dateTimeUTC.Month, dateTimeUTC.Year, dateTimeUTC.TimeOfDay.TotalHours);
    //    return LM_Sidereal_Time(julianDay, longitude);
    //}

    //public static TimeOnly ToGMST(this DateTime dateTimeUTC)
    //{
    //    var julianDay = JulDay(dateTimeUTC.Day, dateTimeUTC.Month, dateTimeUTC.Year, 0);
    //    return LM_Sidereal_Time(julianDay, 0.0);
    //}

    //private static TimeOnly LM_Sidereal_Time(double jd, double longitude)
    //{
    //    var gmst = GM_SiderealTime(jd);
    //    var lmst = 24.0 * frac((gmst + longitude / 15.0) / 24.0);

    //    var h = Math.Floor(lmst);
    //    var min = Math.Floor(60.0 * frac(lmst));
    //    var secs = Math.Round(60.0 * (60.0 * frac(lmst) - min));
    //    if (secs == 60)
    //    {
    //        secs = 0;
    //        min = min + 1;
    //    }

    //    return new TimeOnly((int)h, (int)min, (int)secs);
    //}
    private static double frac(double x)
    {
        x = x - Math.Floor(x);
        if (x < 0) 
        { 
            x = x + 1.0; 
        }
        return x;
    }
    private static double GM_SiderealTime(double jd)
    {
        var MJD = jd - 2400000.5;
        var MJD0 = Math.Floor(MJD);
        var ut = (MJD - MJD0) * 24.0;
        var t_eph = (MJD0 - 51544.5) / 36525.0;
        var result = 6.697374558 + 1.0027379093 * ut + (8640184.812866 + (0.093104 - 0.0000062 * t_eph) * t_eph) * t_eph / 3600.0;
        return result;
    }

    public static double JulDay(int date, int month, int year, double ut)
    {
        if (year < 1900) 
        {
            year = year + 1900;
        }
      if (month <= 2) 
        { 
            month = month + 12;
            year = year - 1; 
        }
        var A = Math.Floor(year / 100.0);
        var B = -13;
        var JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + date + B - 1524.5 + ut / 24.0;
        return JD;
     }

    /// <summary>
    ///     Adds the given number of hours while adjusting for any offset changes.
    ///     Useful for capturing changes caused by daylight savings.
    ///     <see cref="DateTime.Kind" /> must be <see cref="DateTimeKind.Local" />
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="hours"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><see cref="DateTime.Kind" /> != <see cref="DateTimeKind.Local" /></exception>
    public static DateTime AddHoursOffsetAdjusted(this DateTime dateTime, double hours)
    {
        if (dateTime.Kind != DateTimeKind.Local)
        {
            throw new ArgumentException($"{nameof(DateTime.Kind)} must be {nameof(DateTimeKind.Local)}.");
        }

        return dateTime.ToUniversalTime().AddHours(hours).ToLocalTime();
    }

    /// <summary>
    ///     Adds the given number of milliseconds while adjusting for any offset changes.
    ///     Useful for capturing changes caused by daylight savings.
    ///     <see cref="DateTime.Kind" /> must be <see cref="DateTimeKind.Local" />
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><see cref="DateTime.Kind" /> != <see cref="DateTimeKind.Local" /></exception>
    public static DateTime AddMillisecondsOffsetAdjusted(this DateTime dateTime, double milliseconds)
    {
        if (dateTime.Kind != DateTimeKind.Local)
        {
            throw new ArgumentException($"{nameof(DateTime.Kind)} must be {nameof(DateTimeKind.Local)}.");
        }

        return dateTime.ToUniversalTime().AddMilliseconds(milliseconds).ToLocalTime();
    }

    /// <summary>
    ///     Adds the given number of minutes while adjusting for any offset changes.
    ///     Useful for capturing changes caused by daylight savings.
    ///     <see cref="DateTime.Kind" /> must be <see cref="DateTimeKind.Local" />
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="minutes"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><see cref="DateTime.Kind" /> != <see cref="DateTimeKind.Local" /></exception>
    public static DateTime AddMinutesOffsetAdjusted(this DateTime dateTime, double minutes)
    {
        if (dateTime.Kind != DateTimeKind.Local)
        {
            throw new ArgumentException($"{nameof(DateTime.Kind)} must be {nameof(DateTimeKind.Local)}.");
        }

        return dateTime.ToUniversalTime().AddMinutes(minutes).ToLocalTime();
    }

    /// <summary>
    ///     Adds the given number of months while adjusting for any offset changes.
    ///     Useful for capturing changes caused by daylight savings.
    ///     <see cref="DateTime.Kind" /> must be <see cref="DateTimeKind.Local" />
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="months"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><see cref="DateTime.Kind" /> != <see cref="DateTimeKind.Local" /></exception>
    public static DateTime AddMonthsOffsetAdjusted(this DateTime dateTime, int months)
    {
        if (dateTime.Kind != DateTimeKind.Local)
        {
            throw new ArgumentException($"{nameof(DateTime.Kind)} must be {nameof(DateTimeKind.Local)}.");
        }

        return dateTime.ToUniversalTime().AddMonths(months).ToLocalTime();
    }

    /// <summary>
    ///     Adds the given number of seconds while adjusting for any offset changes.
    ///     Useful for capturing changes caused by daylight savings.
    ///     <see cref="DateTime.Kind" /> must be <see cref="DateTimeKind.Local" />
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><see cref="DateTime.Kind" /> != <see cref="DateTimeKind.Local" /></exception>
    public static DateTime AddSecondsOffsetAdjusted(this DateTime dateTime, double seconds)
    {
        if (dateTime.Kind != DateTimeKind.Local)
        {
            throw new ArgumentException($"{nameof(DateTime.Kind)} must be {nameof(DateTimeKind.Local)}.");
        }

        return dateTime.ToUniversalTime().AddSeconds(seconds).ToLocalTime();
    }

    // todo: lift datetimekind restriction by passing timezoneinfo
    /// <summary>
    ///     Adds the given number of ticks while adjusting for any offset changes.
    ///     Useful for capturing changes caused by daylight savings.
    ///     <see cref="DateTime.Kind" /> must be <see cref="DateTimeKind.Local" />
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="ticks"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><see cref="DateTime.Kind" /> != <see cref="DateTimeKind.Local" /></exception>
    public static DateTime AddTicksOffsetAdjusted(this DateTime dateTime, long ticks)
    {
        if (dateTime.Kind != DateTimeKind.Local)
        {
            throw new ArgumentException($"{nameof(DateTime.Kind)} must be {nameof(DateTimeKind.Local)}.");
        }

        return dateTime.ToUniversalTime().AddTicks(ticks).ToLocalTime();
    }

    public static string DateTimeToString(DateTime dateTime)
    {
        if (dateTime.Date == dateTime)
        {
            return dateTime.ToString(GlobalSettings.Instance.DateOnlySerializationFormat);
        }

        return dateTime.ToString(GlobalSettings.Instance.DateAndTimeSerializationFormat);
    }

    public static DateTime EndOfMonth(this DateTime date)
    {
        return StartOfMonth(date).AddMonths(1).AddDays(-1.0);
    }

    public static DateTime EndOfWeek(this DateTime date)
    {
        return StartOfWeek(date).AddDays(6.0);
    }

    public static DateTime NthOf(DateTime from, int occurrence, DayOfWeek day)
    {
        if (occurrence <= 0)
        {
            throw new ArgumentException("It should be >0", nameof(occurrence));
        }

        from = from.Date;
        var diff = (day - from.DayOfWeek + 7) % 7;
        return from.AddDays(7 * (occurrence - 1) + diff);
    }

    public static DateTime NthOfFromBeginningOfMonth(int year, int month, int occurrence, DayOfWeek day)
    {
        return NthOf(new DateTime(year, month, 1), occurrence, day);
    }

    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    public static DateTime StartOfWeek(this DateTime date)
    {
        var num = date.DayOfWeek - DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
        if (num < 0)
        {
            num += 7;
        }

        return date.AddDays(-num).Date;
    }

    public static DateTime StringToDateTime(string s)
    {
        var format = s.Length == 10
            ? GlobalSettings.Instance.DateOnlySerializationFormat
            : GlobalSettings.Instance.DateAndTimeSerializationFormat;
        if (DateTime.TryParseExact(s, format, null, DateTimeStyles.None, out var dateTime))
        {
            return dateTime;
        }

        return DateTime.Parse(s);
    }
}