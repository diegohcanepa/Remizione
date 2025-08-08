using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Adberration
{
    /// <summary>
    /// RoomAreaReadOnlyCollection
    /// </summary>
    public class RoomAreaReadOnlyCollection<T> : NamedObjectReadOnlyCollection<T> where T : Room.Area
    {
        // AreaReadOnlyCollection
        public RoomAreaReadOnlyCollection(IList<T> list)
            : base(list)
        {
        }

        // Contains
        public bool Contains(float x, float y)
        {
            return GetAreaAt(x, y) != null;
        }

        // Contains
        public bool Contains(Vector2 position)
        {
            return GetAreaAt(position) != null;
        }

        // GetAreaAt
        public T? GetAreaAt(float x, float y)
        {
            return GetAreaAt(new Vector2(x, y));
        }

        // GetAreaAt
        public T? GetAreaAt(Vector2 position)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Polygon.Contains(position))
                {
                    return this[i];
                }
            }

            return null;
        }
    }
}
