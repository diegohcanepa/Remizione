using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// NamedObjectCollectionHelper
    /// </summary>
    internal static class NamedObjectCollectionHelper
    {
        // Contains
        internal static bool Contains<T>(IList<T> list, string name) where T : class, INamedObject
        {
            return Find(list, name) != null;
        }

        // Find
        internal static T? Find<T>(IList<T> list, string name) where T : class, INamedObject
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Name == name)
                {
                    return list[i];
                }
            }

            return null;
        }

        // IndexOf
        internal static int IndexOf<T>(IList<T> list, string name) where T : class, INamedObject
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
