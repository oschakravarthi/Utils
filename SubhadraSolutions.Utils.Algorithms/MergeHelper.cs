using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Algorithms;

public static class MergeHelper
{
    public static int GetKeyMatch(string k1, string k2)
    {
        var s1 = k1.Split('.');
        var s2 = k2.Split('.');
        return GetKeyMatch(s1, s2);
    }

    public static int GetKeyMatch(string[] s1, string[] s2)
    {
        var length = Math.Min(s1.Length, s2.Length);
        for (var i = 0; i < length; i++)
            if (!string.Equals(s1[i], s2[i], StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }

        return length;
    }

    public static void Merge<T>(T[] from, T to, IDictionary<T, HashSet<T>> groups)
    {
        var toSet = groups[to];
        foreach (var f in from)
        {
            foreach (var item in groups[f])
            {
                toSet.Add(item);
            }

            groups.Remove(f);
        }
    }

    public static Dictionary<TKey, HashSet<TValue>> MergeGroups<TKey, TValue>(
        Dictionary<TKey, HashSet<TValue>> groups,
        Func<TKey, TKey, HashSet<TValue>, HashSet<TValue>, bool> canMergeFunc,
        Func<TKey[], TKey, Dictionary<TKey, HashSet<TValue>>, Dictionary<TKey, HashSet<TValue>>> tryMergeGroupsFunc)
        where TKey : IEquatable<TKey>
    {
        var keys = new LinkedList<TKey>(groups.Keys);
        var p = keys.First;
        while (p != null)
        {
            var key1 = p.Value;
            var q = p.Next;
            while (q != null)
            {
                var key2 = q.Value;
                var values1 = groups[key1];
                var values2 = groups[key2];

                var canMerge = canMergeFunc(key2, key1, values2, values1);
                var merged = false;
                if (canMerge)
                {
                    var result = tryMergeGroupsFunc([key2], key1, groups);
                    if (result != null)
                    {
                        groups = result;
                        var next = q.Next;
                        keys.Remove(q);
                        q = next;
                        merged = true;
                    }
                }

                if (!merged)
                {
                    q = q.Next;
                }
            }

            p = p.Next;
        }

        return groups;
    }

    public static Dictionary<TKey, HashSet<TValue>> MergeGroupsByProximity<TKey, TValue>(
        Dictionary<TKey, HashSet<TValue>> groups, bool bothWays, int maxNumberOfItemsPerGroup,
        Func<TKey, TKey, HashSet<TValue>, HashSet<TValue>, double> proximityFunc,
        Func<TKey[], TKey, Dictionary<TKey, HashSet<TValue>>, Dictionary<TKey, HashSet<TValue>>> tryMergeGroupsFunc,
        IComparer<TKey> keyComparer) where TKey : IEquatable<TKey>
    {
        while (true)
        {
            var keys = groups.Keys.ToList();
            //keys.Sort();
            var pairs = BuildPairs(keys, bothWays);
            var queue = new PriorityQueue<GroupMergeInfo<TKey>, GroupMergeInfo<TKey>>(
                new GroupMergeInfoComparer<TKey>(keyComparer));
            foreach (var pair in pairs)
            {
                var key1 = pair.Key;
                var key2 = pair.Value;
                var values1 = groups[key1];
                var values2 = groups[key2];

                if (values1.Count == 1 || maxNumberOfItemsPerGroup <= 0 ||
                    values1.Count + values2.Count <= maxNumberOfItemsPerGroup)
                {
                    var proximity = proximityFunc(key1, key2, values1, values2);
                    if (proximity != 0 /*&& rank >= kvp.Value.Count*/)
                    {
                        var info = new GroupMergeInfo<TKey>(proximity, key1, key2);
                        queue.Enqueue(info, info);
                        //lastWritten = Insert(list, info, sortByNameAsWell, lastWritten);
                    }
                }
            }

            var hasChange = false;

            while (queue.Count > 0)
            {
                var l = queue.Dequeue();
                var result = tryMergeGroupsFunc([l.From], l.To, groups);
                if (result != null)
                {
                    groups = result;
                    hasChange = true;
                    queue = Update(queue, l.From, l.To, groups, maxNumberOfItemsPerGroup, proximityFunc);
                }
            }

            if (!hasChange)
            {
                break;
            }
        }

        return groups;
    }

    private static IEnumerable<KeyValuePair<T, T>> BuildPairs<T>(IList<T> keys, bool bothWays)
    {
        for (var i = 0; i < keys.Count; i++)
            for (var j = bothWays ? 0 : i + 1; j < keys.Count; j++)
                if (i != j)
                {
                    yield return new KeyValuePair<T, T>(keys[i], keys[j]);
                }
    }

    private static PriorityQueue<GroupMergeInfo<TKey>, GroupMergeInfo<TKey>> Update<TKey, TValue>(
        PriorityQueue<GroupMergeInfo<TKey>, GroupMergeInfo<TKey>> queue, TKey from, TKey to,
        IDictionary<TKey, HashSet<TValue>> groups, int maxNumberOfItemsPerGroup,
        Func<TKey, TKey, HashSet<TValue>, HashSet<TValue>, double> proximityFunc) where TKey : IEquatable<TKey>
    {
        var toSet = groups[to];
        var newQueue = new PriorityQueue<GroupMergeInfo<TKey>, GroupMergeInfo<TKey>>(queue.Comparer);
        while (queue.Count > 0)
        {
            var existing = queue.Dequeue();
            if (!existing.From.Equals(from) && !existing.To.Equals(from))
            {
                if (existing.To.Equals(to))
                {
                    var fromSet = groups[existing.From];
                    if (fromSet.Count == 1 || maxNumberOfItemsPerGroup <= 0 ||
                        fromSet.Count + toSet.Count <= maxNumberOfItemsPerGroup)
                    {
                        var proximity = proximityFunc(existing.From, to, fromSet, toSet);
                        if (proximity != 0)
                        {
                            var info = new GroupMergeInfo<TKey>(proximity, existing.From, to);
                            newQueue.Enqueue(info, info);
                        }
                    }
                }
                else
                {
                    newQueue.Enqueue(existing, existing);
                }
            }
        }

        return newQueue;
    }

    private class GroupMergeInfoComparer<T> : IComparer<GroupMergeInfo<T>>
    {
        private readonly IComparer<T> comparer;

        public GroupMergeInfoComparer(IComparer<T> comparer = null)
        {
            this.comparer = comparer;
        }

        public int Compare(GroupMergeInfo<T> x, GroupMergeInfo<T> y)
        {
            var result = y.Proximity.CompareTo(x.Proximity);
            if (comparer == null || result != 0)
            {
                return result;
            }

            return comparer.Compare(y.From, x.From);
        }
    }

    //public static void Insert(LinkedList<GroupMergeInfo> list, GroupMergeInfo mergeInfo, bool sortByNameAsWell)
    //{
    //    var p = list.First;
    //    while (p != null)
    //    {
    //        var existing = p.Value;
    //        if (existing.Proximity < mergeInfo.Proximity)
    //        {
    //            list.AddBefore(p, mergeInfo);
    //            return;
    //        }
    //        if (existing.Proximity == mergeInfo.Proximity)
    //        {
    //            if (!sortByNameAsWell || existing.From.CompareTo(mergeInfo.From) < 0)
    //            {
    //                list.AddBefore(p, mergeInfo);
    //                return;
    //            }
    //        }
    //        p = p.Next;
    //    }
    //    list.AddLast(mergeInfo);
    //}
    //public static LinkedListNode<GroupMergeInfo> Insert(LinkedList<GroupMergeInfo> list, GroupMergeInfo mergeInfo, bool sortByNameAsWell, LinkedListNode<GroupMergeInfo> lastWrittenNode)
    //{
    //    var p = lastWrittenNode;
    //    while (p != null && (p.Value.Proximity > mergeInfo.Proximity ||
    //        (p.Value.Proximity == mergeInfo.Proximity && (!sortByNameAsWell || p.Value.From.CompareTo(mergeInfo.From) > 0))))
    //    {
    //        p = p.Next;
    //    }
    //    if (p != null)
    //    {
    //        while (p != null && (p.Value.Proximity < mergeInfo.Proximity ||
    //        (p.Value.Proximity == mergeInfo.Proximity && (!sortByNameAsWell || p.Value.From.CompareTo(mergeInfo.From) < 0))))
    //        {
    //            p = p.Previous;
    //        }
    //    }
    //    if (p != null)
    //    {
    //        return list.AddAfter(p, mergeInfo);
    //    }

    //    return list.AddFirst(mergeInfo);
    //}
    //public static void Update<T>(LinkedList<GroupMergeInfo> list, string from, string to, IDictionary<string, HashSet<T>> groups, int maxNumberOfItemsPerGroup, Func<string, string, HashSet<T>, HashSet<T>, double> proximityFunc, bool sortByNameAsWell)
    //{
    //    var toSet = groups[to];
    //    var toAdd = new List<GroupMergeInfo>();
    //    var p = list.First;
    //    while (p != null)
    //    {
    //        var existing = p.Value;
    //        var removeNode = false;
    //        if (existing.From == from || existing.To == from)
    //        {
    //            removeNode = true;
    //        }
    //        else
    //        {
    //            if (existing.To == to)
    //            {
    //                var fromSet = groups[existing.From];
    //                if (fromSet.Count == 1 || maxNumberOfItemsPerGroup <= 0 || fromSet.Count + toSet.Count <= maxNumberOfItemsPerGroup)
    //                {
    //                    var proximity = proximityFunc(existing.From, to, fromSet, toSet);
    //                    if (proximity != 0)
    //                    {
    //                        toAdd.Add(new GroupMergeInfo(proximity, existing.From, to));
    //                    }
    //                }
    //                removeNode = true;
    //            }
    //        }
    //        if (removeNode)
    //        {
    //            var next = p.Next;
    //            list.Remove(p);
    //            p = next;
    //        }
    //        else
    //        {
    //            p = p.Next;
    //        }
    //    }
    //    foreach (var v in toAdd)
    //    {
    //        Insert(list, v, sortByNameAsWell);
    //    }
    //}
}