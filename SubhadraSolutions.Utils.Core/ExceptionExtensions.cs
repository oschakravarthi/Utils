using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

//using SubhadraSolutions.Utils.Diagnostics.Tracing;

namespace SubhadraSolutions.Utils;

public static class ExceptionExtensions
{
    public static readonly MethodInfo ToDetailedStringMethod =
        typeof(ExceptionExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == nameof(ToDetailedString) && m.GetParameters().Length == 1);

    private static readonly int FACILITY_WIN32 = 7;

    public static Exception GetInnermostException(this Exception chainedException)
    {
        var ex = chainedException;
        while (ex.InnerException != null) ex = ex.InnerException;
        return ex;
    }

    public static int GetMeaningfulHResult(this Exception exception)
    {
        int num;
        //TraceLogger.TraceInformation("{0}: Entry", text);
        //TraceLogger.TraceInformation("\t{0}: Looking for a meaningful HResult from the following exception:\n{1}", text, exception);
        int? num2 = null;
        var empty = string.Empty;
        num = exception.HResult;
        empty = exception.GetType().Name + ".HResult";
        var empty2 = string.Empty;
        if (num2.HasValue)
        {
            num = num2.Value.HResultFromWin32();
            empty2 = string.Format("{0} (0x{1,08:x} / {1})", num2.Value, num);
        }
        else
        {
            empty2 = string.Format("0x{0,08:x} ({0})", num);
        }

        //TraceLogger.TraceInformation("\t{0}: Using {1}: {2} [{3}]", text, empty, empty2, num2.HasValue ? "Win32 error" : "HRESULT");
        //TraceLogger.TraceInformation("{0}: Exit: Returning 0x{1,08:x} ({1})", text, num);
        return num;
    }

    public static int HResultFromWin32(this int x)
    {
        if (x > 0)
        {
            x = ((x & 0xFFFF) | (FACILITY_WIN32 << 16)) + int.MinValue;
        }

        return x;
    }

    [DynamicallyInvoked]
    public static string ToDetailedString(this Exception exception)
    {
        return exception.ToDetailedString(ExceptionOptions.Default);
    }

    public static string ToDetailedString(this Exception exception, ExceptionOptions options)
    {
        if (exception == null)
        {
            return null;
        }

        try
        {
            var stringBuilder = new StringBuilder();

            AppendValue(stringBuilder, "Type", exception.GetType().FullName, options);

            foreach (var property in exception
                         .GetType()
                         .GetProperties()
                         .OrderByDescending(x =>
                             string.Equals(x.Name, nameof(exception.Message), StringComparison.Ordinal))
                         .ThenByDescending(x =>
                             string.Equals(x.Name, nameof(exception.Source), StringComparison.Ordinal))
                         .ThenBy(x =>
                             string.Equals(x.Name, nameof(exception.InnerException), StringComparison.Ordinal))
                         .ThenBy(x => string.Equals(x.Name, nameof(AggregateException.InnerExceptions),
                             StringComparison.Ordinal)))
            {
                var value = property.GetValue(exception, null);
                if (value == null && options.OmitNullProperties)
                {
                    if (options.OmitNullProperties)
                    {
                        continue;
                    }

                    value = string.Empty;
                }

                AppendValue(stringBuilder, property.Name, value, options);
            }

            return stringBuilder.ToString().TrimEnd('\r', '\n');
        }
        catch
        {
            return exception.ToString();
        }
    }

    private static void AppendCollection(StringBuilder stringBuilder, string propertyName, IEnumerable collection,
        ExceptionOptions options)
    {
        stringBuilder.AppendLine($"{options.Indent}{propertyName} =");

        var innerOptions = new ExceptionOptions(options, options.CurrentIndentLevel + 1);

        var i = 0;
        foreach (var item in collection)
        {
            var innerPropertyName = $"[{i}]";

            if (item is Exception innerException)
            {
                AppendException(stringBuilder, innerPropertyName, innerException, innerOptions);
            }
            else
            {
                AppendValue(stringBuilder, innerPropertyName, item, innerOptions);
            }

            ++i;
        }
    }

    private static void AppendException(StringBuilder stringBuilder, string propertyName, Exception exception,
        ExceptionOptions options)
    {
        var innerExceptionString =
            exception.ToDetailedString(new ExceptionOptions(options, options.CurrentIndentLevel + 1));
        stringBuilder.AppendLine($"{options.Indent}{propertyName} =");
        stringBuilder.AppendLine(innerExceptionString);
    }

    private static void AppendValue(StringBuilder stringBuilder, string propertyName, object value,
        ExceptionOptions options)
    {
        if (value == null)
        {
            stringBuilder.AppendLine($"{options.Indent}{propertyName} = {null}");
            return;
        }

        if (value is DictionaryEntry dictionaryEntry)
        {
            stringBuilder.AppendLine(
                $"{options.Indent}{propertyName} = {dictionaryEntry.Key} : {dictionaryEntry.Value}");
            return;
        }

        if (value is Exception innerException)
        {
            AppendException(stringBuilder, propertyName, innerException, options);
            return;
        }

        if (value is IEnumerable collection && !(collection is string))
        {
            if (collection.GetEnumerator().MoveNext())
            {
                AppendCollection(stringBuilder, propertyName, collection, options);
            }

            return;
        }

        stringBuilder.AppendLine($"{options.Indent}{propertyName} = {value}");
    }

    public struct ExceptionOptions
    {
        public static readonly ExceptionOptions Default = new()
        {
            CurrentIndentLevel = 0,
            IndentSpaces = 4,
            OmitNullProperties = true
        };

        internal ExceptionOptions(ExceptionOptions options, int currentIndent)
        {
            CurrentIndentLevel = currentIndent;
            IndentSpaces = options.IndentSpaces;
            OmitNullProperties = options.OmitNullProperties;
        }

        public int IndentSpaces { get; set; }
        public bool OmitNullProperties { get; set; }
        internal int CurrentIndentLevel { get; set; }
        internal string Indent => new(' ', IndentSpaces * CurrentIndentLevel);
    }

    //private static string IndentString(string value, ExceptionOptions options)
    //{
    //    return value.Replace(Environment.NewLine, Environment.NewLine + options.Indent);
    //}
}