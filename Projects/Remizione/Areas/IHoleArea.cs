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

        // InLineOfSight
        bool InLineOfSight(Vector2 start, Vector2 end);

        // IsInside
        bool IsInside(Vector2 point);

        // Name
        string Name { get; }

        // Polygon
        Polygon Polygon { get; }
    }
}
