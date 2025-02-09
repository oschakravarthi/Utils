using System.Text.RegularExpressions;

namespace SubhadraSolutions.Utils.Text.RegularExpressions;

public static class RegexHelper
{
    public const string EnglishAlphaNumericRegex = "^[a-zA-Z0-9]*$";

    public static Regex BuildRegexFromWildcard(string wildcard, bool isCaseSensitive)
    {
        var wildcardRegex = WildcardToRegexString(wildcard);
        if (isCaseSensitive)
        {
            return new Regex(wildcardRegex);
        }

        return new Regex(wildcardRegex, RegexOptions.IgnoreCase);
    }

    public static bool IsWildcardMatched(string s, string wildcard, bool isCaseSensitive)
    {
        var regex = BuildRegexFromWildcard(wildcard, isCaseSensitive);
        return regex.IsMatch(s);
    }

    private static string WildcardToRegexString(string pattern)
    {
        var escapedSingle = Regex.Escape(new string('?', 1));
        var escapedMultiple = Regex.Escape(new string('*', 1));

        pattern = Regex.Escape(pattern);
        pattern = pattern.Replace(escapedSingle, ".");
        pattern = "^" + pattern.Replace(escapedMultiple, ".*") + "$";

        return pattern;
    }
}