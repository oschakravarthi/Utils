using System;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Geography
{
    public class Place : GeoLatLong
    {
        public static TimeSpan DefaultUtcOffset = TimeSpan.FromMinutes(330);
        /*Oslo GPS coordinates*/

        //public const double DefaultLatitude = 59.911491;
        //public const double DefaultLongitude = 10.757933;

        public Place(double latitude, double longitude, TimeSpan utcOffset)
            :base(latitude, longitude)
        {
            UtcOffset = utcOffset;
        }

        [JsonInclude] 
        public TimeSpan UtcOffset { get; private set; }


        //public string LatitudeDMS { get { return Ephemeris.ToDMS(this.Latitude); } }
        //public string LongitudeDMS { get { return Ephemeris.ToDMS(this.Longitude); } }
    }
}