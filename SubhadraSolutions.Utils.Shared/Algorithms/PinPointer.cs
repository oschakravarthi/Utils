using System;

namespace SubhadraSolutions.Utils.Algorithms;

public static class PinPointer
{
    public static Timestamped<T> PinPointEventBackward<T>(DateTime startTime, TimeSpan initialStep, TimeSpan precession,
        Func<DateTime, T> valueProvider, Func<T, T, bool> conditionFunc)
    {
        return PinPointEventForwardOrBackwork(false, startTime, initialStep, precession, valueProvider, conditionFunc);
    }

    public static Timestamped<T> PinPointEventForward<T>(DateTime startTime, TimeSpan initialStep, TimeSpan precession,
        Func<DateTime, T> valueProvider, Func<T, T, bool> conditionFunc)
    {
        return PinPointEventForwardOrBackwork(true, startTime, initialStep, precession, valueProvider, conditionFunc);
    }

    public static DateTime PinPointFloor(DateTime startTime, TimeSpan initialStep, TimeSpan precession,
        Func<DateTime, double> valueProvider, double requiredValue)
    {
        var step = initialStep;
        var datetime = startTime;
        while (true)
        {
            var presentValue = valueProvider(datetime);
            if (presentValue < requiredValue)
            {
                datetime += step;
            }
            else
            {
                step /= 2;
                datetime -= step;
            }

            if (step < precession && presentValue >= requiredValue)
            {
                return datetime;
            }
        }
    }

    private static Timestamped<T> PinPointEventForwardOrBackwork<T>(bool forward, DateTime startTime,
        TimeSpan initialStep, TimeSpan precession, Func<DateTime, T> valueProvider, Func<T, T, bool> conditionFunc)
    {
        var step = initialStep;
        var datetime = startTime;
        var originalValue = valueProvider(datetime);
        var latestValue = originalValue;
        var met = false;
        datetime = startTime;
        while (!met)
        {
            if (forward)
            {
                datetime += step;
            }
            else
            {
                datetime -= step;
            }

            latestValue = valueProvider(datetime);
            met = conditionFunc(originalValue, latestValue);
        }

        var lastmet = datetime;
        var lastmetValue = latestValue;
        step /= 2;
        if (forward)
        {
            datetime -= step;
        }
        else
        {
            datetime += step;
        }

        while (step >= precession)
        {
            step /= 2;
            latestValue = valueProvider(datetime);
            if (conditionFunc(originalValue, latestValue))
            {
                lastmet = datetime;
                lastmetValue = latestValue;
                if (forward)
                {
                    datetime -= step;
                }
                else
                {
                    datetime += step;
                }
            }
            else
            {
                if (forward)
                {
                    datetime += step;
                }
                else
                {
                    datetime -= step;
                }
            }
        }

        return new Timestamped<T>(lastmet, lastmetValue);
    }

    //public static DateTime PinPointFloor(DateTime startTime, TimeSpan initialStep, TimeSpan precession, Func<DateTime, double> valueProvider, double requiredValue)
    //{
    //    var step = initialStep;
    //    var datetime = startTime;
    //    DateTime? metdatetime = null;
    //    while (true)
    //    {
    //        var presentValue = valueProvider(datetime);
    //        if (presentValue < requiredValue)
    //        {
    //            datetime += step;
    //        }
    //        else
    //        {
    //            metdatetime = datetime;
    //            datetime -= step;
    //        }
    //        step /= 2;
    //        if (step < precession)
    //        {
    //            // return metdatetime.Value;
    //            return startTime;
    //        }
    //    }
    //}
}