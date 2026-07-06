using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// Define las operaciones de solo lectura seguras para cualquier polígono.
    /// </summary>
    public interface IReadOnlyPolygon
    {
        bool IsEmpty { get; }
        Rectangle BoundingRectangle { get; }
        RectangleF BoundingRectangleF { get; }
        Vector2 Clamp(Vector2 point);
        bool Contains(Vector2 point);
        bool ContainsVertex(RectangleF rect);
        bool ContainsVertex(IReadOnlyPolygon polygon);
        bool ContainsVertex(IList<Vector2> vertices);
        Vector2 GetClosestPointOnEdge(Vector2 point);
        Vector2[] GetVertices();
        Vector2[] GetVertices(Vector2 offset);
        void GetVertices(Span<Vector2> destination, Vector2 offset);
        bool InLineOfSight(Vector2 origin, Vector2 destination);
        bool Intersects(Vector2 start, Vector2 end);
        bool IsVertexConcave(int vertex);
        PolygonOrientation Orientation { get; }
        Vector2 RandomPoint();
        Vector2 RandomPoint(Vector2 origin, float radius);
        Vector2 RandomPoint(Vector2 origin, float minimumRadius, float maximumRadius);
        ReadOnlyCollection<Vector2> Vertices { get; }
    }
}
