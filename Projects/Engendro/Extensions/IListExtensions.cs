using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// IListExtensions
    /// </summary>
    public static class IListExtensions
    {
        private static readonly Random random = new();

        // ShuffleCore
        private static void ShuffleCore<T>(this IList<T> list, Random? randomObj)
        {
            randomObj ??= random;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = randomObj.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }


        // GetRandomElement
        public static T? GetRandomElement<T>(this IList<T> list) where T : class
        {
            TryGetRandomElement(list, out var result);
            return result;
        }

        // RandomIndex
        public static int RandomIndex<T>(this IList<T> list)
        {
            return list.Count == 0 ? -1 : Randomizer.Next(0, list.Count - 1);
        }

        // Shuffle
        public static void Shuffle<T>(this IList<T> list) => ShuffleCore(list, null);

        // Shuffle
        public static void Shuffle<T>(this IList<T> list, Random random) => ShuffleCore(list, random);

        // Swap
        public static void Swap<T>(this IList<T> list, T item1, T item2)
        {
            var indexA = list.IndexOf(item1);
            var indexB = list.IndexOf(item2);

            if (indexA != -1 && indexB != -1 && indexA != indexB)
            {
                Swap(list, indexA, indexB);
            }
        }

        // Swap
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        // SwapToPrevious
        public static bool SwapToPrevious<T>(this IList<T> list, T item) where T : class
        {
            // Nothing to shift
            if (list.Count < 2)
            {
                return false;
            }

            var index = list.IndexOf(item);
            if (index == -1 || index == 0)
            {
                return false;
            }

            var item1 = list[index];
            var item2 = list[index - 1];

            Swap(list, item1, item2);

            return true;
        }

        // SwapToNext
        public static bool SwapToNext<T>(this IList<T> list, T item) where T : class
        {
            // Nothing to shift
            if (list.Count < 2)
            {
                return false;
            }

            var index = list.IndexOf(item);
            if (index == -1 || index == list.Count - 1)
            {
                return false;
            }

            var item1 = list[index];
            var item2 = list[index + 1];

            Swap(list, item1, item2);

            return true;
        }

        // TryGetRandomElement
        public static bool TryGetRandomElement<T>(this IList<T> list, out T? element) where T : class
        {
            if (list.Count == 0)
            {
                element = default;
                return false;
            }

            element = list[Randomizer.Next(0, list.Count - 1)];

            return true;
        }
    }
}
