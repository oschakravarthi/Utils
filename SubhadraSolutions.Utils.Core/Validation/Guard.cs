using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SubhadraSolutions.Utils.Validation;

public static class Guard
{
    public const string IndexerName = "Item[]";
    private const string STRING_MUST_NOT_BE_EMPTY_EXCEPTION_MESSAGE = "Argument must not be empty";
    private const string StringMustNotBeEmptyExceptionMessage = "Argument must not be empty";

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentAlphaNumericRequired(string argumentValue, string argumentName)
    {
        if (!string.IsNullOrEmpty(argumentValue) && !argumentValue.All(char.IsLetterOrDigit))
        {
            throw new ArgumentException("Argument must not contain any special character", argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentPropertyShouldNotBeNull<T>(T argumentValue, string argumentPath,
        string detailedMessage)
        where T : class
    {
        if (argumentValue == null)
        {
            throw new ArgumentException(argumentPath, detailedMessage);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentPropertyShouldNotBeNull<T>(T argumentValue, string argumentPath)
        where T : class
    {
        if (argumentValue == null)
        {
            var argumentName = argumentPath.GetPathRoot();
            var defaultMessage = $"argument \"{argumentName}\" is invalid because \"{argumentPath}\" is null";

            throw new ArgumentException(argumentPath, defaultMessage);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentPropertyShouldNotBeNullOrEmpty(string argumentValue, string argumentPath,
        string detailedMessage)
    {
        if (string.IsNullOrEmpty(argumentValue))
        {
            throw new ArgumentException(argumentPath, detailedMessage);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentPropertyShouldNotBeNullOrEmpty(string argumentValue, string argumentPath)
    {
        if (string.IsNullOrEmpty(argumentValue))
        {
            var argumentName = argumentPath.GetPathRoot();
            var defaultMessage =
                $"argument \"{argumentName}\" is invalid because \"{argumentPath}\" is null or empty";

            throw new ArgumentException(argumentPath, defaultMessage);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeDateOnly(ref DateTime argumentValue, string argumentName)
    {
        if (argumentValue != argumentValue.Date)
        {
            throw new ArgumentException("value must be date-only (no time part is allowed)", argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeEqualTo(int value, int argumentValue, string argumentName)
    {
        if (argumentValue != value)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeGreaterThan(int min, int argumentValue, string argumentName)
    {
        if (argumentValue <= min)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeGreaterThan(DateTime min, DateTime argumentValue, string argumentName)
    {
        if (argumentValue <= min)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeGreaterThan(double min, double argumentValue, string argumentName)
    {
        if (argumentValue <= min)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeGreaterThanOrEqualTo(int value, int argumentValue, string argumentName)
    {
        if (argumentValue < value)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeGreaterThanOrEqualTo(DateTime value, DateTime argumentValue,
        string argumentName)
    {
        if (argumentValue < value)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeGreaterThanOrEqualTo(double value, double argumentValue, string argumentName)
    {
        if (argumentValue < value)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeInRange(int min, int max, int argumentValue, string argumentName)
    {
        if (argumentValue < min || argumentValue > max)
        {
            var errorMessage = $"argument is not in the range [{min}, {max}]";
            throw new ArgumentOutOfRangeException(argumentName, argumentValue, errorMessage);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeInRange(double min, double max, double argumentValue, string argumentName)
    {
        if (argumentValue < min || argumentValue > max)
        {
            var errorMessage = $"argument is not in the range [{min}, {max}]";
            throw new ArgumentOutOfRangeException(argumentName, argumentValue, errorMessage);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldBeLessThanOrEqualTo(double value, double argumentValue, string argumentName)
    {
        if (argumentValue > value)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeEmpty<T>(
        IEnumerable<T> argumentValue,
        string argumentName)
    {
        if (argumentValue == null)
        {
            return;
        }

        if (!argumentValue.Any())
        {
            throw new ArgumentException("Enumeration must not be empty", argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeEmpty<T>(
        T[] argumentValue,
        string argumentName)
    {
        if (argumentValue == null)
        {
            return;
        }

        if (argumentValue.Length == 0)
        {
            throw new ArgumentException("Enumeration must not be empty", argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeEmpty(ref Guid key, string argumentName)
    {
        if (key == Guid.Empty)
        {
            throw new ArgumentException("Argument must not be empty", argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeNull(object argumentValue, string parameterName)
    {
        if (argumentValue == null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void EnsureKeyExistInDictionary<TKey, TValue>(IDictionary<TKey, TValue> parameter, TKey key)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (!parameter.ContainsKey(key))
        {
            throw new ArgumentException($"Key with the name '{key}' do not exist");
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeNull(object argumentValue, string parameterName, string detailedMessage)
    {
        if (argumentValue == null)
        {
            throw new ArgumentNullException(parameterName, detailedMessage);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeNullOrEmpty(string argumentValue, string parameterName)
    {
        ArgumentShouldNotBeNullOrEmpty(argumentValue, parameterName, STRING_MUST_NOT_BE_EMPTY_EXCEPTION_MESSAGE);
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeNullOrEmpty(string argumentValue, string parameterName,
        string detailedMessage)
    {
        if (argumentValue == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (argumentValue.Length == 0)
        {
            throw new ArgumentException(detailedMessage, parameterName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeNullOrEmpty<T>(
        IEnumerable<T> argumentValue,
        string argumentName)
    {
        ArgumentShouldNotBeNull(argumentValue, argumentName);
        if (!argumentValue.Any())
        {
            throw new ArgumentException("Enumeration must not be empty", argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(string argumentValue, string parameterName)
    {
        ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(argumentValue, parameterName,
            STRING_MUST_NOT_BE_EMPTY_EXCEPTION_MESSAGE);
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(string argumentValue, string parameterName,
        string detailedMessage)
    {
        ArgumentShouldNotBeNull(argumentValue, parameterName, detailedMessage);
        for (var i = 0; i < argumentValue.Length; i++)
            if (!char.IsWhiteSpace(argumentValue[i]))
            {
                return;
            }

        throw new ArgumentException(detailedMessage, parameterName);
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void TypeShouldBeAssignable(Type assignmentTargetType, Type assignmentValueType,
        string argumentName)
    {
        TypeShouldBeAssignable(assignmentTargetType, assignmentValueType, argumentName,
            $"Types are not assignable: {assignmentTargetType} from {assignmentValueType}");
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void TypeShouldBeAssignable(Type assignmentTargetType, Type assignmentValueType,
        string argumentName, string detailedMessage)
    {
        if (assignmentTargetType == null)
        {
            throw new ArgumentNullException(nameof(assignmentTargetType));
        }

        if (assignmentValueType == null)
        {
            throw new ArgumentNullException(nameof(assignmentValueType));
        }

        if (!assignmentTargetType.IsAssignableFrom(assignmentValueType))
        {
            throw new ArgumentException(detailedMessage, argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static void TypeShouldEqualTo(Type assignmentTargetType, Type assignmentValueType, string argumentName,
        string detailedMessage)
    {
        if (assignmentTargetType == null)
        {
            throw new ArgumentNullException(nameof(assignmentTargetType));
        }

        if (assignmentValueType == null)
        {
            throw new ArgumentNullException(nameof(argumentName));
        }

        if (assignmentTargetType != assignmentValueType)
        {
            throw new ArgumentException(detailedMessage, argumentName);
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    private static string GetPathRoot(this string source)
    {
        ArgumentShouldNotBeNull(source, "source");

        var firstSeparator = source.IndexOf('.');
        if (firstSeparator > -1)
        {
            return source.Substring(0, firstSeparator);
        }

        return source;
    }
}