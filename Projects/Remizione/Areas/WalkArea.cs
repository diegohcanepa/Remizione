using Engendro;
using Engendro.PathFinding;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// WalkArea
    /// </summary>
    public sealed class WalkArea : Room.Area
    {
        #region Private members

        private readonly ReadOnlyPolygon deflatedPolygon;
        private readonly PathNode findPathEndNode = new();
        private readonly PathNode findPathStartNode = new();
        private readonly List<IHoleArea> holeAreas = [];
        private readonly NamedObjectCollection<HoleArea> holes = [];
        private readonly ReadOnlyPolygon inflatedPolygon;
        private readonly List<PathNode> linkedNodes = [];
        private readonly List<PathNode> walkAreaNodes = [];

        #endregion

        #region Constructor

        // Constructor
        internal WalkArea(GameRoom room, string name, FlagCondition? condition, params Vector2[] vertices)
            : base(room, name, condition, vertices)
        {
            this.deflatedPolygon = new ReadOnlyPolygon(Polygon.Vertices, -.01f);
            this.inflatedPolygon = new ReadOnlyPolygon(Polygon.Vertices, .01f);
            this.Holes = new RoomAreaReadOnlyCollection<HoleArea>(holes);

            // Create nodes (concave vertices)
            if (!Polygon.IsEmpty)
            {
                // Deflate polygon by a marginal value to allow InLineOfSight between them
                var p = new ReadOnlyPolygon(Polygon.Vertices, -.05f);

                for (var i = 0; i < p.Vertices.Count; i++)
                {
                    if (p.IsVertexConcave(i))
                        walkAreaNodes.Add(new PathNode(p.Vertices[i]));
                }
            }

            ObstacleAreas = new ReadOnlyCollection<IHoleArea>(holeAreas);
        }

        #endregion

        #region Private members

        // CollectHoles
        private void CollectHoles(List<IHoleArea> list, ref RectangleF clipBox)
        {
            // Holes
            for (var i = 0; i < holes.Count; i++)
            {
                if (!holes[i].Polygon.BoundingRectangleF.Intersects(clipBox))
                    continue;

                if (holes[i].Test())
                    list.Add(holes[i]);
            }
        }

        // CollectThingHoles
        private void CollectThingHoles(GameThing? requester, List<IHoleArea> list, ref RectangleF clipBox)
        {
            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] == requester)
                    continue;

                if (Room.CulledThings[i] is IHoleArea holeArea && !holeArea.Polygon.IsEmpty)
                {
                    if (holeArea.Polygon.BoundingRectangleF.Intersects(clipBox))
                        list.Add(holeArea);
                }
            }
        }

        // LinkNodes
        private void LinkNodes()
        {
            for (var i = 0; i < linkedNodes.Count; i++)
                linkedNodes[i].Reset();

            for (var a = 0; a < linkedNodes.Count; a++)
            {
                linkedNodes[a].Links.Clear();

                for (var b = 0; b < linkedNodes.Count; b++)
                {
                    if (b > a)
                        continue;

                    if (linkedNodes[a] == linkedNodes[b])
                        continue;

                    if (InLineOfSight(linkedNodes[a].Position, linkedNodes[b].Position))
                    {
                        linkedNodes[a].Links.Add(b);
                        linkedNodes[b].Links.Add(a);
                    }
                }
            }
        }

        // LinkStartNode
        private void LinkStartNode(PathNode startNode)
        {
            for (var i = 0; i < linkedNodes.Count; i++)
            {
                if (InLineOfSight(startNode.Position, linkedNodes[i].Position))
                    startNode.Links.Add(i);
            }
        }

        // LinkEndNode
        private void LinkEndNode(PathNode endNode)
        {
            for (var i = 0; i < linkedNodes.Count; i++)
            {
                if (InLineOfSight(endNode.Position, linkedNodes[i].Position))
                {
                    endNode.Links.Add(i);
                    linkedNodes[i].Links.Add(linkedNodes.IndexOf(endNode));
                }
            }
        }

        // Prepare
        public void Prepare(GameThing requester, Vector2 destination)
        {
            RectangleF clipBox;

            // Define clip box for optimized path finding
            if (Room.Session.Camera.CullingBox.Contains(requester.Position) &&
                Room.Session.Camera.CullingBox.Contains(destination))
            {
                clipBox = Room.Session.Camera.CullingBox;
            }
            else
            {
                var startRect = new RectangleF(requester.Position, Room.Session.Camera.VisibleBox.Size);
                var destinationRect = new RectangleF(destination, Room.Session.Camera.VisibleBox.Size);
                clipBox = RectangleF.Union(startRect, destinationRect);
            }

            holeAreas.Clear();
            linkedNodes.Clear();

            // Collect holes
            CollectHoles(holeAreas, ref clipBox);
            CollectThingHoles(requester, holeAreas, ref clipBox);

            // Add walk area nodes
            linkedNodes.AddRange(walkAreaNodes);

            // Add hole nodes
            for (var i = 0; i < holeAreas.Count; i++)
            {
                holeAreas[i].CollectNodes(linkedNodes);
            }

            LinkNodes();
        }

        #endregion

        #region Protected members

        // OnEnabledChanged
        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();

            // Holes
            for (var i = 0; i < holes.Count; i++)
            {
                holes[i].IsEnabled = this.IsEnabled;
            }
        }

        #endregion

        // AddHole
        public HoleArea AddHole(string name, FlagCondition? condition, params Vector2[] vertices)
        {
            HoleArea result = new(this, name, condition, vertices);
            holes.Add(result);
            return result;
        }

        // ClampInside
        public Vector2 ClampInside(Vector2 point)
        {
            return ClampInside(point, out _);
        }

        // ClampInside
        public Vector2 ClampInside(Vector2 point, out bool clamped)
        {
            clamped = !IsInside(point);
            if (clamped)
                point = deflatedPolygon.GetClosestPointOnEdge(point);

            return point;
        }

        // FindPath
        public Vector2[]? FindPath(GameThing requester, Vector2 destination)
        {
            // Same position
            if (requester.Position == destination)
                return null;

            if (!IsInside(destination))
                destination = ClampInside(destination, out _);

            Prepare(requester, destination);

            var start = deflatedPolygon.Clamp(requester.Position);
            destination = deflatedPolygon.Clamp(destination);

            // Staight path
            if (InLineOfSight(start, destination))
                return [destination];

            // Create temp start/end nodes
            findPathStartNode.Position = GetWalkablePoint(start);
            findPathEndNode.Position = GetWalkablePoint(destination);

            linkedNodes.Add(findPathStartNode);
            LinkStartNode(findPathStartNode);

            linkedNodes.Add(findPathEndNode);
            LinkEndNode(findPathEndNode);

            var result = AStar.CalculatePath(findPathStartNode, findPathEndNode, linkedNodes);

            return result;
        }

        // GetWalkablePoint
        public Vector2 GetWalkablePoint(Vector2 point)
        {
            if (IsWalkableAt(point))
            {
                return point;
            }

            for (var i = 0; i < holeAreas.Count; i++)
            {
                if (holeAreas[i].IsInside(point))
                {
                    point = holeAreas[i].ClampOutside(point);
                    break;
                }
            }

            if (IsWalkableAt(point))
            {
                return point;
            }

            var distance = float.PositiveInfinity;
            PathNode? closestNode = null;
            for (var i = 0; i < walkAreaNodes.Count; i++)
            {
                var newDistance = Vector2.Distance(point, walkAreaNodes[i].Position);
                if (newDistance < distance)
                {
                    closestNode = walkAreaNodes[i];
                    distance = newDistance;
                }
            }

            return closestNode == null ? point : closestNode.Position;
        }

        // Holes
        public RoomAreaReadOnlyCollection<HoleArea> Holes { get; }

        // InLineOfSight
        public bool InLineOfSight(Vector2 value1, Vector2 value2)
        {
            if ((value1 - value2).LengthSquared() < float.Epsilon)
                return true;

            if (!inflatedPolygon.InLineOfSight(value1, value2))
                return false;

            for (var i = 0; i < holeAreas.Count; i++)
            {
                if (!holeAreas[i].InLineOfSight(value1, value2))
                    return false;
            }

            return true;
        }

        // IsInside
        public bool IsInside(Vector2 position)
        {
            return Polygon.IsPointInside(position);
        }

        // IsWalkableAt
        public bool IsWalkableAt(Vector2 point)
        {
            if (!IsInside(point))
                return false;

            for (var i = 0; i < holeAreas.Count; i++)
            {
                if (holeAreas[i].IsInside(point))
                {
                    return false;
                }
            }

            return true;
        }

        // ObstacleAreas
        public ReadOnlyCollection<IHoleArea> ObstacleAreas { get; }

        // RandomWalkablePoint
        public Vector2 RandomWalkablePoint()
        {
            return GetWalkablePoint(Polygon.RandomPoint());
        }

        // RandomWalkablePoint
        public Vector2 RandomWalkablePoint(Vector2 origin, float minimumRadius, float maximumRadius)
        {
            var pt = Polygon.RandomPoint(origin, minimumRadius, maximumRadius);
            return GetWalkablePoint(pt);
        }
    }
}