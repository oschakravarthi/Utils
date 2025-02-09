using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace SubhadraSolutions.Utils;

public static class GeneralHelper
{
    public static readonly MethodInfo CompareMethod =
        typeof(GeneralHelper).GetMethod(nameof(Compare), BindingFlags.Static | BindingFlags.Public);

    public static readonly MethodInfo GeneralToStringMethod =
        typeof(GeneralHelper).GetMethod(nameof(ToString), [typeof(string)]);

    private static long _identity;

    [ThreadStatic] private static long _threadLocalIdentity;

    public static DateTime CurrentDateTime => GlobalSettings.Instance.UseUtcTime ? DateTime.UtcNow : DateTime.Now;
    public static long Identity => Interlocked.Increment(ref _identity);

    public static long ThreadLocalIdentity => Interlocked.Increment(ref _threadLocalIdentity);

    public static Dictionary<string, string> BuildDictionaryFromNameValueCollection(NameValueCollection collection)
    {
        var dictionary = new Dictionary<string, string>();

        for (var i = 0; i < collection.Count; i++)
        {
            var key = collection.GetKey(i);
            var value = collection[i];
            dictionary.Add(key, value);
        }

        return dictionary;
    }

    [DynamicallyInvoked]
    public static int Compare(object x, object y)
    {
        if (x == y)
        {
            return 0;
        }

        if (x != null && y != null)
        {
            if (x is IComparable comparable)
            {
                return comparable.CompareTo(y);
            }

            if (x.Equals(y))
            {
                return 0;
            }

            return string.CompareOrdinal(x.ToString(), y.ToString());
        }

        return x == null ? -1 : 1;
    }

    public static void CopyArray(List<int> source, List<int> target, int valueToAdd)
    {
        if (valueToAdd != 0)
        {
            for (var i = 0; i < source.Count; i++)
                target.Add(source[i] + valueToAdd);
        }
        else
        {
            target.AddRange(source);
        }
    }

    // public static void AbortThread(Thread t)
    // {
    //    try
    //    {
    //        t.Abort();
    //    }
    //    catch (ThreadAbortException ex)
    //    {
    //        Debug.WriteLine(ex.ToDetailedString());
    //    }
    // }
    public static void DisposeIfDisposable(object obj)
    {
        if (obj == null)
        {
            return;
        }

        if (obj is IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    public static void DisposeIfDisposables(params object[] disposables)
    {
        if (disposables != null)
        {
            foreach (var disposable in disposables)
            {
                DisposeIfDisposable(disposable);
            }
        }
    }

    public static bool IsNetCore()
    {
        var s = RuntimeInformation.FrameworkDescription;
        return !s.Contains("Framework");
    }

    public static void SafeInvoke(Delegate del, params object[] arguments)
    {
        if (del == null)
        {
            return;
        }

        var invocationList = del.GetInvocationList();

        if (invocationList.Length == 1)
        {
            try
            {
                del.DynamicInvoke(arguments);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToDetailedString());
            }
        }
        else
        {
            foreach (var innerDelegate in invocationList)
            {
                SafeInvoke(innerDelegate, arguments);
            }
        }
    }

    // public static Dictionary<string, string> GetApplicationSettings()
    // {
    //    var settings = new Dictionary<string, string>();
    //    for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
    //    {
    //        settings.Add(ConfigurationManager.AppSettings.GetKey(i), ConfigurationManager.AppSettings[i]);
    //    }

    //    return settings;
    // }
    [DynamicallyInvoked]
    public static string ToString(object obj)
    {
        return obj?.ToString();
    }
}