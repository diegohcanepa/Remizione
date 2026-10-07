using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Collections;
using Engendro.PathFinding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// WalkArea
    /// </summary>
    public sealed class WalkArea : Room.Area
    {
        #region Private fields

        private readonly IReadOnlyPolygon deflatedPolygon;
        private readonly IReadOnlyPolygon inflatedPolygon;

        // Grafo estático precalculado (Nunca cambia a menos que cambie la topología del Room)
        private readonly List<PathNode> staticNodes = [];
        private bool isStaticGraphDirty = true;

        // Elementos dinámicos temporales para Pathfinding
        private readonly PathNode findPathEndNode = new();
        private readonly PathNode findPathStartNode = new();
        private readonly List<IHoleArea> dynamicHoles = [];
        private readonly List<PathNode> workingNodes = [];

        // Geometría estática
        private readonly NamedCollection<HoleArea> holes = [];
        private readonly List<PathNode> walkAreaNodes = [];
        private readonly List<IHoleArea> staticThingHoles = []; // Entidades GameThing marcadas como IsStatic

        #endregion

        #region Constructor

        // Constructor
        internal WalkArea(GameRoom room, string name, FlagCondition? condition, params Vector2[] vertices)
            : base(room, name, condition, vertices)
        {
            this.deflatedPolygon = new Polygon(Polygon.Vertices, -.01f);
            this.inflatedPolygon = new Polygon(Polygon.Vertices, .01f);
            this.Holes = new RoomAreaReadOnlyCollection<HoleArea>(holes);

            // Nodos base (Vértices cóncavos del WalkArea)
            if (!Polygon.IsEmpty)
            {
                var p = new Polygon(Polygon.Vertices, -.05f);

                for (var i = 0; i < p.Vertices.Count; i++)
                {
                    if (p.IsVertexConcave(i))
                        walkAreaNodes.Add(new PathNode(p.Vertices[i]));
                }
            }

            ObstacleAreas = new ReadOnlyCollection<IHoleArea>(dynamicHoles); // Se expone la caché dinámica (si es requerida externamente)
        }

        #endregion

        #region Pathfinding Optimizaciones

        /// <summary>
        /// Raycast optimizado que solo chequea colisión contra el polígono externo, 'holes' fijos y entidades IsStatic.
        /// No interactúa con los obstáculos dinámicos efímeros.
        /// </summary>
        private bool InLineOfSightAgainstStaticGeometry(Vector2 value1, Vector2 value2, RaycastContext context)
        {
            if ((value1 - value2).LengthSquared() < float.Epsilon)
                return true;

            if (!inflatedPolygon.InLineOfSight(value1, value2))
                return false;

            // Geometría nativa
            for (var i = 0; i < holes.Count; i++)
            {
                var hole = holes[i];
                if (context == RaycastContext.LineOfSight && !hole.BlocksLineOfSight) continue;
                if (!hole.InLineOfSight(value1, value2) || hole.Contains(value2)) return false;
            }

            // Entidades marcadas como estáticas
            for (var i = 0; i < staticThingHoles.Count; i++)
            {
                var hole = staticThingHoles[i];
                if (context == RaycastContext.LineOfSight && !hole.BlocksLineOfSight) continue;
                if (!hole.InLineOfSight(value1, value2) || hole.Contains(value2)) return false;
            }

            return true;
        }

        #endregion

        #region Private members

        // CollectThingHoles
        private void CollectThingHoles(GameThing requester, List<IHoleArea> list, ref RectangleF clipBox)
        {
            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] == requester)
                    continue;

                if (Room.CulledThings[i] is IHoleArea holeArea && holeArea.IsActive)
                {
                    // Si la entidad es estática, ya está en el grafo estático. La ignoramos aquí.
                    if (holeArea.IsStatic)
                        continue;

                    if (requester.Altitude > holeArea.CollisionHeight)
                        continue;

                    if (holeArea.Polygon.IsEmpty)
                        continue;

                    if (holeArea.Polygon.BoundingRectangleF.Intersects(clipBox))
                        list.Add(holeArea);
                }
            }
        }

        // FindPathCore
        private Vector2[]? FindPathCore(GameThing requester, Vector2 destination)
        {
            var start = deflatedPolygon.Clamp(requester.Position);
            destination = deflatedPolygon.Clamp(destination);

            // Straight path (Evaluado contra TODA la geometría, estática y dinámica)
            if (HasLineOfSight(start, destination, RaycastContext.Navigation, out IHoleArea? _))
                return [destination];

            // Asegurar que el esqueleto estático existe
            if (isStaticGraphDirty)
                BuildStaticNavGraph();

            findPathStartNode.Position = GetWalkablePoint(start);
            findPathEndNode.Position = GetWalkablePoint(destination);

            workingNodes.Clear();
            workingNodes.AddRange(staticNodes);

            // Purgar variables G, H y Parent del AStar anterior en los nodos estáticos, PERO preservar sus Links.
            for (var i = 0; i < workingNodes.Count; i++)
            {
                var n = workingNodes[i];
                n.GCost = 1;
                n.HCost = 0;
                n.Parent = null;
                n.HeapIndex = 0;
            }

            int baseNodeCount = workingNodes.Count;

            // Anexar nodos de agujeros dinámicos
            for (int i = 0; i < dynamicHoles.Count; i++)
            {
                dynamicHoles[i].CollectPathNodes(workingNodes);
            }

            // Anexar inicio y fin dinámicos
            workingNodes.Add(findPathStartNode);
            workingNodes.Add(findPathEndNode);

            int startNodeIndex = workingNodes.Count - 2;
            int endNodeIndex = workingNodes.Count - 1;

            // 1. Limpiar completamente los links de los nodos DYNAMICOS generados en este frame
            for (int i = baseNodeCount; i < workingNodes.Count; i++)
            {
                workingNodes[i].Links.Clear();
            }

            // 2. Conectar nodos DYNAMICOS contra todos los nodos visibles
            for (var a = baseNodeCount; a < workingNodes.Count; a++)
            {
                for (var b = 0; b < workingNodes.Count; b++)
                {
                    if (a == b) continue;

                    // Validación total (Holes + StaticThings + DynamicHoles)
                    if (HasLineOfSight(workingNodes[a].Position, workingNodes[b].Position, RaycastContext.Navigation, out IHoleArea? _))
                    {
                        workingNodes[a].Links.Add(b);
                        workingNodes[b].Links.Add(a); // Esto modifica el esqueleto estático temporalmente
                    }
                }
            }

            var result = AStar.CalculatePath(workingNodes[startNodeIndex], workingNodes[endNodeIndex], workingNodes);

            // 3. PURGAR ESQUELETO: Remover los links temporales añadidos a los nodos estáticos.
            for (int i = 0; i < baseNodeCount; i++)
            {
                var links = workingNodes[i].Links;
                for (int j = links.Count - 1; j >= 0; j--)
                {
                    if (links[j] >= baseNodeCount)
                    {
                        links.RemoveAt(j);
                    }
                }
            }

            return result;
        }

        // Prepare
        public Vector2 Prepare(GameThing requester, Vector2 destination)
        {
            var result = destination;

            RectangleF clipBox;

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

            dynamicHoles.Clear();
            CollectThingHoles(requester, dynamicHoles, ref clipBox);

            // Validar si el destino cae dentro de geometría estática nativa...
            for (var i = 0; i < holes.Count; i++)
            {
                if (holes[i].Contains(destination))
                {
                    result = holes[i].ClampOutside(destination);
                    break;
                }
            }

            // ...o entidades estáticas...
            for (var i = 0; i < staticThingHoles.Count; i++)
            {
                if (staticThingHoles[i].Contains(destination))
                {
                    result = staticThingHoles[i].ClampOutside(destination);
                    break;
                }
            }

            // ...o geometría dinámica
            for (var i = 0; i < dynamicHoles.Count; i++)
            {
                if (dynamicHoles[i].Contains(destination))
                {
                    result = dynamicHoles[i].ClampOutside(destination);
                    break;
                }
            }

            return result;
        }

        #endregion

        #region Protected members

        // OnEnabledChanged
        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();

            for (var i = 0; i < holes.Count; i++)
            {
                holes[i].IsEnabled = this.IsEnabled;
            }
        }

        #endregion

        // AddHole
        public HoleArea AddHole(string name, string vertices)
        {
            return AddHole(name, null, Engendro.Polygon.GetVertices(vertices));
        }

        // AddHole
        public HoleArea AddHole(string name, FlagCondition? condition, params Vector2[] vertices)
        {
            HoleArea result = new(this, name, condition, vertices);
            holes.Add(result);
            MarkStaticGraphDirty(); // Invalida el esqueleto para forzar su regeneración
            return result;
        }

        // BuildStaticNavGraph
        public void BuildStaticNavGraph()
        {
            staticNodes.Clear();
            staticThingHoles.Clear();

            // 1. Agregar nodos de área base
            staticNodes.AddRange(walkAreaNodes);

            // 2. Agregar nodos de agujeros fijos nativos (los que se añaden por AddHole)
            for (var i = 0; i < holes.Count; i++)
            {
                holes[i].CollectPathNodes(staticNodes);
            }

            // 3. Recolectar entidades estáticas de la habitación completa.
            for (var i = 0; i < Room.Children.Count; i++)
            {
                if (Room.Children[i] is IHoleArea hole && hole.IsActive && hole.IsStatic)
                {
                    if (!hole.Polygon.IsEmpty)
                    {
                        staticThingHoles.Add(hole);
                        hole.CollectPathNodes(staticNodes);
                    }
                }
            }

            // 4. Limpiar links y variables de AStar (G, H, Parent, HeapIndex)
            for (var i = 0; i < staticNodes.Count; i++)
            {
                staticNodes[i].Reset();
            }

            // 5. Calcular conexiones de LineOfSight para el esqueleto (Navigation Mode)
            for (var a = 0; a < staticNodes.Count; a++)
            {
                for (var b = a + 1; b < staticNodes.Count; b++)
                {
                    if (InLineOfSightAgainstStaticGeometry(staticNodes[a].Position, staticNodes[b].Position, RaycastContext.Navigation))
                    {
                        staticNodes[a].Links.Add(b);
                        staticNodes[b].Links.Add(a);
                    }
                }
            }

            isStaticGraphDirty = false;
        }

        // ClampInside
        public Vector2 ClampInside(Vector2 point)
        {
            return ClampInside(point, out _);
        }

        // ClampInside
        public Vector2 ClampInside(Vector2 point, out bool clamped)
        {
            clamped = !Contains(point);
            if (clamped)
                point = deflatedPolygon.GetClosestPointOnEdge(point);

            return point;
        }

        // Contains
        public bool Contains(Vector2 position)
        {
            return Polygon.Contains(position);
        }

        // FindPath
        public Vector2[]? FindPath(GameThing requester, Vector2 destination)
        {
            if (requester.Position == destination)
                return null;

            if (!Contains(destination))
                destination = ClampInside(destination, out _);

            destination = Prepare(requester, destination);

            var result = FindPathCore(requester, destination);

            return result;
        }

        // GetWalkablePoint
        public Vector2 GetWalkablePoint(Vector2 point)
        {
            if (IsWalkableAt(point))
                return point;

            // Clampeo contra entidades dinámicas
            for (var i = 0; i < dynamicHoles.Count; i++)
            {
                if (dynamicHoles[i].Contains(point))
                {
                    point = dynamicHoles[i].ClampOutside(point);
                    break;
                }
            }

            // Clampeo contra entidades estáticas
            for (var i = 0; i < staticThingHoles.Count; i++)
            {
                if (staticThingHoles[i].Contains(point))
                {
                    point = staticThingHoles[i].ClampOutside(point);
                    break;
                }
            }

            // Clampeo contra agujeros nativos
            for (var i = 0; i < holes.Count; i++)
            {
                if (holes[i].Contains(point))
                {
                    point = holes[i].ClampOutside(point);
                    break;
                }
            }

            if (IsWalkableAt(point))
                return point;

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

        // HasLineOfSight
        public bool HasLineOfSight(Vector2 value1, Vector2 value2, RaycastContext context, out IHoleArea? blockingArea)
        {
            return HasLineOfSight(value1, value2, context, null, out blockingArea);
        }

        // HasLineOfSight
        public bool HasLineOfSight(Vector2 value1, Vector2 value2, RaycastContext context, object? sender, out IHoleArea? blockingHoleArea)
        {
            blockingHoleArea = null;

            if ((value1 - value2).LengthSquared() < float.Epsilon)
                return true;

            if (!inflatedPolygon.InLineOfSight(value1, value2))
                return false;

            // Raycast contra agujeros nativos
            for (var i = 0; i < holes.Count; i++)
            {
                var hole = holes[i];
                if (hole == sender) continue;
                if (context == RaycastContext.LineOfSight && !hole.BlocksLineOfSight) continue;

                if (!hole.InLineOfSight(value1, value2) || hole.Contains(value2))
                {
                    blockingHoleArea = hole;
                    return false;
                }
            }

            // Raycast contra entidades estáticas
            for (var i = 0; i < staticThingHoles.Count; i++)
            {
                var hole = staticThingHoles[i];
                if (hole == sender) continue;
                if (context == RaycastContext.LineOfSight && !hole.BlocksLineOfSight) continue;

                if (!hole.InLineOfSight(value1, value2) || hole.Contains(value2))
                {
                    blockingHoleArea = hole;
                    return false;
                }
            }

            // Raycast contra entidades dinámicas (recogidas por la cámara)
            for (var i = 0; i < dynamicHoles.Count; i++)
            {
                var hole = dynamicHoles[i];
                if (hole == sender) continue;
                if (context == RaycastContext.LineOfSight && !hole.BlocksLineOfSight) continue;

                if (!hole.InLineOfSight(value1, value2) || hole.Contains(value2))
                {
                    blockingHoleArea = hole;
                    return false;
                }
            }

            return true;
        }

        // Holes
        public RoomAreaReadOnlyCollection<HoleArea> Holes { get; }

        // IsWalkableAt
        public bool IsWalkableAt(Vector2 point)
        {
            if (!Contains(point))
                return false;

            for (var i = 0; i < holes.Count; i++)
            {
                if (holes[i].Contains(point)) return false;
            }

            for (var i = 0; i < staticThingHoles.Count; i++)
            {
                if (staticThingHoles[i].Contains(point)) return false;
            }

            return true;
        }

        // MarkStaticGraphDirty
        public void MarkStaticGraphDirty()
        {
            isStaticGraphDirty = true;
        }

        // ObstacleAreas
        public ReadOnlyCollection<IHoleArea> ObstacleAreas { get; }

        // RandomWalkablePoint
        public Vector2 RandomWalkablePoint(Random rng, float margin = 0)
        {
            var result = GetWalkablePoint(Polygon.RandomPoint(rng));

            if (margin > 0)
            {
                var wap = new Polygon(Polygon.Vertices, -margin);
                result = wap.Clamp(result);
            }

            return result;
        }

        // RandomWalkablePoint
        public Vector2 RandomWalkablePoint(Random rng, Vector2 origin, float minimumRadius, float maximumRadius)
        {
            int attempts = 10;
            for (int i = 0; i < attempts; i++)
            {
                var pt = Polygon.RandomPoint(rng, origin, minimumRadius, maximumRadius);
                if (IsWalkableAt(pt)) return pt;
            }

            return origin;
        }
    }
}