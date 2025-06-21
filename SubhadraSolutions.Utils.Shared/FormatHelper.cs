using System;
using System.Text;

namespace SubhadraSolutions.Utils
{
    public static class FormatHelper
    {
        public static string ToCustomFormat(double value, params double[] divisons)
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
            var v = Math.Abs(value);
            for (int i = 0; i < divisons.Length; i++)
            {
                var x = (int)(v / divisons[i]);
                var s = x.ToString();
                if(s.Length==1)
                {
                    s = "0" + s;
                }
                sb.Append($"{s}");
                if (i < divisons.Length - 1)
                {
                    sb.Append("-");
                }
                v = v - (x * divisons[i]);
            }
            //sb.Append($"{v}");
            return sb.ToString();
        }
    }
}
