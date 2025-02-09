using System;

namespace SubhadraSolutions.Utils.Core
{
    public static class SiderealTimeCalculator
    {
        // Converts degrees to hours
        private static double DegreesToHours(double degrees)
        {
            return degrees / 15.0;
        }

        // Normalizes an angle to the range [0, 24) hours
        private static double NormalizeTo24Hours(double hours)
        {
            hours %= 24;
            if (hours < 0) hours += 24;
            return hours;
        }

        // Calculates the Julian Date for a given UTC DateTime
        public static double CalculateJulianDate(this DateTime dateTimeUTC)
        {
            int year = dateTimeUTC.Year;
            int month = dateTimeUTC.Month;
            double day = dateTimeUTC.Day +
                         dateTimeUTC.Hour / 24.0 +
                         dateTimeUTC.Minute / 1440.0 +
                         dateTimeUTC.Second / 86400.0;

            if (month <= 2)
            {
                year--;
                month += 12;
            }

            int A = year / 100;
            int B = 2 - A + (A / 4);

            return Math.Floor(365.25 * (year + 4716)) +
                   Math.Floor(30.6001 * (month + 1)) +
                   day + B - 1524.5;
        }

        // Calculates Greenwich Sidereal Time in hours
        private static double CalculateGST(this DateTime dateTimeUTC)
        {
            double JD = CalculateJulianDate(dateTimeUTC);
            double D = JD - 2451545.0;

            double GST = 280.46061837 + 360.98564736629 * D;
            return NormalizeTo24Hours(DegreesToHours(GST));
        }

        // Calculates Local Sidereal Time in hours for a given longitude
        public static DateTime CalculateLST(this DateTime dateTimeUTC, double longitude)
        {
            double GST = CalculateGST(dateTimeUTC);
            double LST = GST + DegreesToHours(longitude);
            return dateTimeUTC.AddHours(LST);
        }
    }

}
