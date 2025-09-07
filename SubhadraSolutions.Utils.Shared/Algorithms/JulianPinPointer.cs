using System;

namespace SubhadraSolutions.Utils.Algorithms;

public static class JulianPinPointer
{
    public static JulianTimestamped<T> PinPointEventBackward<T>(JulianDateTime startTime, JulianTimeSpan initialStep, JulianTimeSpan precession,
        Func<JulianDateTime, T> valueProvider, Func<T, T, bool> conditionFunc)
    {
        return PinPointEventForwardOrBackwork(false, startTime, initialStep, precession, valueProvider, conditionFunc);
    }

    public static JulianTimestamped<T> PinPointEventForward<T>(JulianDateTime startTime, JulianTimeSpan initialStep, JulianTimeSpan precession,
        Func<JulianDateTime, T> valueProvider, Func<T, T, bool> conditionFunc)
    {
        return PinPointEventForwardOrBackwork(true, startTime, initialStep, precession, valueProvider, conditionFunc);
    }

    public static JulianDateTime PinPointFloor(JulianDateTime startTime, JulianTimeSpan initialStep, JulianTimeSpan precession,
        Func<JulianDateTime, double> valueProvider, double requiredValue)
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


    private static JulianTimestamped<T> PinPointEventForwardOrBackwork<T>(bool forward, JulianDateTime startTime,
        JulianTimeSpan initialStep, JulianTimeSpan precession, Func<JulianDateTime, T> valueProvider, Func<T, T, bool> conditionFunc)
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

        return new JulianTimestamped<T>(lastmet, lastmetValue);
    }
}