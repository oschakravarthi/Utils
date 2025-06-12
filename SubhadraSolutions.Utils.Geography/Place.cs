using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Geography
{
    public class Place : GeoLatLong
    {
        public const double DefaultTimeZone = 82.5;
        /*Oslo GPS coordinates*/

        //public const double DefaultLatitude = 59.911491;
        //public const double DefaultLongitude = 10.757933;

        public Place(double latitude, double longitude, double timeZone)
            :base(latitude, longitude)
        {
            TimeZone = timeZone;
        }

        [JsonInclude] 
        public double TimeZone { get; private set; }


        //public string LatitudeDMS { get { return Ephemeris.ToDMS(this.Latitude); } }
        //public string LongitudeDMS { get { return Ephemeris.ToDMS(this.Longitude); } }
    }
}