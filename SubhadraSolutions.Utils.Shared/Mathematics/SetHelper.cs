using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Mathematics
{
    public static class SetHelper
    {
        /// <summary>
        /// Gets the permutations.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <returns>A list of T[].</returns>
        public static IEnumerable<T[]> GetPermutations<T>(this T[] set)
        {
            var temp = new T[set.Length];
            var flags = new BitArray(set.Length);
            var permutations = GetPermutations(set, temp, 0, flags);
            foreach (var p in permutations)
            {
                var array = new T[p.Length];
                Array.Copy(p, array, p.Length);
                yield return array;
            }
        }

        /// <summary>
        /// Gets the permutations with same output array.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <returns>A list of T[].</returns>
        public static IEnumerable<T[]> GetPermutationsWithSameOutputArray<T>(this T[] set)
        {
            var temp = new T[set.Length];
            var flags = new BitArray(set.Length);
            return GetPermutations(set, temp, 0, flags);
        }

        private static IEnumerable<T[]> GetPermutations<T>(T[] input, T[] temp, int index, BitArray flags)
        {
            var length = input.Length;
            if (index == length)
            {
                yield return temp;
            }
            for (int i = 0; i < length; i++)
            {
                if (!flags.Get(i))
                {
                    temp[index] = input[i];
                    flags.Set(i, true);
                    var permutations = GetPermutations(input, temp, index + 1, flags);
                    foreach (var permutation in permutations)
                    {
                        yield return permutation;
                    }
                    flags.Set(i, false);
                }
            }
        }
    }
}