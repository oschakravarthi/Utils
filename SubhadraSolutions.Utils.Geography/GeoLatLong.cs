using SubhadraSolutions.Utils.Geography.Helpers;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Geography
{
    public class GeoLatLong
    {
        public GeoLatLong(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
        [JsonInclude]
        public double Latitude { get; private set; }

        [JsonInclude]
        public double Longitude { get; private set; }

        public double DistanceTo(Meridian other)
        {
            var d = GeographyHelper.GetDistance(Latitude, Longitude, other.Latitude, other.Longitude);
            return d;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return Equals(obj as Meridian);
        }
    }
}
