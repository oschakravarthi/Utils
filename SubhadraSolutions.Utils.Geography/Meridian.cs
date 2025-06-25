using SubhadraSolutions.Utils.Shared.Mathematics;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace SubhadraSolutions.Utils.Geography
{
    public class Meridian : Place
    {
        public static readonly Meridian UTC = new Meridian(0, 0, TimeSpan.FromHours(0), "UTC");

        public static readonly Meridian Mooresville = new Meridian(35.58486, -80.81007, TimeSpan.FromHours(-5), "Mooresville");
        public static readonly Meridian Rajamahendravaramu = new Meridian(17.18144, 81.36903, DefaultUtcOffset, "రాజమహేంద్రవరము");
        public static readonly Meridian Visakhapatnamu = new Meridian(17.7336, DMSHelper.FromDMS(83, 13, 0), DefaultUtcOffset, "విశాఖపట్టణము");
        public static readonly Meridian Bengaluru = new Meridian(12.9944071, DMSHelper.FromDMS(77, 35, 0), DefaultUtcOffset, "బెంగళూరు");
        public static readonly Meridian Pedanandipalli = new Meridian(17.9273601, 82.9952979, DefaultUtcOffset, "పెదనందిపల్లి");
        public static readonly Meridian Chicago = new Meridian(41.85003, -87.65005, DefaultUtcOffset, "చికాగో");
        //public static readonly Place Ujjayini = new Meridian(23.18239, 75.77, DefaultUtcOffset, "ఉజ్జయినీ");
        public static readonly Meridian Ujjayini = new Meridian(DMSHelper.FromDMS(23, 11, 0), DMSHelper.FromDMS(75, 45, 0), DefaultUtcOffset, "ఉజ్జయినీ");//75.683333
        public static readonly Meridian Golkonda = new Meridian(17.3833, 78.4011, DefaultUtcOffset, "గోల్కొండ");
        public static readonly Meridian Kakinada = new Meridian(16.96036, 82.23809, DefaultUtcOffset, "Kakinada");
        public static readonly Meridian Default = Rajamahendravaramu;
        public static readonly Meridian NEMANI = new Meridian(16.96036, 82.390895, DefaultUtcOffset, "NEMANI");
        public static readonly Meridian Vijayawada = new Meridian(16.50745, 80.6466, DefaultUtcOffset,"Vijayawada");
        public static readonly Meridian Eluru = new Meridian(16.7152607, 81.0823865, DefaultUtcOffset, "Eluru");
        public static readonly Meridian Dharwada = new Meridian(21.598582, 78.2359883, DefaultUtcOffset, "ధార్వాడ");
        public static readonly Meridian Hyderabad = new Meridian(17.38405, 78.45636, DefaultUtcOffset, "Hyderabad");
        public static readonly Meridian Tirupati = new Meridian(13.6277325, 79.3853037, DefaultUtcOffset, "తిరుపతి");
        public static readonly Meridian Anantapuram = new Meridian(DMSHelper.FromDMS(14, 41, 0), DMSHelper.FromDMS(77, 36, 0), DefaultUtcOffset, "అనంతపురము");

        public Meridian(double latitude, double longitude, TimeSpan utcOffset, string name)
            : this(latitude, longitude, utcOffset, name, null, null, null)
        {
        }

        [JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public Meridian(double latitude, double longitude, TimeSpan utcOffset, string name, string district, string state, string country)
            :base(latitude, longitude, utcOffset)
        {
            Name = name;
            //District = district;
            State = state;
            Country = country;
        }

        [JsonInclude]
        public string Name { get; private set; }
        //public string District { get; set; }
        [JsonInclude] 
        public string State { get; private set; }
        [JsonInclude] 
        public string Country { get; private set; }


        //public static bool operator ==(City left, City right)
        //{
        //    if (ReferenceEquals(left, null))
        //    {
        //        return ReferenceEquals(right, null);
        //    }

        public override string ToString()
        {
            var sb = new StringBuilder();

            //if (this.District != null)
            //{
            //    sb.Append($" - {this.District}");
            //}
            if (this.State != null)
            {
                sb.Append($" - {this.State}");
            }
            if (this.Country != null)
            {
                sb.Append($" - {this.Country}");
            }


            return $"{Name}{sb.ToString()} ({Latitude}, {Longitude})";
        }
    }
}