using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engendro.PathFinding
{
    /// <summary>
    /// AStar
    /// </summary>
    public static class AStar
    {
        // CalculatePath
        public static Vector2[]? CalculatePath(PathNode startNode, PathNode endNode, IList<PathNode> nodes)
        {
            // Priority queue for open nodes
            Heap<PathNode> openList = new(nodes.Count);
            HashSet<PathNode> closedList = [];
            var success = false;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var currentNode = openList.RemoveFirst();
                if (currentNode == endNode)
                {
                    success = true;
                    break;
                }

                closedList.Add(currentNode);

                var linkIndeces = currentNode.Links;
                for (var i = 0; i < linkIndeces.Count; i++)
                {
                    var neighbour = nodes[linkIndeces[i]];

                    if (closedList.Contains(neighbour))
                        continue;

                    var costToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (costToNeighbour < neighbour.GCost || !openList.Contains(neighbour))
                    {
                        neighbour.GCost = costToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, endNode);
                        neighbour.Parent = currentNode;

                        if (openList.Contains(neighbour))
                            openList.UpdateItem(neighbour);
                        else
                            openList.Add(neighbour);
                    }
                }
            }

            if (success)
            {
                // Retrace Path if one exists
                List<Vector2> path = [];
                var currentNode = endNode;
                while (currentNode != startNode && currentNode != null)
                {
                    path.Add(currentNode.Position);
                    currentNode = currentNode.Parent;
                }

                path.Reverse();

                return [.. path];
            }

            return null;
        }

        // GetDistance
        public static float GetDistance(PathNode a, PathNode b)
        {
            return (a.Position - b.Position).Length();
        }
    }
}