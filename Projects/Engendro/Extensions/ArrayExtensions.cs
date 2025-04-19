using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// ArrayExtensions
    /// </summary>
    public static class ArrayExtensions
    {
        // Concatenate
        public static T[] Concatenate<T>(this T[] first, T[] second)
        {
            if (first == null)
            {
                return second;
            }

            if (second == null)
            {
                return first;
            }

            List<T> result = [.. first, .. second];

            return result.ToArray();
        }

        // RandomIndex
        public static int RandomIndex(this Array array)
        {
            if (array.Length == 0)
            {
                return -1;
            }

            return Randomizer.Next(0, array.Length - 1);
        }
    }
}
