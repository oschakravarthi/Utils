using SubhadraSolutions.Utils.Shared.Mathematics;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Geography
{
    public class Meridian : Place
    {
        public static readonly Meridian Rajamahendravaramu = new Meridian(17.18144, 81.36903, DefaultTimeZone, "రాజమహేంద్రవరము");
        public static readonly Meridian Visakhapatnamu = new Meridian(17.7336, 83.3836, DefaultTimeZone, "విశాఖపట్టణము");
        public static readonly Meridian Bengaluru = new Meridian(12.9944071, DMSHelper.FromDMS(77, 35, 0), DefaultTimeZone, "బెంగళూరు");
        public static readonly Meridian Pedanandipalli = new Meridian(17.9273601, 82.9952979, DefaultTimeZone, "పెదనందిపల్లి");
        public static readonly Meridian Chicago = new Meridian(41.85003, -87.65005, DefaultTimeZone, "చికాగో");
        //public static readonly Place Ujjayini = new Place(23.18239, 75.77, DefaultAltitude, "ఉజ్జయినీ");
        public static readonly Meridian Ujjayini = new Meridian(DMSHelper.FromDMS(23, 11, 0), DMSHelper.FromDMS(75, 41, 0), DefaultTimeZone, "ఉజ్జయినీ");
        public static readonly Meridian Golkonda = new Meridian(17.3833, 78.4011, DefaultTimeZone, "గోల్కొండ");
        public static readonly Meridian Kakinada = new Meridian(16.96036, 82.23809, DefaultTimeZone, "Kakinada");
        public static readonly Meridian Default = Rajamahendravaramu;
        public static readonly Meridian NEMANI = new Meridian(16.96036, 82.390895, DefaultTimeZone, "NEMANI");
        public static readonly Meridian Vijayawada = new Meridian(16.50745, 80.6466, DefaultTimeZone,"Vijayawada");
        public static readonly Meridian Eluru = new Meridian(16.7152607, 81.0823865, DefaultTimeZone, "Eluru");

        public Meridian(double latitude, double longitude, double timeZone, string name)
            : this(latitude, longitude, timeZone, name, null, null, null)
        {
        }

        [JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public Meridian(double latitude, double longitude, double timeZone, string name, string district, string state, string country)
            :base(latitude, longitude, timeZone)
        {
            Name = name;
            District = district;
            State = state;
            Country = country;
        }

        [JsonInclude]
        public string Name { get; private set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        

        //public static bool operator ==(City left, City right)
        //{
        //    if (ReferenceEquals(left, null))
        //    {
        //        return ReferenceEquals(right, null);
        //    }

        public override string ToString()
        {
            return $"{Name} - {District} - {State} - {Country} ({Latitude}, {Longitude})";
        }
    }
}