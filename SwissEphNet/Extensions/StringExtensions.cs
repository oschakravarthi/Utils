﻿using System;
using System.Linq;

namespace SwissEphNet
{

    /// <summary>
    /// String extensions methods
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// String.Contains() for Char
        /// </summary>
        public static bool Contains(this String s, Char c)
        {
            if (String.IsNullOrEmpty(s)) return false;
            return s.Contains(c.ToString());
        }

        /// <summary>
        /// String.Contains() for Char
        /// </summary>
        public static bool Contains(this String s, Char[] charSet)
        {
            if (charSet == null || String.IsNullOrWhiteSpace(s)) return false;
            foreach (var c in charSet)
            {
                if (s.Contains(c)) return true;
            }
            return false;
        }

        /// <summary>
        /// Search index of first char that is not in chars
        /// </summary>
        public static int IndexOfFirstNot(this String s, params char[] chars)
        {
            if (String.IsNullOrEmpty(s) || chars == null || chars.Length == 0) return -1;
            for (int i = 0; i < s.Length; i++)
            {
                if (!chars.Contains(s[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// Substring with check limits
        /// </summary>
        public static string Substr(this string s, int startIndex)
            => s == null ? null
            : startIndex >= s.Length ? string.Empty
            : s.Substring(Math.Max(0, startIndex))
            ;

        /// <summary>
        /// Substring with check limits
        /// </summary>
        public static string Substr(this string s, int startIndex, int length)
            => s == null ? null
            : startIndex >= s.Length ? string.Empty
            : s.Substring(Math.Max(0, startIndex), Math.Max(0, Math.Min(length, s.Length - startIndex)))
            ;
    }

}
