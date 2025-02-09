using System;
using System.Diagnostics;
using System.Reflection;

namespace SubhadraSolutions.Utils;

public static class EventHelper
{
    public static readonly MethodInfo SafeInvokeMethod =
        typeof(EventHelper).GetMethod(nameof(SafeInvoke), BindingFlags.Public | BindingFlags.Static);

    public static void SafeInvoke(Delegate del, params object[] arguments)
    {
        if (del == null)
        {
            return;
        }

        var invocationList = del.GetInvocationList();
        if (invocationList == null || invocationList.Length == 0)
        {
            return;
        }

        if (invocationList.Length == 1)
        {
            try
            {
                del.DynamicInvoke(arguments);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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
}