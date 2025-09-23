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
        public Polygon(string value, float inflate = 0)
            : base(value, inflate)
        {
        }

        // Constructor
        public Polygon(IList<Vector2> points, float inflate = 0)
            : base(points, inflate)
        {
        }

        #region Private fields

        // FlipCore
        private void FlipCore(float? originX, float? originY)
        {
            var vertices = GetVertices();

            for (int i = 0; i < vertices.Length; i++)
            {
                if (originX.HasValue)
                    vertices[i].X = 2 * originX.Value - vertices[i].X;

                if (originY.HasValue)
                    vertices[i].Y = 2 * originY.Value - vertices[i].Y;
            }

            SetVertices(vertices);
        }

        #endregion

        // Deflate
        public void Deflate(float distance) => DeflateCore(distance);

        // Flip
        public void Flip(float originX, float originY) => FlipCore(originX, originY);

        // FlipHorizontally
        public void FlipHorizontally(float originX) => FlipCore(originX, null);

        // FlipVertically
        public void FlipVertically(float originY) => FlipCore(null, originY);

        // Inflate
        public void Inflate(float distance) => InflateCore(distance);

        // Offset
        public void Offset(float x, float y) => OffsetCore(x, y);

        // SetVertices
        public void SetVertices(string value) => SetVerticesCore(value, 0);

        // SetVertices
        public void SetVertices(string value, float inflate) => SetVerticesCore(value, 0);

        // SetVertices
        public void SetVertices(IList<Vector2> vertices) => SetVerticesCore(vertices, 0);

        // SetVertices
        public void SetVertices(IList<Vector2> vertices, float inflate) => SetVerticesCore(vertices, inflate);
    }
}
