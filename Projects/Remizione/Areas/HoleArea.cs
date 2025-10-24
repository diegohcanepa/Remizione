using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.PathFinding;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// HoleArea
    /// </summary>
    public class HoleArea : Room.Area, IHoleArea
    {
        private readonly ReadOnlyPolygon inflatedPolygon;
        private readonly List<PathNode> nodes = [];

        #region Constructor

        // Constructor
        public HoleArea(WalkArea walkArea, string name, FlagCondition? condition, params Vector2[] vertices)
            : base(walkArea.Room, name, condition, vertices)
        {
            this.WalkArea = walkArea;

            // Inflate polygon by a marginal value to allow InLineOfSight between them            
            this.inflatedPolygon = new(vertices, .01f);

            // Create nodes (only convex vertices inside walk area)
            if (!Polygon.IsEmpty)
            {
                for (var i = 0; i < inflatedPolygon.Vertices.Count; i++)
                {
                    if (!inflatedPolygon.IsVertexConcave(i))
                    {
                        if (walkArea.Contains(inflatedPolygon.Vertices[i]))
                            nodes.Add(new PathNode(inflatedPolygon.Vertices[i]));
                    }
                }
            }
        }

        #endregion

        #region IHoleArea explicit implementation

        // CollisionHeight
        int IHoleArea.CollisionHeight => 0;

        // IsActive
        bool IHoleArea.IsActive => true;

        #endregion

        // ClampOutside
        public Vector2 ClampOutside(Vector2 position)
        {
            if (Polygon.Contains(position))
                position = inflatedPolygon.GetClosestPointOnEdge(position);

            return position;
        }

        // CollectPathNodes
        public void CollectPathNodes(IList<PathNode> list)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                list.Add(nodes[i]);
            }
        }

        // Contains
        public bool Contains(Vector2 point) => Polygon.Contains(point);

        // InLineOfSight
        public bool InLineOfSight(Vector2 start, Vector2 end)
        {
            return Polygon.InLineOfSight(start, end);
        }

        // WalkArea
        public WalkArea WalkArea { get; }
    }
}
