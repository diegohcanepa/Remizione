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

        #region Private members

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

        #endregion

        // GetRandomItem
        public static T? GetRandomItem<T>(this IList<T> list, Random? random = null) where T : class
        {
            TryGetRandomItem(list, out var result);
            return result;
        }

        // NextIndex
        public static int NextIndex<T>(this IList<T> list, T currentItem)
        {
            var currentIndex = list.IndexOf(currentItem);
            return currentIndex < 0 ? -1 : NextIndex(list, currentIndex);
        }

        // NextIndex
        public static int NextIndex<T>(this IList<T> list, int currentIndex)
        {
            // El operador módulo (%) maneja el wrap-around automáticamente.
            // (currentIndex + 1) será el siguiente índice. Si es igual al Count,
            // el módulo con Count devolverá 0.
            return (currentIndex + 1) % list.Count;
        }

        // NextItem
        public static T? NextItem<T>(this IList<T> list, T currentItem)
        {
            var index = NextIndex(list, currentItem);
            if (index < 0)
                return default;
            else
                return list[index];
        }

        // PreviousIndex
        public static int PreviousIndex<T>(this IList<T> list, T currentItem)
        {
            var currentIndex = list.IndexOf(currentItem);
            return currentIndex < 0 ? -1 : PreviousIndex(list, currentIndex);
        }

        // PreviousIndex
        public static int PreviousIndex<T>(this IList<T> list, int currentIndex)
        {
            // El método más seguro para obtener el índice anterior cíclico en C# es:
            // (currentIndex - 1 + list.Count) garantiza que el resultado de (currentIndex - 1)
            // no sea negativo (lo que ocurre cuando currentIndex es 0), y luego
            // el módulo (%) con Count realiza el wrap-around al último índice.
            return (currentIndex - 1 + list.Count) % list.Count;
        }

        // PreviousItem
        public static T? PreviousItem<T>(this IList<T> list, T currentItem)
        {
            var index = PreviousIndex(list, currentItem);
            if (index < 0)
                return default;
            else
                return list[index];
        }

        // RandomIndex
        public static int RandomIndex<T>(this IList<T> list, Random? random = null)
        {
            random ??= Random.Shared;
            return list.Count == 0 ? -1 : random.Next(list.Count);
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
            if (index is (-1) or 0)
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

        // TryGetRandomItem
        public static bool TryGetRandomItem<T>(this IList<T> list, out T? item, Random? random = null) where T : class
        {
            if (list.Count == 0)
            {
                item = default;
                return false;
            }

            random ??= Random.Shared;

            var index = Random.Shared.Next(list.Count);
            item = list[index];

            return true;
        }
    }
}
