using SubhadraSolutions.Utils.Mathematics;
using System.Text;

namespace SubhadraSolutions.Utils.Shared.Mathematics
{
    public static class DMSHelper
    {
        public static double FromDMS(double degrees, double minutes, double seconds)
        {
            return GeometryHelper.NormalizeDegrees(degrees + minutes / 60.0 + seconds / 3600.0);
        }
        //public static string NakshatrasToNakshatraFormat(double nakshatras)
        //{
        //    nakshatras = GeometryHelper.MakePositiveDegrees(nakshatras);
        //    return ToCustomFormat(nakshatras * 60, [800, 800 / 60.0, 800 / 3600.0, 800 / (3600.0 * 60.0), 800 / (3600.0 * 3600.0)]);
        //}
        public static string ToDMS(double degrees)
        {
            //return ToCustomFormat(GeometryHelper.MakePositiveDegrees(value), [30, 1.0, 1 / 60.0, 1 / 3600.0]);
            if (double.IsNaN(degrees))
            {
                return "nan";
            }

            var sb = new StringBuilder();
            bool isNegative = degrees < 0;
            degrees = GeometryHelper.NormalizeDegrees(degrees);
            if (isNegative)
            {
                degrees -= 360;
                degrees = -degrees;
                sb.Append("-");
            }

            //var divisors = new double[] { 30, 1.0, 1 / 60.0, 1 / 3600.0 };
            //var symbols = new string[] { "☉︎", "°", "′", "″" };
            var divisors = new[] { 1.0, 1 / 60.0, 1 / 3600.0 };
            var symbols = new[] { "°", "′", "″" };
            var v = degrees;
            for (int i = 0; i < divisors.Length; i++)
            {
                var x = (int)(v / divisors[i]);
                sb.Append($"{x}{symbols[i]} ");
                v = v - x * divisors[i];
            }
            //sb.Append($"{v}");
            if (sb.Length > 0)
            {
                sb.Length--;
            }
            return sb.ToString();
        }

    }
}
