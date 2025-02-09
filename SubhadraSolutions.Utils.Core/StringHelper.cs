using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SubhadraSolutions.Utils;

public static class StringHelper
{
    public static readonly MethodInfo ObjectToStringMethod =
        typeof(StringHelper).GetMethod(nameof(GetObjectAsString), BindingFlags.Static | BindingFlags.Public);

    public static string Escape(this string input)
    {
        if (input == null)
        {
            return null;
        }
        var length = input.Length;
        var result = new char[length * 2];
        int j = 0;
        for (int i = 0; i < length; i++, j++)
        {
            var c = input[i];
            if (c == '"')
            {
                result[j] = '\\';
                j++;
            }
            result[j] = c;
        }
        if (j == length)
        {
            return input;
        }
        return new string(result, 0, j);
    }

    public static string BuildDisplayName(this string s)
    {
        if (s == null)
        {
            return null;
        }

        var chars = new char[s.Length * 2];
        var cIndex = 0;
        var mode = 0; //1=upper, -1 =lower, 0=other

        var previousUpperCount = 0;

        for (var sIndex = 0; sIndex < s.Length; sIndex++)
        {
            var sc = s[sIndex];
            if (sc == '_' || char.IsWhiteSpace(sc))
            {
                if (cIndex != 0)
                {
                    chars[cIndex++] = ' ';
                    previousUpperCount = 0;
                }

                mode = 0;
                continue;
            }

            if (char.IsUpper(sc))
            {
                if (mode != 1 && cIndex > 0)
                {
                    chars[cIndex++] = ' ';
                }

                chars[cIndex++] = sc;
                previousUpperCount++;
                mode = 1;
            }
            else
            {
                if (!char.IsLetter(sc))
                {
                    if (mode != 0)
                    {
                        chars[cIndex++] = ' ';
                    }

                    chars[cIndex++] = sc;
                    previousUpperCount = 0;
                    mode = 0;
                }
                else
                {
                    if (mode == -1)
                    {
                        chars[cIndex++] = sc;
                    }
                    else
                    {
                        if (mode == 1)
                        {
                            if (previousUpperCount == 1)
                            {
                                chars[cIndex++] = sc;
                            }
                            else
                            {
                                chars[cIndex] = chars[cIndex - 1];
                                chars[cIndex - 1] = ' ';
                                chars[cIndex + 1] = sc;
                                cIndex += 2;
                            }
                        }
                        else
                        {
                            chars[cIndex++] = ' ';
                            chars[cIndex++] = sc;
                        }
                    }

                    previousUpperCount = 0;
                    mode = -1;
                }
            }
        }

        if (s.Length == cIndex)
        {
            return s;
        }

        var s1 = new string(chars, 0, cIndex);
        return s1;
    }

    public static string BuildUniqueString()
    {
        return '_' + GeneralHelper.Identity.ToString();
    }

    public static string ConcatinateStringsWithCaps(this IList<string> strings, char leftCap = '[',
        char rightCap = ']')
    {
        if (strings == null)
        {
            return null;
        }

        var count = strings.Count;
        if (count == 0)
        {
            return null;
        }

        if (count == 1)
        {
            return strings[0];
        }

        var sb = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            sb.Append(leftCap);
            sb.Append(strings[i]);
            sb.Append(rightCap);
        }

