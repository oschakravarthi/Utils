using GeoTimeZone;
using System;
using TimeZoneConverter;

namespace SubhadraSolutions.Utils.Geography;

public static class GeographyHelper
{
    public static TimeZoneInfo GetTimeZoneInfo(double latitude, double longitude)
    {
        var tzIana = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        var tzInfo = TZConvert.GetTimeZoneInfo(tzIana);
        return tzInfo;
    }
}