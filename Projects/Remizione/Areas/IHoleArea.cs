using Engendro;
using Engendro.PathFinding;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// IHoleArea
    /// </summary>
    public interface IHoleArea
    {
        // ClampOutside
        Vector2 ClampOutside(Vector2 position);

        // CollectNodes
        void CollectNodes(IList<PathNode> list);

        // Contains
        bool Contains(Vector2 point);

        // InLineOfSight
        bool InLineOfSight(Vector2 start, Vector2 end);

        // Name
        string Name { get; }

        // Polygon
        ReadOnlyPolygon Polygon { get; }
    }
}
