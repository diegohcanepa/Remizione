using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// Implementación concreta y mutable de un Polígono.
    /// </summary>
    public sealed class Polygon : IReadOnlyPolygon
    {
        #region Private fields

        private Rectangle boundingRectangle;
        private RectangleF boundingRectangleF;
        private VertexType[]? vertexTypes;
        private readonly List<Vector2> vertices = [];

        #endregion

        #region Constructors

        // Constructor
        public Polygon()
        {
            this.Vertices = new ReadOnlyCollection<Vector2>(this.vertices);
            Clear();
        }

        // Constructor
        public Polygon(string value, float inflate = 0)
        {
            this.Vertices = new ReadOnlyCollection<Vector2>(this.vertices);
            SetVerticesCore(value, inflate);
        }

        // Constructor
        public Polygon(IList<Vector2> points, float inflate = 0)
        {
            this.Vertices = new ReadOnlyCollection<Vector2>(this.vertices);
            SetVerticesCore(points, inflate);
        }

        #endregion

        #region Private members

        // DeflateCore
        private void DeflateCore(float distance)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                var a = vertices[i == 0 ? vertices.Count - 1 : i - 1];
                var b = vertices[i];
                var c = vertices[(i + 1) % vertices.Count];

                Vector2 ab = Vector2.Normalize(a - b);
                Vector2 cb = Vector2.Normalize(c - b);
                var mid = ab + cb;
                mid *= IsVertexConcaveCore(i) ? -distance : distance;

                vertices[i] += mid;
            }
            InvalidateCachesOnly();
        }

        // GetBoundingRectangle
        private Rectangle GetBoundingRectangle()
        {
            var rectF = GetBoundingRectangleF();
            return new Rectangle((int)rectF.Left, (int)rectF.Top, (int)(rectF.Right - rectF.Left), (int)(rectF.Bottom - rectF.Top));
        }

        // GetBoundingRectangleF
        private RectangleF GetBoundingRectangleF()
        {
            if (vertices.Count < 3)
                return RectangleF.Empty;

            var left = float.MaxValue;
            var top = float.MaxValue;
            var right = float.MinValue;
            var bottom = float.MinValue;

            for (var i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].X < left) left = vertices[i].X;
                if (vertices[i].Y < top) top = vertices[i].Y;
                if (vertices[i].X > right) right = vertices[i].X;
                if (vertices[i].Y > bottom) bottom = vertices[i].Y;
            }

            return new RectangleF(left, top, right - left, bottom - top);
        }

        // InflateCore
        private void InflateCore(float distance)
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
            InvalidateCachesOnly();
        }

        // InvalidateCachesOnly
        private void InvalidateCachesOnly()
        {
            boundingRectangle = Rectangle.Empty;
            boundingRectangleF = RectangleF.Empty;
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

        // FlipCore  
        private void FlipCore(float? originX, float? originY)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (originX.HasValue)
                    vertices[i] = vertices[i] with { X = (2 * originX.Value) - vertices[i].X };

                if (originY.HasValue)
                    vertices[i] = vertices[i] with { Y = (2 * originY.Value) - vertices[i].Y };
            }

            InvalidateOrientation();
            InvalidateCachesOnly();
        }

        // OffsetCore
        private void OffsetCore(float x, float y)
        {
            var offsetVector = new Vector2(x, y);
            for (var i = 0; i < vertices.Count; i++)
            {
                vertices[i] += offsetVector;
            }
            InvalidateCachesOnly();
        }

        // SetVerticesCore
        private void SetVerticesCore(string value, float inflate)
        {
            var parsedVertices = GetVertices(value);
            SetVerticesCore(parsedVertices, inflate);
        }

        // SetVerticesCore
        private void SetVerticesCore(IList<Vector2> newVertices, float inflate)
        {
            this.vertices.Clear();
            this.vertices.AddRange(newVertices);

            InvalidateOrientation();

            if (inflate > 0)
                InflateCore(inflate);
            else if (inflate < 0)
                DeflateCore(Math.Abs(inflate));

            InvalidateCachesOnly();

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
            return !Contains(point) ? GetClosestPointOnEdge(point) : point;
        }

        // Clear
        public void Clear()
        {
            vertices.Clear();
            boundingRectangle = Rectangle.Empty;
            boundingRectangleF = RectangleF.Empty;
            vertexTypes = null;
            Orientation = PolygonOrientation.CounterClockwise;
        }

        // Contains
        public bool Contains(Vector2 point)
        {
            if (IsEmpty || !BoundingRectangleF.Contains(point))
                return false;

            bool inside = false;
            int count = vertices.Count;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if ((vertices[i].Y > point.Y) != (vertices[j].Y > point.Y) &&
                    point.X < ((vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y)) + vertices[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        // ContainsVertex
        public bool ContainsVertex(RectangleF rect)
        {
            // Optimización C#14 con Span para evitar generar basura en el GC
            ReadOnlySpan<Vector2> rectVertices = stackalloc Vector2[]
            {
                new(rect.Left, rect.Top),
                new(rect.Right, rect.Top),
                new(rect.Right, rect.Bottom),
                new(rect.Left, rect.Bottom)
            };

            for (int i = 0; i < rectVertices.Length; i++)
            {
                if (Contains(rectVertices[i]))
                    return true;
            }
            return false;
        }

        // ContainsVertex
        public bool ContainsVertex(IReadOnlyPolygon polygon)
        {
            return ContainsVertex(polygon.Vertices);
        }

        // ContainsVertex
        public bool ContainsVertex(IList<Vector2> verticesList)
        {
            for (var i = 0; i < verticesList.Count; i++)
            {
                if (Contains(verticesList[i]))
                    return true;
            }

            return false;
        }

        // Deflate
        public void Deflate(float distance)
        {
            DeflateCore(distance);
        }

        // Flip
        public void Flip(float originX, float originY)
        {
            FlipCore(originX, originY);
        }

        // FlipHorizontally
        public void FlipHorizontally(float originX)
        {
            FlipCore(originX, null);
        }

        // FlipVertically
        public void FlipVertically(float originY)
        {
            FlipCore(null, originY);
        }

        // GetClampPosition
        public Vector2 GetClampPosition(ITransform transform)
        {
            if (IsEmpty)
                return transform.Position;

            float width = transform.BoundingBox.Width;
            float height = transform.BoundingBox.Height;

            // 1. Calcular el desfase basado en el PivotOrigin. 
            // Esto define dónde está realmente la Posición respecto al rectangulo.
            float pivotOffsetX = transform.PivotOrigin switch
            {
                RectanglePoint.Top or RectanglePoint.Center or RectanglePoint.Bottom => -width / 2f,
                RectanglePoint.RightTop or RectanglePoint.Right or RectanglePoint.RightBottom => -width,
                _ => 0f // Cubre LeftTop, Left, LeftBottom
            };

            float pivotOffsetY = transform.PivotOrigin switch
            {
                RectanglePoint.Left or RectanglePoint.Center or RectanglePoint.Right => -height / 2f,
                RectanglePoint.LeftBottom or RectanglePoint.Bottom or RectanglePoint.RightBottom => -height,
                _ => 0f // Cubre LeftTop, Top, RightTop
            };

            // 2. Precalcular los vectores de desfase de las 4 esquinas
            Vector2 tlOffset = new(pivotOffsetX, pivotOffsetY);
            Vector2 trOffset = new(pivotOffsetX + width, pivotOffsetY);
            Vector2 blOffset = new(pivotOffsetX, pivotOffsetY + height);
            Vector2 brOffset = new(pivotOffsetX + width, pivotOffsetY + height);

            var correctedPosition = transform.Position;

            const int MaxCorrections = 4;
            const float SafetyMargin = 1.01f;

            for (int i = 0; i < MaxCorrections; i++)
            {
                // Aplicar los offsets a la posición iterada
                var topLeft = correctedPosition + tlOffset;
                var topRight = correctedPosition + trOffset;
                var bottomLeft = correctedPosition + blOffset;
                var bottomRight = correctedPosition + brOffset;

                var tlIn = Contains(topLeft);
                var trIn = Contains(topRight);
                var blIn = Contains(bottomLeft);
                var brIn = Contains(bottomRight);

                if (tlIn && trIn && blIn && brIn)
                    break;

                var criticalVertex = Vector2.Zero;

                if (!tlIn) criticalVertex = topLeft;
                else if (!trIn) criticalVertex = topRight;
                else if (!blIn) criticalVertex = bottomLeft;
                else if (!brIn) criticalVertex = bottomRight;

                var closestEdgePoint = GetClosestPointOnEdge(criticalVertex);
                var pushVector = closestEdgePoint - criticalVertex;

                correctedPosition += pushVector * SafetyMargin;
            }

            // 3. Fallback en caso de que la relajación falle
            if (!Contains(correctedPosition))
            {
                // Consideración de diseño: Si falla, clampeamos la Posición directamente, 
                // pero ten en cuenta que si el pivote NO es Center, el objeto podría quedar 
                // ligeramente fuera del polígono visible.
                correctedPosition = GetClosestPointOnEdge(transform.Position);
            }

            return correctedPosition;
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

            var xu = x1 + (u * (x2 - x1));
            var yu = y1 + (u * (y2 - y1));

            if (u < 0) return new Vector2(x1, y1);
            if (u > 1) return new Vector2(x2, y2);

            return new Vector2(xu, yu);
        }

        // GetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < vertices.Count; i++)
                {
                    hash = (hash * 31) + vertices[i].GetHashCode();
                }
                return hash;
            }
        }

        // GetVertices
        public Vector2[] GetVertices()
        {
            return [.. vertices];
        }

        // GetVertices
        public Vector2[] GetVertices(Vector2 offset)
        {
            var result = vertices.ToArray();
            for (var i = 0; i < result.Length; i++) result[i] += offset;
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

        // GetVertices
        public static Vector2[] GetVertices(string value)
        {
            var values = value.Split(';');
            var result = new Vector2[values.Length];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = DataConvert.ToVector2(values[i].Trim());
            }

            return result;
        }

        // Inflate
        public void Inflate(float distance)
        {
            InflateCore(distance);
        }

        // InLineOfSight
        public bool InLineOfSight(Vector2 origin, Vector2 destination)
        {
            if ((origin - destination).LengthSquared() < float.Epsilon)
                return true;

            for (var i = 0; i < vertices.Count; i++)
            {
                if (Geometry.LineSegmentsCross(origin, destination, vertices[i], vertices[(i + 1) % vertices.Count]))
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
                    if (Geometry.DistanceToSegment(start, v1, v2) > epsilon && Geometry.DistanceToSegment(end, v1, v2) > epsilon)
                        return true;
                }
            }

            return false;
        }

        // IsEmpty
        public bool IsEmpty => vertices.Count < 3;

        // IsVertexConcave
        public bool IsVertexConcave(int vertex)
        {
            if (vertexTypes != null)
                return vertexTypes[vertex] == VertexType.Concave;

            return IsVertexConcaveCore(vertex);
        }

        // Offset
        public void Offset(float x, float y)
        {
            OffsetCore(x, y);
        }

        // Orientation
        public PolygonOrientation Orientation { get; private set; }

        // RandomPoint
        public Vector2 RandomPoint()
        {
            var bounds = BoundingRectangle;
            Vector2 result = new(Random.Shared.Next(bounds.Left, bounds.Right + 1), Random.Shared.Next(bounds.Top, bounds.Bottom + 1));
            return Clamp(result);
        }

        // RandomPoint
        public Vector2 RandomPoint(Vector2 origin, float radius)
        {
            return RandomPoint(origin, 0, radius);
        }

        // RandomPoint
        public Vector2 RandomPoint(Vector2 origin, float minimumRadius, float maximumRadius)
        {
            return Clamp(origin.Random(minimumRadius, maximumRadius));
        }

        // SetVertices
        public void SetVertices(string value)
        {
            SetVerticesCore(value, 0);
        }

        // SetVertices
        public void SetVertices(string value, float inflate)
        {
            SetVerticesCore(value, inflate);
        }

        // SetVertices
        public void SetVertices(IList<Vector2> vertices)
        {
            SetVerticesCore(vertices, 0);
        }

        // SetVertices
        public void SetVertices(IList<Vector2> vertices, float inflate)
        {
            SetVerticesCore(vertices, inflate);
        }

        // Vertices
        public ReadOnlyCollection<Vector2> Vertices { get; }
    }
}