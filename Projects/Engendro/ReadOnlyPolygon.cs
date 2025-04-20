using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// ReadOnlyPolygon
    /// </summary>
    public class ReadOnlyPolygon
    {
        #region Private fields

        private Rectangle boundingRectangle;
        private RectangleF boundingRectangleF;
        private VertexType[]? vertexTypes;
        private readonly List<Vector2> vertices = [];

        #endregion

        #region Constructors

        // Constructor
        public ReadOnlyPolygon(IList<Vector2> points, float inflate = 0)
        {
            this.Vertices = new ReadOnlyCollection<Vector2>(this.vertices);
            SetVerticesCore(points, inflate);
        }

        #endregion

        #region Private members

        // GetBoundingRectangle
        private Rectangle GetBoundingRectangle()
        {
            var rectF = GetBoundingRectangleF();
            return new Rectangle((int)rectF.Left, (int)rectF.Top, (int)(rectF.Right - rectF.Left), (int)(rectF.Bottom - rectF.Top));
        }

        // GetBoundingRectangleF
        private RectangleF GetBoundingRectangleF()
        {
            if (vertices.Count < 4)
                return RectangleF.Empty;

            var left = float.MaxValue;
            var top = float.MaxValue;
            var right = float.MinValue;
            var bottom = float.MinValue;

            for (var i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].X < left)
                    left = vertices[i].X;

                if (vertices[i].Y < top)
                    top = vertices[i].Y;

                if (vertices[i].X > right)
                    right = vertices[i].X;

                if (vertices[i].Y > bottom)
                    bottom = vertices[i].Y;
            }

            return new RectangleF(left, top, right - left, bottom - top);
        }

        // InvalidateOrientation
        private void InvalidateOrientation()
        {
            if (IsEmpty)
                return;

            float area = 0;
            for (var i = 0; i < vertices.Count; i++)
            {
                var j = (i + 1) % vertices.Count;

                area += vertices[i].X * vertices[j].Y;
                area -= vertices[j].X * vertices[i].Y;
            }

            Orientation = area > 0 ? PolygonOrientation.Clockwise : PolygonOrientation.CounterClockwise;
        }

        // IsVertexConcaveCore
        private bool IsVertexConcaveCore(int vertex)
        {
            var current = vertices[vertex];
            var next = vertices[(vertex + 1) % vertices.Count];
            var previous = vertices[vertex == 0 ? vertices.Count - 1 : vertex - 1];

            Vector2 left = new(current.X - previous.X, current.Y - previous.Y);
            Vector2 right = new(next.X - current.X, next.Y - current.Y);

            var cross = (left.X * right.Y) - (left.Y * right.X);

            return Orientation == PolygonOrientation.Clockwise ? cross < 0 : cross > 0;
        }

        #endregion

        #region Protected members

        // DeflateCore
        protected void DeflateCore(float distance)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                var a = vertices[i == 0 ? vertices.Count - 1 : i - 1];
                var b = vertices[i];
                var c = vertices[(i + 1) % vertices.Count];

                Vector2 ab = Vector2.Normalize(a - b);
                Vector2 cb = Vector2.Normalize(c - b);
                var mid = ab + cb;
                mid *= IsVertexConcaveCore(i) ? -distance : distance; // Invertimos la lógica

                vertices[i] += mid;
            }
        }

        // InflateCore (Optimized with ChatGPT)
        protected void InflateCore(float distance)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                var a = vertices[i == 0 ? vertices.Count - 1 : i - 1];
                var b = vertices[i];
                var c = vertices[(i + 1) % vertices.Count];

                Vector2 ab = Vector2.Normalize(a - b);
                Vector2 cb = Vector2.Normalize(c - b);
                var mid = ab + cb;
                mid *= !IsVertexConcaveCore(i) ? -distance : distance;

                vertices[i] += mid;
            }
        }

        // OffsetCore
        protected void OffsetCore(float x, float y)
        {
            for (var i = 0; i < vertices.Count; i++)
            {
                vertices[i] += new Vector2(x, y);
            }
        }

        // SetVerticesCore
        protected void SetVerticesCore(IList<Vector2> vertices, float inflate)
        {
            this.vertices.Clear();
            this.vertices.AddRange(vertices);
            boundingRectangle = Rectangle.Empty;
            boundingRectangleF = RectangleF.Empty;

            InvalidateOrientation();

            if (inflate > 0)
                InflateCore(inflate);
            else
                DeflateCore(Math.Abs(inflate));

            vertexTypes = new VertexType[this.vertices.Count];
            for (var i = 0; i < vertexTypes.Length; i++)
            {
                vertexTypes[i] = IsVertexConcaveCore(i) ? VertexType.Concave : VertexType.Convex;
            }
        }

        #endregion

        // BoundingRectangle
        public Rectangle BoundingRectangle
        {
            get
            {
                if (boundingRectangle.IsEmpty)
                    boundingRectangle = GetBoundingRectangle();

                return boundingRectangle;
            }
        }

        // BoundingRectangleF
        public RectangleF BoundingRectangleF
        {
            get
            {
                if (boundingRectangleF.IsEmpty)
                    boundingRectangleF = GetBoundingRectangleF();

                return boundingRectangleF;
            }
        }

        // Clamp
        public Vector2 Clamp(Vector2 point)
        {
            return !IsPointInside(point) ? GetClosestPointOnEdge(point) : point;
        }

        // GetClosestPointOnEdge
        public Vector2 GetClosestPointOnEdge(Vector2 point)
        {
            var vi1 = -1;
            var vi2 = -1;
            var mindist = float.PositiveInfinity;

            for (var i = 0; i < vertices.Count; i++)
            {
                var dist = Geometry.DistanceToSegment(point, vertices[i], vertices[(i + 1) % vertices.Count]);
                if (dist < mindist)
                {
                    mindist = dist;
                    vi1 = i;
                    vi2 = (i + 1) % vertices.Count;
                }
            }

            var p1 = vertices[vi1];
            var p2 = vertices[vi2];

            var x1 = p1.X;
            var y1 = p1.Y;
            var x2 = p2.X;
            var y2 = p2.Y;
            var x3 = point.X;
            var y3 = point.Y;

            var u = (((x3 - x1) * (x2 - x1)) + ((y3 - y1) * (y2 - y1))) / (((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));

            var xu = x1 + u * (x2 - x1);
            var yu = y1 + u * (y2 - y1);

            Vector2 lineVector;
            if (u < 0)
            {
                lineVector = new Vector2(x1, y1);
            }
            else if (u > 1)
            {
                lineVector = new Vector2(x2, y2);
            }
            else
            {
                lineVector = new Vector2(xu, yu);
            }

            return lineVector;
        }

        // GetHashCode (Optimized with ChatGPT)
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                for (int i = 0; i < vertices.Count; i++)
                {
                    hash = hash * 31 + vertices[i].GetHashCode();
                }

                return hash;
            }
        }

        // GetVertices
        public Vector2[] GetVertices() => vertices.ToArray();

        // GetVertices
        public Vector2[] GetVertices(Vector2 offset)
        {
            var result = vertices.ToArray();

            for (var i = 0; i < result.Length; i++)
            {
                result[i] += offset;
            }

            return result;
        }

        // GetVertices
        public void GetVertices(Span<Vector2> destination, Vector2 offset)
        {
            if (vertices.Count > destination.Length)
                throw new ArgumentException("Destination span is too small.");

            for (int i = 0; i < vertices.Count; i++)
            {
                destination[i] = vertices[i] + offset;
            }
        }

        // InLineOfSight
        public bool InLineOfSight(Vector2 start, Vector2 end)
        {
            if ((start - end).LengthSquared() < float.Epsilon)
                return true;

            for (var i = 0; i < Vertices.Count; i++)
            {
                if (Geometry.LineSegmentsCross(start, end, Vertices[i], Vertices[(i + 1) % Vertices.Count]))
                    return false;
            }

            return true;
        }

        // Intersects
        public bool Intersects(Vector2 start, Vector2 end)
        {
            const float epsilon = .5f;

            for (var i = 0; i < vertices.Count; i++)
            {
                var v1 = vertices[i];
                var v2 = vertices[(i + 1) % vertices.Count];

                if (Geometry.LineSegmentsCross(start, end, v1, v2))
                {
                    // In some cases a 'snapped' endpoint is just a little over the line due to rounding errors. So a 0.5 margin is used to tackle those cases.
                    if (Geometry.DistanceToSegment(start, v1, v2) > epsilon && Geometry.DistanceToSegment(end, v1, v2) > epsilon)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // IsEmpty
        public bool IsEmpty => vertices.Count < 3;

        // IsPointInside (optimized with ChatGPT)
        public bool IsPointInside(Vector2 point)
        {
            if (IsEmpty)
                return false;

            bool inside = false;
            int count = vertices.Count;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if ((vertices[i].Y > point.Y) != (vertices[j].Y > point.Y) &&
                    point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        // IsVertexConcave
        public bool IsVertexConcave(int vertex)
        {
            if (vertexTypes != null)
                return vertexTypes[vertex] == VertexType.Concave;
            else
                return IsVertexConcaveCore(vertex);
        }

        // Orientation
        public PolygonOrientation Orientation { get; private set; }

        // RandomPoint
        public Vector2 RandomPoint()
        {
            var bounds = BoundingRectangle;
            Vector2 result = new(Randomizer.Next(bounds.Left, bounds.Right), Randomizer.Next(bounds.Top, bounds.Bottom));
            result = Clamp(result);
            return result;
        }

        // RandomPoint
        public Vector2 RandomPoint(Vector2 origin, float radius) => RandomPoint(origin, 0, radius);

        // RandomPoint
        public Vector2 RandomPoint(Vector2 origin, float minimumRadius, float maximumRadius)
        {
            return Clamp(origin.Random(minimumRadius, maximumRadius));
        }

        // Vertices
        public ReadOnlyCollection<Vector2> Vertices { get; }
    }
}
