using System.Collections.Generic;

namespace xggameplan.core.Extensions
{
    /// <summary>
    /// Combinations
    /// </summary>
    public static class Combinations{
        private static IEnumerable<int[]> GetCombinationsRecursive(int[] buffer, int done, int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                buffer[done] = i;

                if (done == buffer.Length - 1)
                {
                    yield return buffer;
                }
                else
                {
                    foreach (int[] child in GetCombinationsRecursive(buffer, done + 1, i + 1, end))
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Returns combinations of size N from M items, integers
        /// </summary>
        /// <param name="combinationSize"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        private static IEnumerable<int[]> GetCombinationIntegers(int combinationSize, int itemCount) =>
            GetCombinationsRecursive(new int[combinationSize], 0, 0, itemCount);

        /// <summary>
        /// Returns combinations for specific types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="combinationSize"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> GetCombinations<T>(int combinationSize, T[] items)
        {
            var combinations = new List<T[]>();

            foreach (int[] combination in GetCombinationIntegers(combinationSize, items.Length))
            {
                var values = new T[combination.Length];

                for (int index = 0; index < combination.Length; index++)
                {
                    values[index] = items[combination[index]];
                }

                combinations.Add(values);
            }

            return combinations;
        }
    }
}
