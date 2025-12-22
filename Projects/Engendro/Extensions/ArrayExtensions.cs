using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// ArrayExtensions
    /// </summary>
    public static class ArrayExtensions
    {
        extension<T>(T[] first)
        {
            // Concatenate
            public T[] Concatenate(T[] second)
            {
                if (first == null)
                    return second;

                if (second == null)
                    return first;

                List<T> result = [.. first, .. second];

                return result.ToArray();
            }
        }
    }
}
