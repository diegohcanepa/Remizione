using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Adberration
{
    /// <summary>
    /// RoomAreaReadOnlyCollection
    /// </summary>
    public class RoomAreaReadOnlyCollection<T> : NamedReadOnlyCollection<T> where T : Room.Area
    {
        // AreaReadOnlyCollection
        public RoomAreaReadOnlyCollection(IList<T> list)
            : base(list)
        {
        }

        // Contains
        public bool Contains(float x, float y)
        {
            return FindAreaAt(x, y) != null;
        }

        // Contains
        public bool Contains(Vector2 position)
        {
            return FindAreaAt(position) != null;
        }

        // FindAreaAt
        public T? FindAreaAt(float x, float y)
        {
            return FindAreaAt(new Vector2(x, y));
        }

        // FindAreaAt
        public T? FindAreaAt(Vector2 position)
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
