using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Geography.Helpers
{
    public static class GeographyHelper
    {
        public static double GetDistance<T>(T from, T to) where T : GeoLatLong
        {
            return GetDistance(from.Latitude, from.Longitude, to.Latitude, to.Longitude);
        }
        public static double GetDistance(double latitude, double longitude, double otherLatitude, double otherLongitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
        public static T GetNearest<T>(T originMeridian, IEnumerable<T> locations) where T : GeoLatLong
        {
            return GetNearest(originMeridian.Latitude, originMeridian.Longitude, locations);
        }
        public static T GetNearest<T>(double latitude, double longitude, IEnumerable<T> locations) where T:GeoLatLong
        {
            T nearestMeridian = null;
            double nearest = 0;
            foreach (var meridian in locations)
            {
                if (nearestMeridian == null)
                {
                    nearestMeridian = meridian;
                    nearest = GetDistance(latitude, longitude, meridian.Latitude, meridian.Longitude);
                }
                else
                {
                    var distance = GetDistance(latitude, longitude, meridian.Latitude, meridian.Longitude);
                    if (distance < nearest)
                    {
                        nearestMeridian = meridian;
                        nearest = distance;
                    }
                }
            }

            return nearestMeridian;
        }
    }
}
