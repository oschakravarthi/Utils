using System;
using System.Text;

namespace SubhadraSolutions.Utils
{
    public static class FormatHelper
    {
        public static string ToCustomFormat(double value, bool shortAndRound, params double[] divisors)
        {
            if (double.IsNaN(value))
            {
                return "nan";
            }
            var sb = new StringBuilder();
            if (value < 0)
            {
                sb.Append('-');
            }
            value = Math.Abs(value);
            if (divisors.Length > 1 && shortAndRound)
            {
                var d = divisors[divisors.Length - 2];
                var rem = value % d;
                if (rem / d > 0.5)
                {
                    value += d;
                }
            }
            for (int i = 0; i < divisors.Length; i++)
            {
                if (divisors.Length > 1 && i == divisors.Length - 1 && shortAndRound)
                {
                    break;
                }
                if (i > 0)
                {
                    sb.Append("-");
                }
                var x = (int)(value / divisors[i]);
                var s = x.ToString();
                if (!shortAndRound)
                {
                    if (s.Length == 1)
                    {
                        s = "0" + s;
                    }
                }
                sb.Append($"{s}");
                
                //v = v - (x * divisors[i]);
                value = value % divisors[i];
            }
            //sb.Append($"{v}");
            var result = sb.ToString();

            return result;
        }

        //public static string ToCustomFormatWithRounding(double value, params double[] divisors)
        //{
        //    if (double.IsNaN(value))
        //    {
        //        return "nan";
        //    }
        //    int[] result = new int[divisors.Length];
        //    var sb = new StringBuilder();
        //    if (value < 0)
        //    {
        //        sb.Append('-');
        //    }
        //    var v = Math.Abs(value);
        //    for (int i = 0; i < divisors.Length; i++)
        //    {
        //        var x = (int)(v / divisors[i]);
        //        result[i] = x;

        //        var s = x.ToString();
        //        sb.Append($"{s}");
        //        v = v - (x * divisors[i]);
        //    }

        //    //sb.Append($"{v}");
        //    return sb.ToString();
        //}
    }
}
