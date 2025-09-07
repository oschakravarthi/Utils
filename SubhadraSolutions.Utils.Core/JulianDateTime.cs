namespace SubhadraSolutions.Utils
{
    using System;

    public struct JulianDateTime : IComparable<JulianDateTime>, IEquatable<JulianDateTime>
    {
        // Julian Day Number (integer part)
        public double Value { get; }

        // Constructor from Julian Day
        public JulianDateTime(double value)
        {
            Value = value;
        }

        // Conversion from DateTime (assumes UTC)
        public static JulianDateTime FromDateTime(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
                dt = dt.ToUniversalTime();

            int year = dt.Year;
            int month = dt.Month;
            double day = dt.Day +
                         (dt.Hour / 24.0) +
                         (dt.Minute / 1440.0) +
                         (dt.Second / 86400.0) +
                         (dt.Millisecond / 86400000.0);

            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }

            int A = year / 100;
            int B = 2 - A + (A / 4);

            double jd = Math.Floor(365.25 * (year + 4716))
                        + Math.Floor(30.6001 * (month + 1))
                        + day + B - 1524.5;

            return new JulianDateTime(jd);
        }

        // Convert back to DateTime (UTC)
        public DateTime ToDateTime()
        {
            double jd = Value + 0.5;
            int Z = (int)Math.Floor(jd);
            double F = jd - Z;

            int A = Z;
            if (Z >= 2299161)
            {
                int alpha = (int)((Z - 1867216.25) / 36524.25);
                A += 1 + alpha - (alpha / 4);
            }

            int B = A + 1524;
            int C = (int)((B - 122.1) / 365.25);
            int D = (int)(365.25 * C);
            int E = (int)((B - D) / 30.6001);

            double day = B - D - Math.Floor(30.6001 * E) + F;
            int month = (E < 14) ? E - 1 : E - 13;
            int year = (month > 2) ? C - 4716 : C - 4715;

            int dayInt = (int)Math.Floor(day);
            double fracDay = day - dayInt;

            int hour = (int)(fracDay * 24);
            fracDay -= hour / 24.0;
            int minute = (int)(fracDay * 1440);
            fracDay -= minute / 1440.0;
            int second = (int)(fracDay * 86400);
            int millisecond = (int)((fracDay * 86400 - second) * 1000);

            return new DateTime(year, month, dayInt, hour, minute, second, millisecond, DateTimeKind.Utc);
        }

        // Operators
        public static bool operator ==(JulianDateTime left, JulianDateTime right) => left.Equals(right);
        public static bool operator !=(JulianDateTime left, JulianDateTime right) => !left.Equals(right);
        public static bool operator <(JulianDateTime left, JulianDateTime right) => left.Value < right.Value;
        public static bool operator >(JulianDateTime left, JulianDateTime right) => left.Value > right.Value;
        public static bool operator <=(JulianDateTime left, JulianDateTime right) => left.Value <= right.Value;
        public static bool operator >=(JulianDateTime left, JulianDateTime right) => left.Value >= right.Value;

        // ✅ Arithmetic with JulianTimeSpan
        public static JulianDateTime operator +(JulianDateTime dt, JulianTimeSpan ts)
            => new JulianDateTime(dt.Value + ts.TotalDays);
        public static JulianDateTime operator +(JulianDateTime dt, TimeSpan ts)
            => new JulianDateTime(dt.Value + ts.TotalDays);

        public static JulianDateTime operator -(JulianDateTime dt, JulianTimeSpan ts)
            => new JulianDateTime(dt.Value - ts.TotalDays);
        public static JulianDateTime operator -(JulianDateTime dt, TimeSpan ts)
            => new JulianDateTime(dt.Value - ts.TotalDays);

        public static JulianTimeSpan operator -(JulianDateTime a, JulianDateTime b)
            => new JulianTimeSpan(a.Value - b.Value);
        public int CompareTo(JulianDateTime other) => Value.CompareTo(other.Value);
        public bool Equals(JulianDateTime other) => Value.Equals(other.Value);
        public override bool Equals(object obj) => obj is JulianDateTime other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"JD {Value:0.000000}";
    }

}
