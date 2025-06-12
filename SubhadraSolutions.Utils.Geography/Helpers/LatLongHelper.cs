using GeoTimeZone;
using System;
using TimeZoneConverter;

namespace SubhadraSolutions.Utils.Geography.Helpers
{
    public static class LatLongHelper
    {
        public static TimeZoneInfo GetTimeZoneInfo(double latitude, double longitude)
        {
            string tz = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
            TimeZoneInfo tzInfo = TZConvert.GetTimeZoneInfo(tz);
            return tzInfo;
        }

    }
}