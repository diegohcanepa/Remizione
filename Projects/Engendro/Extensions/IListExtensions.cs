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

        extension<T>(IList<T> list)
        {
            #region Private members

            // ShuffleCore
            private void ShuffleCore(Random? randomObj)
            {
                randomObj ??= random;

                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = randomObj.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }

            // NextIndex
            public int NextIndex(T currentItem)
            {
                var currentIndex = list.IndexOf(currentItem);
                return currentIndex < 0 ? -1 : NextIndex(list, currentIndex);
            }

            // NextIndex
            public int NextIndex(int currentIndex)
            {
                // El operador módulo (%) maneja el wrap-around automáticamente.
                // (currentIndex + 1) será el siguiente índice. Si es igual al Count,
                // el módulo con Count devolverá 0.
                return (currentIndex + 1) % list.Count;
            }

            // NextItem
            public T? NextItem(T currentItem)
            {
                var index = NextIndex(list, currentItem);
                if (index < 0)
                    return default;
                else
                    return list[index];
            }

            // PreviousIndex
            public int PreviousIndex(T currentItem)
            {
                var currentIndex = list.IndexOf(currentItem);
                return currentIndex < 0 ? -1 : PreviousIndex(list, currentIndex);
            }

            // PreviousIndex
            public int PreviousIndex(int currentIndex)
            {
                // El método más seguro para obtener el índice anterior cíclico en C# es:
                // (currentIndex - 1 + list.Count) garantiza que el resultado de (currentIndex - 1)
                // no sea negativo (lo que ocurre cuando currentIndex es 0), y luego
                // el módulo (%) con Count realiza el wrap-around al último índice.
                return (currentIndex - 1 + list.Count) % list.Count;
            }

            // PreviousItem
            public T? PreviousItem(T currentItem)
            {
                var index = PreviousIndex(list, currentItem);
                if (index < 0)
                    return default;
                else
                    return list[index];
            }

            // RandomIndex
            public int RandomIndex(Random? random = null)
            {
                random ??= Random.Shared;
                return list.Count == 0 ? -1 : random.Next(list.Count);
            }

            // Shuffle
            public void Shuffle()
            {
                ShuffleCore(list, null);
            }

            // Shuffle
            public void Shuffle(Random random)
            {
                ShuffleCore(list, random);
            }

            // Swap
            public void Swap(T item1, T item2)
            {
                var indexA = list.IndexOf(item1);
                var indexB = list.IndexOf(item2);

                if (indexA != -1 && indexB != -1 && indexA != indexB)
                {
                    Swap(list, indexA, indexB);
                }
            }

            // Swap
            public void Swap(int indexA, int indexB)
            {
                var tmp = list[indexA];
                list[indexA] = list[indexB];
                list[indexB] = tmp;
            }
        }

        extension<T>(IList<T> list) where T : class
        {
            #endregion

            // GetRandomItem
            public T? GetRandomItem(Random? random = null)
            {
                TryGetRandomItem(list, out var result);
                return result;
            }

            // SwapToPrevious
            public bool SwapToPrevious(T item)
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
            public bool SwapToNext(T item)
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
            public bool TryGetRandomItem(out T? item, Random? random = null)
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
}
