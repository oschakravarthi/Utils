namespace SubhadraSolutions.Utils
{
    using System;

    public struct JulianTimeSpan : IComparable<JulianTimeSpan>, IEquatable<JulianTimeSpan>
    {
        // Store value as days (fractional)
        public double TotalDays { get; }

        // Constructors
        public JulianTimeSpan(double totalDays)
        {
            TotalDays = totalDays;
        }

        public static JulianTimeSpan FromDays(double days) => new JulianTimeSpan(days);
        public static JulianTimeSpan FromHours(double hours) => new JulianTimeSpan(hours / 24.0);
        public static JulianTimeSpan FromMinutes(double minutes) => new JulianTimeSpan(minutes / 1440.0);
        public static JulianTimeSpan FromSeconds(double seconds) => new JulianTimeSpan(seconds / 86400.0);

        // Properties
        public double TotalHours => TotalDays * 24.0;
        public double TotalMinutes => TotalDays * 1440.0;
        public double TotalSeconds => TotalDays * 86400.0;

        // Conversion to TimeSpan
        public TimeSpan ToTimeSpan() => TimeSpan.FromDays(TotalDays);

        public static JulianTimeSpan FromTimeSpan(TimeSpan ts) => new JulianTimeSpan(ts.TotalDays);

        // Operators
        public static JulianTimeSpan operator +(JulianTimeSpan a, JulianTimeSpan b) => new JulianTimeSpan(a.TotalDays + b.TotalDays);
        public static JulianTimeSpan operator -(JulianTimeSpan a, JulianTimeSpan b) => new JulianTimeSpan(a.TotalDays - b.TotalDays);
        public static JulianTimeSpan operator -(JulianTimeSpan a) => new JulianTimeSpan(-a.TotalDays);
        public static JulianTimeSpan operator *(JulianTimeSpan a, double factor) => new JulianTimeSpan(a.TotalDays * factor);
        public static JulianTimeSpan operator /(JulianTimeSpan a, double divisor) => new JulianTimeSpan(a.TotalDays / divisor);

        public static JulianTimeSpan operator *(double factor, JulianTimeSpan a) => new JulianTimeSpan(a.TotalDays * factor);
        public static JulianTimeSpan operator /(double divisor, JulianTimeSpan a) => new JulianTimeSpan(a.TotalDays / divisor);
        
        public static JulianTimeSpan operator +(JulianTimeSpan a, TimeSpan b) => new JulianTimeSpan(a.TotalDays + b.TotalDays);
        public static JulianTimeSpan operator -(JulianTimeSpan a, TimeSpan b) => new JulianTimeSpan(a.TotalDays - b.TotalDays);
        public static JulianTimeSpan operator +(TimeSpan a, JulianTimeSpan b) => new JulianTimeSpan(a.TotalDays + b.TotalDays);
        public static JulianTimeSpan operator -(TimeSpan a, JulianTimeSpan b) => new JulianTimeSpan(a.TotalDays - b.TotalDays);
        public static double operator /(JulianTimeSpan a, JulianTimeSpan b)
        {
            if (b.TotalDays == 0)
                throw new DivideByZeroException("Cannot divide by a zero-length JulianTimeSpan.");
            return a.TotalDays / b.TotalDays;
        }
        public static double operator /(JulianTimeSpan a, TimeSpan b)
        {
            if (b.TotalDays == 0)
                throw new DivideByZeroException("Cannot divide by a zero-length JulianTimeSpan.");
            return a.TotalDays / b.TotalDays;
        }
        public static JulianTimeSpan operator /(TimeSpan a, JulianTimeSpan b)
        {
            if (b.TotalDays == 0)
                throw new DivideByZeroException("Cannot divide by a zero-length JulianTimeSpan.");
            return new JulianTimeSpan(a.TotalDays / b.TotalDays);
        }

        public static JulianTimeSpan operator *(JulianTimeSpan a, JulianTimeSpan b)
        {
            return new JulianTimeSpan(a.TotalDays * b.TotalDays);
        }
        public static JulianTimeSpan operator *(JulianTimeSpan a, TimeSpan b)
        {
            return new JulianTimeSpan(a.TotalDays * b.TotalDays);
        }
        public static bool operator ==(JulianTimeSpan left, JulianTimeSpan right) => left.Equals(right);
        public static bool operator !=(JulianTimeSpan left, JulianTimeSpan right) => !left.Equals(right);
        public static bool operator <(JulianTimeSpan left, JulianTimeSpan right) => left.TotalDays < right.TotalDays;
        public static bool operator >(JulianTimeSpan left, JulianTimeSpan right) => left.TotalDays > right.TotalDays;
        public static bool operator <=(JulianTimeSpan left, JulianTimeSpan right) => left.TotalDays <= right.TotalDays;
        public static bool operator >=(JulianTimeSpan left, JulianTimeSpan right) => left.TotalDays >= right.TotalDays;

        // Interfaces
        public int CompareTo(JulianTimeSpan other) => TotalDays.CompareTo(other.TotalDays);
        public bool Equals(JulianTimeSpan other) => TotalDays.Equals(other.TotalDays);
        public override bool Equals(object obj) => obj is JulianTimeSpan other && Equals(other);
        public override int GetHashCode() => TotalDays.GetHashCode();

        public override string ToString() => $"{TotalDays:0.000000} days";
    }

}
