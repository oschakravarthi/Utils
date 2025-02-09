using SubhadraSolutions.Utils.Validation;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Algorithms;

public static class InPlaceMergeSortHelper
{
    public static void MergeSort<T>(this IList<T> list)
    {
        list.MergeSort(Comparer<T>.Default);
    }

    public static void MergeSort<T>(this IList<T> list, IComparer<T> comparer)
    {
        Guard.ArgumentShouldNotBeNull(comparer, nameof(comparer));
        MergeSort(list, comparer, 0, list.Count - 1);
    }

    public static void MergeSort<T>(this IList<T> list, Comparison<T> comparison)
    {
        Guard.ArgumentShouldNotBeNull(comparison, nameof(comparison));
        var comparer = new DelegateBasedComparer<T>(comparison);
        MergeSort(list, comparer);
    }

    private static void MergeSort<T>(IList<T> list, IComparer<T> comparer, int fromPos, int toPos)
    {
        Guard.ArgumentShouldNotBeNull(list, nameof(list));
        Guard.ArgumentShouldNotBeNull(comparer, nameof(comparer));
        MergeSortCore(list, comparer, fromPos, toPos);
    }

    private static void MergeSortCore<T>(IList<T> list, IComparer<T> comparer, int fromPos, int toPos)
    {
        if (fromPos < toPos)
        {
            var mid = (fromPos + toPos) / 2;

            MergeSortCore(list, comparer, fromPos, mid);
            MergeSortCore(list, comparer, mid + 1, toPos);

            var endLow = mid;
            var startHigh = mid + 1;

            while ((fromPos <= endLow) & (startHigh <= toPos))
                if (comparer.Compare(list[fromPos], list[startHigh]) < 0)
                {
                    fromPos++;
                }
                else
                {
                    var temp = list[startHigh];
                    for (var index = startHigh - 1; index >= fromPos; index--) list[index + 1] = list[index];

                    list[fromPos] = temp;
                    fromPos++;
                    endLow++;
                    startHigh++;
                }
        }
    }
}