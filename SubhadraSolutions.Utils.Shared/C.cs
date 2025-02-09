using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SubhadraSolutions.Utils
{
    public static class C
    {
        private static readonly char[] fchars = "0123456789.+-Ee".ToCharArray();

        private static readonly char[] ichars = "0123456789".ToCharArray();

        public static double atof(string s)
        {
            s = (s ?? string.Empty).Trim();
            int num = s.IndexOfFirstNot(fchars);
            if (num >= 0)
            {
                s = s.Substring(0, num);
            }

            double result;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return 0.0;
        }

        public static int atoi(string s)
        {
            s = (s ?? string.Empty).Trim();
            int num = s.IndexOfFirstNot(ichars);
            if (num >= 0)
            {
                s = s.Substring(0, num);
            }

            int result;
            if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double fmod(double numer, double denom)
        {
            return numer % denom;
        }

        public static void fprintf(TextWriter Destination, string Format, params object[] Parameters)
        {
            Destination.Write(sprintf(Format, Parameters));
        }

        public static bool IsNumericType(object o)
        {
            if (!(o is byte) && !(o is sbyte) && !(o is short) && !(o is ushort) && !(o is int) && !(o is uint) && !(o is long) && !(o is ulong) && !(o is float) && !(o is double))
            {
                return o is decimal;
            }

            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static void fclose(CFile file)
        //{
        //    file?.Dispose();
        //}
        public static bool IsPositive(object Value, bool ZeroIsPositive)
        {
            switch (Value.GetType().GetTypeCode())
            {
                case TypeCode.SByte:
                    if (!ZeroIsPositive)
                    {
                        return (sbyte)Value > 0;
                    }

                    return (sbyte)Value >= 0;

                case TypeCode.Int16:
                    if (!ZeroIsPositive)
                    {
                        return (short)Value > 0;
                    }

                    return (short)Value >= 0;

                case TypeCode.Int32:
                    if (!ZeroIsPositive)
                    {
                        return (int)Value > 0;
                    }

                    return (int)Value >= 0;

                case TypeCode.Int64:
                    if (!ZeroIsPositive)
                    {
                        return (long)Value > 0;
                    }

                    return (long)Value >= 0;

                case TypeCode.Single:
                    if (!ZeroIsPositive)
                    {
                        return (float)Value > 0f;
                    }

                    return (float)Value >= 0f;

                case TypeCode.Double:
                    if (!ZeroIsPositive)
                    {
                        return (double)Value > 0.0;
                    }

                    return (double)Value >= 0.0;

                case TypeCode.Decimal:
                    if (!ZeroIsPositive)
                    {
                        return (decimal)Value > 0m;
                    }

                    return (decimal)Value >= 0m;

                case TypeCode.Byte:
                    if (!ZeroIsPositive)
                    {
                        return (byte)Value > 0;
                    }

                    return true;

                case TypeCode.UInt16:
                    if (!ZeroIsPositive)
                    {
                        return (ushort)Value > 0;
                    }

                    return true;

                case TypeCode.UInt32:
                    if (!ZeroIsPositive)
                    {
                        return (uint)Value != 0;
                    }

                    return true;

                case TypeCode.UInt64:
                    if (!ZeroIsPositive)
                    {
                        return (ulong)Value != 0;
                    }

                    return true;

                case TypeCode.Char:
                    if (!ZeroIsPositive)
                    {
                        return (char)Value != '\0';
                    }

                    return true;

                default:
                    return false;
            }
        }

        public static string ReplaceMetaChars(string input)
        {
            return Regex.Replace(input, "(\\\\)(\\d{3}|[^\\d]|0)?", new MatchEvaluator(ReplaceMetaCharsMatch));
        }

        public static string sprintf(string Format, params object[] Parameters)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Regex regex = new Regex("\\%(\\d*\\$)?([\\'\\#\\-\\+ ]*)(\\d*)(?:\\.(\\d+))?([hl])?([dioxXucsfeEgGpn%])");
            int num = 0;
            stringBuilder.Append(Format);
            Match match = regex.Match(stringBuilder.ToString());
            while (match.Success)
            {
                int num4 = num;
                if (match.Groups[1]?.Value.Length > 0)
                {
                    num4 = Convert.ToInt32(match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - 1)) - 1;
                }

                bool flag2 = false;
                bool flag = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                bool flag6 = false;
                if (match.Groups[2]?.Value.Length > 0)
                {
                    string value = match.Groups[2].Value;
                    flag2 = value.IndexOf('#') >= 0;
                    flag = value.IndexOf('-') >= 0;
                    flag3 = value.IndexOf('+') >= 0;
                    flag4 = value.IndexOf(' ') >= 0;
                    flag6 = value.IndexOf('\'') >= 0;
                    if (flag3 && flag4)
                    {
                        flag4 = false;
                    }
                }

                char c3 = ' ';
                int num2 = int.MinValue;
                if (match.Groups[3]?.Value.Length > 0)
                {
                    num2 = Convert.ToInt32(match.Groups[3].Value);
                    flag5 = match.Groups[3].Value[0] == '0';
                }

                if (flag5)
                {
                    c3 = '0';
                }

                if (flag && flag5)
                {
                    c3 = ' ';
                }

                int num3 = int.MinValue;
                if (match.Groups[4]?.Value.Length > 0)
                {
                    num3 = Convert.ToInt32(match.Groups[4].Value);
                }

                char c = '\0';
                if (match.Groups[5]?.Value.Length > 0)
                {
                    c = match.Groups[5].Value[0];
                }

                char c2 = '\0';
                if (match.Groups[6]?.Value.Length > 0)
                {
                    c2 = match.Groups[6].Value[0];
                }

                if (num3 == int.MinValue && c2 != 's' && c2 != 'c' && char.ToUpper(c2) != 'X' && c2 != 'o')
                {
                    num3 = 6;
                }

                object obj;
                if (Parameters == null || num4 >= Parameters.Length)
                {
                    obj = null;
                }
                else
                {
                    obj = Parameters[num4];
                    switch (c)
                    {
                        case 'h':
                            if (obj is int)
                            {
                                obj = (short)(int)obj;
                            }
                            else if (obj is long)
                            {
                                obj = (short)(long)obj;
                            }
                            else if (obj is uint)
                            {
                                obj = (ushort)(uint)obj;
                            }
                            else if (obj is ulong)
                            {
                                obj = (ushort)(ulong)obj;
                            }

                            break;

                        case 'l':
                            if (obj is short)
                            {
                                obj = (long)(short)obj;
                            }
                            else if (obj is int)
                            {
                                obj = (long)(int)obj;
                            }
                            else if (obj is ushort)
                            {
                                obj = (ulong)(ushort)obj;
                            }
                            else if (obj is uint)
                            {
                                obj = (ulong)(uint)obj;
                            }

                            break;
                    }
                }

                string empty = string.Empty;
                switch (c2)
                {
                    case '%':
                        empty = "%";
                        break;

                    case 'd':
                    case 'i':
                        empty = FormatNumber(flag6 ? "n" : "d", flag2, num2, int.MinValue, flag, flag3, flag4, c3, obj);
                        num++;
                        break;

                    case 'o':
                        empty = FormatOct("o", flag2, num2, int.MinValue, flag, c3, obj);
                        num++;
                        break;

                    case 'x':
                        empty = FormatHex("x", flag2, num2, num3, flag, c3, obj);
                        num++;
                        break;

                    case 'X':
                        empty = FormatHex("X", flag2, num2, num3, flag, c3, obj);
                        num++;
                        break;

                    case 'u':
                        empty = FormatNumber(flag6 ? "n" : "d", flag2, num2, int.MinValue, flag, PositiveSign: false, PositiveSpace: false, c3, ToUnsigned(obj));
                        num++;
                        break;

                    case 'c':
                        if (IsNumericType(obj))
                        {
                            empty = Convert.ToChar(obj).ToString();
                        }
                        else if (obj is char)
                        {
                            empty = ((char)obj).ToString();
                        }
                        else if (obj is string && ((string)obj).Length > 0)
                        {
                            empty = ((string)obj)[0].ToString();
                        }

                        num++;
                        break;

                    case 's':
                        _ = "{0" + (num2 != int.MinValue ? "," + (flag ? "-" : string.Empty) + num2 : string.Empty) + ":s}";
                        empty = (obj ?? string.Empty).ToString();
                        if (num3 >= 0)
                        {
                            empty = empty.Substring(0, num3);
                        }

                        if (num2 != int.MinValue)
                        {
                            empty = !flag ? empty.PadLeft(num2, c3) : empty.PadRight(num2, c3);
                        }

                        num++;
                        break;

                    case 'f':
                        empty = FormatNumber(flag6 ? "n" : "f", flag2, num2, num3, flag, flag3, flag4, c3, obj);
                        num++;
                        break;

                    case 'e':
                        empty = FormatNumber("e", flag2, num2, num3, flag, flag3, flag4, c3, obj);
                        num++;
                        break;

                    case 'E':
                        empty = FormatNumber("E", flag2, num2, num3, flag, flag3, flag4, c3, obj);
                        num++;
                        break;

                    case 'g':
                        empty = FormatNumber("g", flag2, num2, num3, flag, flag3, flag4, c3, obj);
                        num++;
                        break;

                    case 'G':
                        empty = FormatNumber("G", flag2, num2, num3, flag, flag3, flag4, c3, obj);
                        num++;
                        break;

                    case 'p':
                        if (obj is IntPtr)
                        {
                            empty = "0x" + ((IntPtr)obj).ToInt64().ToString("x");
                        }

                        num++;
                        break;

                    case 'n':
                        empty = FormatNumber("d", flag2, num2, int.MinValue, flag, flag3, flag4, c3, match.Index);
                        break;
                }

                stringBuilder.Remove(match.Index, match.Length);
                stringBuilder.Insert(match.Index, empty);
                match = regex.Match(stringBuilder.ToString(), match.Index + empty.Length);
            }

            return stringBuilder.ToString();
        }

        public static int sscanf<T>(string input, string format, ref T r)
        {
            ScanFormatted scanFormatted = new ScanFormatted();
            scanFormatted.Parse(input, format);
            int count = scanFormatted.Results.Count;
            if (count > 0)
            {
                r = (T)scanFormatted.Results[0];
            }

            return count;
        }

        public static int sscanf<T1, T2>(string input, string format, ref T1 r1, ref T2 r2)
        {
            ScanFormatted scanFormatted = new ScanFormatted();
            scanFormatted.Parse(input, format);
            int count = scanFormatted.Results.Count;
            if (count > 0)
            {
                r1 = (T1)scanFormatted.Results[0];
            }

            if (count > 1)
            {
                r2 = (T2)scanFormatted.Results[1];
            }

            return count;
        }

        public static int sscanf<T1, T2, T3>(string input, string format, ref T1 r1, ref T2 r2, ref T3 r3)
        {
            ScanFormatted scanFormatted = new ScanFormatted();
            scanFormatted.Parse(input, format);
            int count = scanFormatted.Results.Count;
            if (count > 0)
            {
                r1 = (T1)scanFormatted.Results[0];
            }

            if (count > 1)
            {
                r2 = (T2)scanFormatted.Results[1];
            }

            if (count > 2)
            {
                r3 = (T3)scanFormatted.Results[2];
            }

            return count;
        }

        public static int sscanf<T1, T2, T3, T4>(string input, string format, ref T1 r1, ref T2 r2, ref T3 r3, ref T4 r4)
        {
            ScanFormatted scanFormatted = new ScanFormatted();
            scanFormatted.Parse(input, format);
            int count = scanFormatted.Results.Count;
            if (count > 0)
            {
                r1 = (T1)scanFormatted.Results[0];
            }

            if (count > 1)
            {
                r2 = (T2)scanFormatted.Results[1];
            }

            if (count > 2)
            {
                r3 = (T3)scanFormatted.Results[2];
            }

            if (count > 3)
            {
                r4 = (T4)scanFormatted.Results[3];
            }

            return count;
        }

        public static int sscanf<T1, T2, T3, T4, T5>(string input, string format, ref T1 r1, ref T2 r2, ref T3 r3, ref T4 r4, ref T5 r5)
        {
            ScanFormatted scanFormatted = new ScanFormatted();
            scanFormatted.Parse(input, format);
            int count = scanFormatted.Results.Count;
            if (count > 0)
            {
                r1 = (T1)scanFormatted.Results[0];
            }

            if (count > 1)
            {
                r2 = (T2)scanFormatted.Results[1];
            }

            if (count > 2)
            {
                r3 = (T3)scanFormatted.Results[2];
            }

            if (count > 3)
            {
                r4 = (T4)scanFormatted.Results[3];
            }

            if (count > 4)
            {
                r5 = (T5)scanFormatted.Results[4];
            }

            return count;
        }

        public static int sscanf<T1, T2, T3, T4, T5, T6>(string input, string format, ref T1 r1, ref T2 r2, ref T3 r3, ref T4 r4, ref T5 r5, ref T6 r6)
        {
            ScanFormatted scanFormatted = new ScanFormatted();
            scanFormatted.Parse(input, format);
            int count = scanFormatted.Results.Count;
            if (count > 0)
            {
                r1 = (T1)scanFormatted.Results[0];
            }

            if (count > 1)
            {
                r2 = (T2)scanFormatted.Results[1];
            }

            if (count > 2)
            {
                r3 = (T3)scanFormatted.Results[2];
            }

            if (count > 3)
            {
                r4 = (T4)scanFormatted.Results[3];
            }

            if (count > 4)
            {
                r5 = (T5)scanFormatted.Results[4];
            }

            if (count > 5)
            {
                r6 = (T6)scanFormatted.Results[5];
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void strcat(ref string a, string b)
        {
            a += b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int strchr(string s, char c)
        {
            if (string.IsNullOrEmpty(s))
            {
                return -1;
            }

            return s.IndexOf(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int strcmp(string a, string b)
        {
            return string.Compare(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string strcpy(out string a, string b)
        {
            return a = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int strlen(string s)
        {
            return s?.Length ?? 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void strncat(ref string a, string b, int n)
        {
            n = Math.Min(n, b?.Length ?? 0);
            if (n > 0)
            {
                a += b.Substr(0, n);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int strncmp(string a, string b, int n)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                return string.Compare(a, b);
            }

            return string.Compare(a.Substring(0, Math.Min(a.Length, n)), b.Substring(0, Math.Min(b.Length, n)));
        }

        //    return array + num;
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void strncpy(out string a, string b, int n)
        {
            a = b?.Substring(0, Math.Min(n, b.Length));
        }

        //public static CPointer<TVal> bsearch<TKey, TVal>(TKey key, CPointer<TVal> array, int n, Func<TKey, TVal, int> compare)
        //{
        //    int num = new List<TVal>(array.ToArray().Take(n)).BinarySearch(default(TVal), new bcomparer<TKey, TVal>(key, compare));
        //    if (num < 0)
        //    {
        //        return default(CPointer<TVal>);
        //    }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int strstr(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                return -1;
            }

            return a.IndexOf(b);
        }

        public static object ToInteger(object Value, bool Round)
        {
            return Value.GetType().GetTypeCode() switch
            {
                TypeCode.SByte => Value,
                TypeCode.Int16 => Value,
                TypeCode.Int32 => Value,
                TypeCode.Int64 => Value,
                TypeCode.Byte => Value,
                TypeCode.UInt16 => Value,
                TypeCode.UInt32 => Value,
                TypeCode.UInt64 => Value,
                TypeCode.Single => Round ? (int)Math.Round((float)Value) : (int)(float)Value,
                TypeCode.Double => Round ? (long)Math.Round((double)Value) : (long)(double)Value,
                TypeCode.Decimal => Round ? Math.Round((decimal)Value) : (decimal)Value,
                _ => null,
            };
        }

        //public static void qsort<T>(CPointer<T> array, int n, Comparison<T> compare)
        //{
        //    List<T> list = new List<T>(array.ToArray().Take(n));
        //    list.Sort(compare);
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        array[i] = list[i];
        //    }
        //}
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static void rewind(CFile file)
        //{
        //    file?.Seek(0L, SeekOrigin.Begin);
        //}
        public static object ToUnsigned(object Value)
        {
            return Value.GetType().GetTypeCode() switch
            {
                TypeCode.SByte => (byte)(sbyte)Value,
                TypeCode.Int16 => (ushort)(short)Value,
                TypeCode.Int32 => (uint)(int)Value,
                TypeCode.Int64 => (ulong)(long)Value,
                TypeCode.Byte => Value,
                TypeCode.UInt16 => Value,
                TypeCode.UInt32 => Value,
                TypeCode.UInt64 => Value,
                TypeCode.Single => (uint)(float)Value,
                TypeCode.Double => (ulong)(double)Value,
                TypeCode.Decimal => (ulong)(decimal)Value,
                _ => null,
            };
        }

        public static long UnboxToLong(object Value, bool Round)
        {
            switch (Value.GetType().GetTypeCode())
            {
                case TypeCode.SByte:
                    return (sbyte)Value;

                case TypeCode.Int16:
                    return (short)Value;

                case TypeCode.Int32:
                    return (int)Value;

                case TypeCode.Int64:
                    return (long)Value;

                case TypeCode.Byte:
                    return (byte)Value;

                case TypeCode.UInt16:
                    return (ushort)Value;

                case TypeCode.UInt32:
                    return (uint)Value;

                case TypeCode.UInt64:
                    return (long)(ulong)Value;

                case TypeCode.Single:
                    if (!Round)
                    {
                        return (long)(float)Value;
                    }

                    return (long)Math.Round((float)Value);

                case TypeCode.Double:
                    if (!Round)
                    {
                        return (long)(double)Value;
                    }

                    return (long)Math.Round((double)Value);

                case TypeCode.Decimal:
                    if (!Round)
                    {
                        return (long)(decimal)Value;
                    }

                    return (long)Math.Round((decimal)Value);

                default:
                    return 0L;
            }
        }

        private static string FormatHex(string NativeFormat, bool Alternate, int FieldLength, int FieldPrecision, bool Left2Right, char Padding, object Value)
        {
            string text = string.Empty;
            string format = "{0" + (FieldLength != int.MinValue ? "," + (Left2Right ? "-" : string.Empty) + FieldLength : string.Empty) + "}";
            string format2 = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ? FieldPrecision.ToString() : string.Empty) + "}";
            if (IsNumericType(Value))
            {
                text = string.Format(format2, [Value]);
                if (Left2Right || Padding == ' ')
                {
                    if (Alternate)
                    {
                        text = (NativeFormat == "x" ? "0x" : "0X") + text;
                    }

                    text = string.Format(format, [text]);
                }
                else
                {
                    if (FieldLength != int.MinValue)
                    {
                        text = text.PadLeft(FieldLength - (Alternate ? 2 : 0), Padding);
                    }

                    if (Alternate)
                    {
                        text = (NativeFormat == "x" ? "0x" : "0X") + text;
                    }
                }
            }

            return text;
        }

        private static string FormatNumber(string NativeFormat, bool Alternate, int FieldLength, int FieldPrecision, bool Left2Right, bool PositiveSign, bool PositiveSpace, char Padding, object Value)
        {
            string text = string.Empty;
            string format = "{0" + (FieldLength != int.MinValue ? "," + (Left2Right ? "-" : string.Empty) + FieldLength : string.Empty) + "}";
            string format2 = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ? FieldPrecision.ToString() : "0") + "}";
            if (IsNumericType(Value))
            {
                text = string.Format(format2, [Value]);
                if (Left2Right || Padding == ' ')
                {
                    if (IsPositive(Value, ZeroIsPositive: true))
                    {
                        text = (PositiveSign ? "+" : PositiveSpace ? " " : string.Empty) + text;
                    }

                    text = string.Format(format, [text]);
                }
                else
                {
                    if (text.StartsWith("-"))
                    {
                        text = text.Substring(1);
                    }

                    if (FieldLength != int.MinValue)
                    {
                        text = text.PadLeft(FieldLength - (PositiveSign || PositiveSpace || !IsPositive(Value, ZeroIsPositive: true) ? 1 : 0), Padding);
                    }

                    if (IsPositive(Value, ZeroIsPositive: true))
                    {
                        if (PositiveSign || PositiveSpace)
                        {
                            text = (PositiveSign ? "+" : PositiveSpace ? " " : FieldLength != int.MinValue ? Padding.ToString() : string.Empty) + text;
                        }
                    }
                    else
                    {
                        text = "-" + text;
                    }
                }
            }

            return text;
        }

        private static string FormatOct(string NativeFormat, bool Alternate, int FieldLength, int FieldPrecision, bool Left2Right, char Padding, object Value)
        {
            string text = string.Empty;
            string format = "{0" + (FieldLength != int.MinValue ? "," + (Left2Right ? "-" : string.Empty) + FieldLength : string.Empty) + "}";
            if (IsNumericType(Value))
            {
                text = Convert.ToString(UnboxToLong(Value, Round: true), 8);
                if (Left2Right || Padding == ' ')
                {
                    if (Alternate && text != "0")
                    {
                        text = "0" + text;
                    }

                    text = string.Format(format, [text]);
                }
                else
                {
                    if (FieldLength != int.MinValue)
                    {
                        text = text.PadLeft(FieldLength - (Alternate && text != "0" ? 1 : 0), Padding);
                    }

                    if (Alternate && text != "0")
                    {
                        text = "0" + text;
                    }
                }
            }

            return text;
        }

        private static string ReplaceMetaCharsMatch(Match m)
        {
            if (m.Groups[2].Length == 3)
            {
                return Convert.ToChar(Convert.ToByte(m.Groups[2].Value, 8)).ToString();
            }

            return m.Groups[2].Value switch
            {
                "0" => "\0",
                "a" => "\a",
                "b" => "\b",
                "f" => "\f",
                "v" => "\v",
                "r" => "\r",
                "n" => "\n",
                "t" => "\t",
                _ => m.Groups[2].Value,
            };
        }

        private class BComparer<TKey, TVal> : IComparer<TVal>
        {
            public BComparer(TKey key, Func<TKey, TVal, int> compare)
            {
                Key = key;
                Comparer = compare;
            }

            public Func<TKey, TVal, int> Comparer { get; }
            public TKey Key { get; }

            public int Compare(TVal x, TVal y)
            {
                bool flag = x?.Equals(default(TVal)) ?? true;
                bool flag2 = y?.Equals(default(TVal)) ?? true;
                if (flag2 && !flag)
                {
                    int num = Comparer(Key, x);
                    if (num != 0)
                    {
                        return -num;
                    }

                    return num;
                }

                if (flag && !flag2)
                {
                    return Comparer(Key, y);
                }

                return -1;
            }
        }

        private class ScanFormatted
        {
            public List<object> Results;

            protected TypeParser[] _typeParsers;

            public ScanFormatted()
            {
                _typeParsers =
                [
                    new TypeParser
                    {
                        Type = Types.Character,
                        Parser = ParseCharacter
                    },
                    new TypeParser
                    {
                        Type = Types.Decimal,
                        Parser = ParseDecimal
                    },
                    new TypeParser
                    {
                        Type = Types.Float,
                        Parser = ParseFloat
                    },
                    new TypeParser
                    {
                        Type = Types.Hexadecimal,
                        Parser = ParseHexadecimal
                    },
                    new TypeParser
                    {
                        Type = Types.Octal,
                        Parser = ParseOctal
                    },
                    new TypeParser
                    {
                        Type = Types.ScanSet,
                        Parser = ParseScanSet
                    },
                    new TypeParser
                    {
                        Type = Types.String,
                        Parser = ParseString
                    },
                    new TypeParser
                    {
                        Type = Types.Unsigned,
                        Parser = ParseDecimal
                    }
                ];
                Results = [];
            }

            protected delegate bool ParseValue(TextParser input, FormatSpecifier spec);

            protected enum Modifiers
            {
                None,
                ShortShort,
                Short,
                Long,
                LongLong
            }

            protected enum Types
            {
                Character,
                Decimal,
                Float,
                Hexadecimal,
                Octal,
                ScanSet,
                String,
                Unsigned
            }

            public int Parse(string input, string format)
            {
                TextParser textParser = new TextParser(input);
                TextParser textParser2 = new TextParser(format);
                new List<object>();
                FormatSpecifier spec = new FormatSpecifier();
                int num = 0;
                Results.Clear();
                while (!textParser2.EndOfText && !textParser.EndOfText)
                {
                    if (ParseFormatSpecifier(textParser2, spec))
                    {
                        if (!_typeParsers.First((tp) => tp.Type == spec.Type).Parser(textParser, spec))
                        {
                            break;
                        }

                        num++;
                        continue;
                    }

                    if (char.IsWhiteSpace(textParser2.Peek()))
                    {
                        textParser.MovePastWhitespace();
                        textParser2.MoveAhead();
                        continue;
                    }

                    if (textParser2.Peek() != textParser.Peek())
                    {
                        break;
                    }

                    textParser.MoveAhead();
                    textParser2.MoveAhead();
                }

                return num;
            }

            protected bool ParseFormatSpecifier(TextParser format, FormatSpecifier spec)
            {
                if (format.Peek() != '%')
                {
                    return false;
                }

                format.MoveAhead();
                if (format.Peek() == '%')
                {
                    return false;
                }

                if (format.Peek() == '*')
                {
                    spec.NoResult = true;
                    format.MoveAhead();
                }
                else
                {
                    spec.NoResult = false;
                }

                int position = format.Position;
                while (char.IsDigit(format.Peek()))
                {
                    format.MoveAhead();
                }

                if (format.Position > position)
                {
                    spec.Width = int.Parse(format.Extract(position, format.Position));
                }
                else
                {
                    spec.Width = 0;
                }

                if (format.Peek() == 'h')
                {
                    format.MoveAhead();
                    if (format.Peek() == 'h')
                    {
                        format.MoveAhead();
                        spec.Modifier = Modifiers.ShortShort;
                    }
                    else
                    {
                        spec.Modifier = Modifiers.Short;
                    }
                }
                else if (char.ToLower(format.Peek()) == 'l')
                {
                    format.MoveAhead();
                    if (format.Peek() == 'l')
                    {
                        format.MoveAhead();
                        spec.Modifier = Modifiers.LongLong;
                    }
                    else
                    {
                        spec.Modifier = Modifiers.Long;
                    }
                }
                else
                {
                    spec.Modifier = Modifiers.None;
                }

                switch (format.Peek())
                {
                    case 'c':
                        spec.Type = Types.Character;
                        break;

                    case 'd':
                    case 'i':
                        spec.Type = Types.Decimal;
                        break;

                    case 'A':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'a':
                    case 'e':
                    case 'f':
                    case 'g':
                        spec.Type = Types.Float;
                        break;

                    case 'o':
                        spec.Type = Types.Octal;
                        break;

                    case 's':
                        spec.Type = Types.String;
                        break;

                    case 'u':
                        spec.Type = Types.Unsigned;
                        break;

                    case 'X':
                    case 'x':
                        spec.Type = Types.Hexadecimal;
                        break;

                    case '[':
                        spec.Type = Types.ScanSet;
                        format.MoveAhead();
                        if (format.Peek() == '^')
                        {
                            spec.ScanSetExclude = true;
                            format.MoveAhead();
                        }
                        else
                        {
                            spec.ScanSetExclude = false;
                        }

                        position = format.Position;
                        if (format.Peek() == ']')
                        {
                            format.MoveAhead();
                        }

                        format.MoveTo(']');
                        if (format.EndOfText)
                        {
                            throw new Exception("Type specifier expected character : ']'");
                        }

                        spec.ScanSet = format.Extract(position, format.Position);
                        break;

                    default:
                        throw new Exception($"Unknown format type specified : '{format.Peek()}'");
                }

                format.MoveAhead();
                return true;
            }

            protected bool ParseHexadecimal(TextParser input, FormatSpecifier spec)
            {
                input.MovePastWhitespace();
                int position = input.Position;
                if (input.Peek() == '0' && input.Peek(1) == 'x')
                {
                    input.MoveAhead(2);
                }

                while (IsValidDigit(input.Peek(), 16))
                {
                    input.MoveAhead();
                }

                if (spec.Width > 0)
                {
                    int num = input.Position - position;
                    if (spec.Width < num)
                    {
                        input.MoveAhead(spec.Width - num);
                    }
                }

                if (input.Position > position)
                {
                    if (!spec.NoResult)
                    {
                        AddUnsigned(input.Extract(position, input.Position), spec.Modifier, 16);
                    }

                    return true;
                }

                return false;
            }

            protected bool ParseScanSet(TextParser input, FormatSpecifier spec)
            {
                int position = input.Position;
                if (!spec.ScanSetExclude)
                {
                    while (StringHelper.Contains(spec.ScanSet, input.Peek()))
                    {
                        input.MoveAhead();
                    }
                }
                else
                {
                    while (!input.EndOfText && !StringHelper.Contains(spec.ScanSet, input.Peek()))
                    {
                        input.MoveAhead();
                    }
                }

                if (spec.Width > 0)
                {
                    int num = input.Position - position;
                    if (spec.Width < num)
                    {
                        input.MoveAhead(spec.Width - num);
                    }
                }

                if (input.Position > position)
                {
                    if (!spec.NoResult)
                    {
                        Results.Add(input.Extract(position, input.Position));
                        input.MoveAhead();
                    }

                    return true;
                }

                return false;
            }

            private void AddSigned(string token, Modifiers mod, int radix)
            {
                object item;
                switch (mod)
                {
                    case Modifiers.ShortShort:
                        item = Convert.ToSByte(token, radix);
                        break;

                    case Modifiers.Short:
                        item = Convert.ToInt16(token, radix);
                        break;

                    case Modifiers.Long:
                    case Modifiers.LongLong:
                        item = Convert.ToInt64(token, radix);
                        break;

                    default:
                        item = Convert.ToInt32(token, radix);
                        break;
                }

                Results.Add(item);
            }

            private void AddUnsigned(string token, Modifiers mod, int radix)
            {
                object item;
                switch (mod)
                {
                    case Modifiers.ShortShort:
                        item = Convert.ToByte(token, radix);
                        break;

                    case Modifiers.Short:
                        item = Convert.ToUInt16(token, radix);
                        break;

                    case Modifiers.Long:
                    case Modifiers.LongLong:
                        item = Convert.ToUInt64(token, radix);
                        break;

                    default:
                        item = Convert.ToUInt32(token, radix);
                        break;
                }

                Results.Add(item);
            }

            private bool IsValidDigit(char c, int radix)
            {
                int num = "0123456789abcdef".IndexOf(char.ToLower(c));
                if (num >= 0 && num < radix)
                {
                    return true;
                }

                return false;
            }

            private bool ParseCharacter(TextParser input, FormatSpecifier spec)
            {
                int position = input.Position;
                int num = spec.Width <= 1 ? 1 : spec.Width;
                while (!input.EndOfText && num-- > 0)
                {
                    input.MoveAhead();
                }

                if (num <= 0 && input.Position > position)
                {
                    if (!spec.NoResult)
                    {
                        string text = input.Extract(position, input.Position);
                        if (text.Length > 1)
                        {
                            Results.Add(text.ToCharArray());
                        }
                        else
                        {
                            Results.Add(text[0]);
                        }
                    }

                    return true;
                }

                return false;
            }

            private bool ParseDecimal(TextParser input, FormatSpecifier spec)
            {
                int radix = 10;
                input.MovePastWhitespace();
                int position = input.Position;
                if (input.Peek() == '+' || input.Peek() == '-')
                {
                    input.MoveAhead();
                }

                while (IsValidDigit(input.Peek(), radix))
                {
                    input.MoveAhead();
                }

                if (spec.Width > 0)
                {
                    int num = input.Position - position;
                    if (spec.Width < num)
                    {
                        input.MoveAhead(spec.Width - num);
                    }
                }

                if (input.Position > position)
                {
                    if (!spec.NoResult)
                    {
                        if (spec.Type == Types.Decimal)
                        {
                            AddSigned(input.Extract(position, input.Position), spec.Modifier, radix);
                        }
                        else
                        {
                            AddUnsigned(input.Extract(position, input.Position), spec.Modifier, radix);
                        }
                    }

                    return true;
                }

                return false;
            }

            private bool ParseFloat(TextParser input, FormatSpecifier spec)
            {
                input.MovePastWhitespace();
                int position = input.Position;
                if (input.Peek() == '+' || input.Peek() == '-')
                {
                    input.MoveAhead();
                }

                bool flag = false;
                while (char.IsDigit(input.Peek()) || input.Peek() == '.')
                {
                    if (input.Peek() == '.')
                    {
                        if (flag)
                        {
                            break;
                        }

                        flag = true;
                    }

                    input.MoveAhead();
                }

                if (char.ToLower(input.Peek()) == 'e')
                {
                    input.MoveAhead();
                    if (input.Peek() == '+' || input.Peek() == '-')
                    {
                        input.MoveAhead();
                    }

                    while (char.IsDigit(input.Peek()))
                    {
                        input.MoveAhead();
                    }
                }

                if (spec.Width > 0)
                {
                    int num = input.Position - position;
                    if (spec.Width < num)
                    {
                        input.MoveAhead(spec.Width - num);
                    }
                }

                if (input.Position > position && double.TryParse(input.Extract(position, input.Position), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    if (!spec.NoResult)
                    {
                        if (spec.Modifier is Modifiers.Long or Modifiers.LongLong)
                        {
                            Results.Add(result);
                        }
                        else
                        {
                            Results.Add((float)result);
                        }
                    }

                    return true;
                }

                return false;
            }

            private bool ParseOctal(TextParser input, FormatSpecifier spec)
            {
                input.MovePastWhitespace();
                int position = input.Position;
                while (IsValidDigit(input.Peek(), 8))
                {
                    input.MoveAhead();
                }

                if (spec.Width > 0)
                {
                    int num = input.Position - position;
                    if (spec.Width < num)
                    {
                        input.MoveAhead(spec.Width - num);
                    }
                }

                if (input.Position > position)
                {
                    if (!spec.NoResult)
                    {
                        AddUnsigned(input.Extract(position, input.Position), spec.Modifier, 8);
                    }

                    return true;
                }

                return false;
            }

            private bool ParseString(TextParser input, FormatSpecifier spec)
            {
                input.MovePastWhitespace();
                int position = input.Position;
                while (!input.EndOfText && !char.IsWhiteSpace(input.Peek()))
                {
                    input.MoveAhead();
                }

                if (spec.Width > 0)
                {
                    int num = input.Position - position;
                    if (spec.Width < num)
                    {
                        input.MoveAhead(spec.Width - num);
                    }
                }

                if (input.Position > position)
                {
                    if (!spec.NoResult)
                    {
                        Results.Add(input.Extract(position, input.Position));
                    }

                    return true;
                }

                return false;
            }

            protected class FormatSpecifier
            {
                public Modifiers Modifier { get; set; }
                public bool NoResult { get; set; }
                public string ScanSet { get; set; }
                public bool ScanSetExclude { get; set; }
                public Types Type { get; set; }
                public int Width { get; set; }
            }

            protected class TypeParser
            {
                public ParseValue Parser { get; set; }
                public Types Type { get; set; }
            }
        }

        private class TextParser
        {
            public static char NullChar;
            private int _pos;
            private string _text;

            public TextParser(string text)
            {
                Reset(text);
            }

            public bool EndOfText => _pos >= _text.Length;
            public int Position => _pos;

            public string Extract(int start, int end)
            {
                return _text.Substring(start, end - start);
            }

            public void MoveAhead()
            {
                MoveAhead(1);
            }

            public void MoveAhead(int ahead)
            {
                _pos = Math.Min(_pos + ahead, _text.Length);
            }

            public void MovePastWhitespace()
            {
                while (char.IsWhiteSpace(Peek()))
                {
                    MoveAhead();
                }
            }

            public void MoveTo(char c)
            {
                _pos = _text.IndexOf(c, _pos);
                if (_pos < 0)
                {
                    _pos = _text.Length;
                }
            }

            public char Peek()
            {
                return Peek(0);
            }

            public char Peek(int ahead)
            {
                int num = _pos + ahead;
                if (num < _text.Length)
                {
                    return _text[num];
                }

                return NullChar;
            }

            public void Reset(string text)
            {
                _text = text ?? string.Empty;
                _pos = 0;
            }
        }
    }
}