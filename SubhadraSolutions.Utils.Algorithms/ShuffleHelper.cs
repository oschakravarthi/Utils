using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Algorithms;

public static class ShuffleHelper
{
    public static int[] BuildShuffleSequence(int count)
    {
        var ints = new int[count];
        for (var i = 0; i < count; i++) ints[i] = i;

        InPlaceShuffle(ints);
        return ints;
    }

    public static IEnumerable<Tuple<int, int>> BuildShuffleSequence(int size, int count)
    {
        var random = new Random();
        for (var i = 0; i < count; i++)
        {
            yield return new Tuple<int, int>(random.Next(size), random.Next(size));
        }
    }

    public static void InPlaceShuffle<T>(IList<T> list)
    {
        foreach (var pair in BuildShuffleSequence(list.Count, list.Count / 2))
        {
            (list[pair.Item1], list[pair.Item2]) = (list[pair.Item2], list[pair.Item1]);
        }
    }

    public static void InPlaceShuffle<T>(IList<T> list, int[] shuffleSequence)
    {
        for (var i = 0; i < shuffleSequence.Length; i++)
        {
            (list[i], list[shuffleSequence[i]]) = (list[shuffleSequence[i]], list[i]);
        }
    }

    public static IEnumerable<T> Shuffle<T>(IList<T> list)
    {
        var ints = BuildShuffleSequence(list.Count);
        foreach (var i in ints)
        {
            yield return list[i];
        }
    }

    public static IEnumerable<T> Shuffle<T>(IList<T> list, int[] shuffleSequence)
    {
        foreach (var sequence in shuffleSequence)
        {
            yield return list[sequence];
        }
    }
}