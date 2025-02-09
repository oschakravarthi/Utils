using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Collections.Generic;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SubhadraSolutions.Utils.Collections;

public static class CollectionHelper
{
    public static readonly MethodInfo EnumerableToStringTemplateMethod =
        typeof(CollectionHelper).GetMethod(nameof(EnumerableToString), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo SortListInlineTemplateMethod =
        typeof(CollectionHelper).GetMethod(nameof(SortListInline), BindingFlags.Public | BindingFlags.Static);

    [DynamicallyInvoked]
    public static string EnumerableToString<T>(this IEnumerable<T> source)
    {
        if (source == null)
        {
            return null;
        }

        var sb = new StringBuilder();
        foreach (var item in source)
        {
            if (item == null)
            {
                continue;
            }

            sb.Append(item);
            sb.Append(", ");
        }

        if (sb.Length > 0)
        {
            sb.Length -= 2;
        }

        return sb.ToString();
    }

    public static bool Equals(IList list1, IList list2)
    {
        if (list1 == null && list2 == null)
        {
            return true;
        }

        if (list1 == null || list2 == null)
        {
            return false;
        }

        if (ReferenceEquals(list1, list2))
        {
            return true;
        }

        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (var i = 0; i < list1.Count; i++)
            if (list1[i] == null)
            {
                if (list2[i] != null)
                {
                    return false;
                }
            }
            else
            {
                if (!list1[i].Equals(list2[i]))
                {
                    return false;
                }
            }

        return true;
    }

    public static IEnumerable<T> FilterByType<T>(IEnumerable enumerable)
    {
        foreach (var v in enumerable)
        {
            if (v is T type)
            {
                yield return type;
            }
        }
    }

    public static U GetValueOrDefault<T, U>(this IDictionary<T, U> parameter, T key, U defaultValue)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException("Parameter cannot be null");
        }

        return parameter.TryGetValue(key, out var value) ? value : defaultValue;
    }

    //public static bool IsNullOrEmpty(this object obj)
    //{
    //    return obj == null || obj.ToString() == string.Empty;
    //}

    /// <summary>
    ///     Determines whether the collection is null or contains no elements.
    /// </summary>
    /// <typeparam name="T">The IEnumerable type.</typeparam>
    /// <param name="enumerable">The enumerable, which may be null or empty.</param>
    /// <returns>
    ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null)
        {
            return true;
        }

        /* If this is a list, use the Count property for perf.
         * The Count property is O(1) while IEnumerable.Count() is O(N). */
        if (enumerable is ICollection<T> collection)
        {
            return collection.Count < 1;
        }

        return !enumerable.Any();
    }

    public static IEnumerable<T> Merge<T>(IEnumerable<T> a, IEnumerable<T> b, bool removeDuplicates = false)
        where T : IComparable<T>
    {
        return Merge(a.GetEnumerator(), b.GetEnumerator(), removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerable<T> a, IEnumerable<T> b, IComparer<T> comparer,
        bool removeDuplicates = false)
    {
        return Merge(a.GetEnumerator(), b.GetEnumerator(), comparer, removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerable<T> a, IEnumerable<T> b, Comparison<T> comparer,
        bool removeDuplicates = false)
    {
        return Merge(a.GetEnumerator(), b.GetEnumerator(), comparer, removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerator<T> a, IEnumerator<T> b, bool removeDuplicates = false)
        where T : IComparable<T>
    {
        return Merge(a, b, (x, y) => x.CompareTo(y), removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerator<T> a, IEnumerator<T> b, IComparer<T> comparer,
        bool removeDuplicates = false)
    {
        return Merge(a, b, comparer.Compare, removeDuplicates);
    }

    public static IEnumerable<T> Merge<T>(IEnumerator<T> a, IEnumerator<T> b, Comparison<T> comparer,
        bool removeDuplicates = false)
    {
        var aCanMove = a.MoveNext();
        var bCanMove = b.MoveNext();

        while (aCanMove || bCanMove)
        {
            if (aCanMove && bCanMove)
            {
                var compareResult = comparer(a.Current, b.Current);
                if (compareResult == 0)
                {
                    yield return a.Current;
                    if (!removeDuplicates)
                    {
                        yield return b.Current;
                    }

                    aCanMove = a.MoveNext();
                    bCanMove = b.MoveNext();
                    continue;
                }

                if (compareResult < 0)
                {
                    yield return a.Current;
                    aCanMove = a.MoveNext();
                }
                else
                {
                    yield return b.Current;
                    bCanMove = b.MoveNext();
                }

                continue;
            }

            if (aCanMove)
            {
                while (aCanMove)
                {
                    yield return a.Current;
                    aCanMove = a.MoveNext();
                }
            }
            else
            {
                while (bCanMove)
                {
                    yield return b.Current;
                    bCanMove = b.MoveNext();
                }
            }
        }
    }

    //public static void ExportToStringSeparatedFile<T>(this IEnumerable<T> enumerable, string fileName, Encoding encoding = null, string delimiter = "\t")
    //{
    //    StreamWriter writer = encoding == null ? new StreamWriter(fileName) : new StreamWriter(fileName, false, encoding);
    //    using (writer)
    //    {
    //        DynamicStringSeparatedExportHelper<T>.WriteToTextWriter(enumerable, writer, delimiter, true);
    //    }
    //}
    public static IEnumerable<T> RemoveSequencedDuplicates<T>(this IEnumerable<T> enumerable,
        Func<T, T, bool> equalityComparerFunc)
    {
        return new SequencedDuplicatedRemoverEnumerableDecorator<T>(enumerable, equalityComparerFunc);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = RandomHelper.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    [DynamicallyInvoked]
    public static List<T> SortListInline<T>(this List<T> source, SortingOrder sortingOrder)
    {
        if (source == null)
        {
            return null;
        }

        IComparer<T> comparer = Comparer<T>.Default;
        if (sortingOrder == SortingOrder.Descending)
        {
            comparer = new InverseComparer<T>(comparer);
        }

        source.Sort(comparer);
        return source;
    }

    [DynamicallyInvoked]
    public static IEnumerable<TTo> ToMappedEnumerable<TFrom, TTo>(this IEnumerable<TFrom> source,
        Func<TFrom, TTo> mapper)
    {
        if (source != null)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            foreach (var item in source)
            {
                yield return mapper(item);
            }
        }
    }

    [DynamicallyInvoked]
    public static IEnumerable<TTo> ToMappedEnumerable<TFrom, TTo>(this IEnumerable<TFrom> source)
    {
        var mapper = ConvertHelper.GetConvertMethod<TFrom, TTo>(true);
        return source.ToMappedEnumerable(mapper);
    }
}