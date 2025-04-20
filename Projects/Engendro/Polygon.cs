using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// Polygon
    /// </summary>
    public sealed class Polygon : ReadOnlyPolygon
    {
        // Constructor
        public Polygon()
            : base([])
        {
        }

        // Constructor
        public Polygon(IList<Vector2> points, float inflate = 0)
            : base(points, inflate)
        {
        }

        // Deflate
        public void Deflate(float distance) => DeflateCore(distance);

        // Inflate
        public void Inflate(float distance) => InflateCore(distance);

        // Offset
        public void Offset(float x, float y) => OffsetCore(x, y);

        // SetVertices
        public void SetVertices(IList<Vector2> vertices) => SetVerticesCore(vertices, 0);

        // SetVertices
        public void SetVertices(IList<Vector2> vertices, float inflate) => SetVerticesCore(vertices, inflate);
    }
}
