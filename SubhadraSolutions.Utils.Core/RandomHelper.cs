using System;

namespace SubhadraSolutions.Utils;

public static class RandomHelper
{
    private static readonly Random random = new();

    public static int Next(int n)
    {
        return random.Next(n);
    }

    public static double Random()
    {
        return random.NextDouble();
    }
}