        return sb.ToString();
    }

    public static string GetEmptyIfNull(this string str)
    {
        return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? string.Empty : str;
    }

    [DynamicallyInvoked]
    public static string GetObjectAsString(object o)
    {
        return o?.ToString();
    }

    public static string ReplaceMultipleChars(this string s, char replaceWith, params char[] chars)
    {
        if (chars == null || chars.Length == 0)
        {
            return s;
        }

        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        var array = new char[s.Length];
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            for (var j = 0; j < chars.Length; j++)
                if (chars[j] == c)
                {
                    c = replaceWith;
                    break;
                }

            array[i] = c;
        }

        return new string(array);
    }

    //private const string _IllegalChars = "/\\:*?\"<>|#%";

    public static bool Contains(this string s, char c)
    {
        if (string.IsNullOrEmpty(s))
        {
            return false;
        }

        return s.Contains(c.ToString());
    }

    public static bool Contains(this string s, char[] charSet)
    {
        if (charSet == null || string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        foreach (var c in charSet)
        {
            if (s.Contains(c))
            {
                return true;
            }
        }

        return false;
    }

    public static int IndexOfFirstNot(this string s, params char[] chars)
    {
        if (string.IsNullOrEmpty(s) || chars == null || chars.Length == 0)
        {
            return -1;
        }

        for (var i = 0; i < s.Length; i++)
            if (!chars.Contains(s[i]))
            {
                return i;
            }

        return -1;
    }

    public static string Substr(this string s, int startIndex)
    {
        if (s != null)
        {
            if (startIndex < s.Length)
            {
                return s.Substring(Math.Max(0, startIndex));
            }

            return string.Empty;
        }

        return null;
    }

    public static string Substr(this string s, int startIndex, int length)
    {
        if (s != null)
        {
            if (startIndex < s.Length)
            {
                return s.Substring(Math.Max(0, startIndex), Math.Max(0, Math.Min(length, s.Length - startIndex)));
            }

            return string.Empty;
        }

        return null;
    }

    public static string ToKebabCase(this string source)
    {
        if (source is null)
        {
            return null;
        }

        if (source.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < source.Length; i++)
            if (char.IsLower(source[i])) // if current char is already lowercase
            {
                builder.Append(source[i]);
            }
            else if (i == 0) // if current char is the first char
            {
                builder.Append(char.ToLowerInvariant(source[i]));
            }
            else if (char.IsLower(source[i - 1]) &&
                     char.IsLetter(source[i])) // if current char is upper and previous char is lower
            {
                builder.Append('-');
                builder.Append(char.ToLowerInvariant(source[i]));
            }
            else if
                (i + 1 == source.Length ||
                 char.IsUpper(source[i + 1])) // if current char is upper and next char doesn't exist or is upper
            {
                builder.Append(char.ToLowerInvariant(source[i]));
            }
            else if (char.IsUpper(source[i]) && char.IsLetter(source[i + 1]) &&
                     char.IsLower(source[i + 1])) // if current char is upper and next char is lower
            {
                builder.Append('-');
                builder.Append(char.ToLowerInvariant(source[i]));
            }
            else
            {
                builder.Append(char.ToLowerInvariant(source[i]));
            }

        return builder.ToString();
    }

    //
    //public static string ToPascalCase(this string source)
    //{
    //    if (source is null)
    //        return null;
    //    if (source.Length == 0)
    //        return string.Empty;
    //    var tokens = Regex.Split(source, @"[-_ ]");
    //    return string.Join(string.Empty, tokens.Select(x => x.Capitalize()));
    //}

    //public static string ToCompressedEncodedUrl(this string code)
    //{
    //    string urlEncodedBase64compressedCode;
    //    byte[] bytes;

    //    using (var uncompressed = new MemoryStream(Encoding.UTF8.GetBytes(code)))
    //    using (var compressed = new MemoryStream())
    //    using (var compressor = new DeflateStream(compressed, CompressionMode.Compress))
    //    {
    //        uncompressed.CopyTo(compressor);
    //        compressor.Close();
    //        bytes = compressed.ToArray();
    //        urlEncodedBase64compressedCode = WebEncoders.Base64UrlEncode(bytes);

    //        return urlEncodedBase64compressedCode;
    //    }
    //}

    public static bool Has(this string source, bool ignoreCase, string word)
    {
        var splits = source.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        var found = false;
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        for (var i = 0; i < splits.Length; i++)
            if (string.Equals(word, splits[i], comparison))
            {
                found = true;
                break;
            }

        return found;
    }

    public static bool HasAllIgnoreCase(this string source, params string[] words)
    {
        var splits = source.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        var comparison = StringComparison.OrdinalIgnoreCase;
        for (var i = 0; i < words.Length; i++)
        {
            var found = false;
            for (var j = 0; j < splits.Length; j++)
                if (string.Equals(words[i], splits[j], comparison))
                {
                    found = true;
                    break;
                }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    public static bool HasAnyIgnoreCase(this string source, params string[] words)
    {
        var splits = source.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        var comparison = StringComparison.OrdinalIgnoreCase;
        for (var i = 0; i < splits.Length; i++)
            for (var j = 0; j < words.Length; j++)
                if (string.Equals(splits[i], words[j], comparison))
                {
                    return true;
                }

        return false;
    }

    public static bool HasPrefix(this string source, bool ignoreCase, string prefix)
    {
        var words = source.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        for (var i = 0; i < words.Length; i++)
            if (words[i].StartsWith(prefix, comparison))
            {
                return true;
            }

        return false;
    }

    public static bool HasSuffix(this string source, bool ignoreCase, string suffix)
    {
        var words = source.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        for (var i = 0; i < words.Length; i++)
            if (words[i].EndsWith(suffix, comparison))
            {
                return true;
            }

        return false;
    }

    public static bool In(this string source, bool ignoreCase, params string[] strings)
    {
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        for (var i = 0; i < strings.Length; i++)
            if (string.Equals(source, strings[i], comparison))
            {
                return true;
            }

        return false;
    }

    public static bool MatchesRegex(this string source, string regex)
    {
        var reg = new Regex(regex);
        return reg.IsMatch(source);
    }

    public static string Trim(this string source, string regex)
    {
        regex = $"^{regex}$";
        return Regex.Replace(source, regex, string.Empty);
    }

    public static string TrimEnd(this string source, string regex)
    {
        regex = $"{regex}$";
        return Regex.Replace(source, regex, string.Empty);
    }

    public static string TrimStart(this string source, string regex)
    {
        regex = $"^{regex}";
        return Regex.Replace(source, regex, string.Empty);
    }

    public static string ToCamelCase(string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            var sCopy = string.Copy(s);
            MutableCamelCase(sCopy);
            return sCopy;
        }

        return s;
    }

    public static string ToPascalCase(string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            var sCopy = string.Copy(s);
            MutablePascalCase(sCopy);
            return sCopy;
        }

        return s;
    }

    public static void MutableSetChar(this string s, int at, char c)
    {
        var length = s.Length;
        var handle = GCHandle.Alloc(s, GCHandleType.Pinned);
        var iPtr = handle.AddrOfPinnedObject();
        unsafe
        {
            var p = (char*)iPtr.ToPointer();
            *(p + at) = c;
        }

        handle.Free();
    }

    public static void MutableCamelCase(this string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            MutableSetChar(s, 0, char.ToLower(s[0]));
        }
    }

    public static void MutablePascalCase(this string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            MutableSetChar(s, 0, char.ToUpper(s[0]));
        }
    }
}