using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// Geometry
    /// </summary>
    public static class Geometry
    {
        // DistanceToSegmentSquared (Optimized by ChatGPT)
        public static float DistanceToSegmentSquared(Vector2 p, Vector2 v, Vector2 w)
        {
            float dx = w.X - v.X;
            float dy = w.Y - v.Y;
            float l2 = dx * dx + dy * dy;

            if (l2 == 0)
                return (p.X - v.X) * (p.X - v.X) + (p.Y - v.Y) * (p.Y - v.Y);

            float t = ((p.X - v.X) * dx + (p.Y - v.Y) * dy) / l2;

            if (t < 0)
                return (p.X - v.X) * (p.X - v.X) + (p.Y - v.Y) * (p.Y - v.Y);

            if (t > 1)
                return (p.X - w.X) * (p.X - w.X) + (p.Y - w.Y) * (p.Y - w.Y);

            float projX = v.X + t * dx;
            float projY = v.Y + t * dy;

            return (p.X - projX) * (p.X - projX) + (p.Y - projY) * (p.Y - projY);
        }

        // DistanceToSegment (Optimized by ChatGPT)
        public static float DistanceToSegment(Vector2 p, Vector2 v, Vector2 w)
        {
            return MathF.Sqrt(DistanceToSegmentSquared(p, v, w));
        }

        // IsColinear
        public static bool IsColinear(Vector2 a, Vector2 b, Vector2 c, float epsilon = 0.0001f)
        {
            var ab = b - a;
            var bc = c - b;
            return Math.Abs(ab.X * bc.Y - ab.Y * bc.X) < epsilon;
        }

        // LineSegmentsCross
        public static bool LineSegmentsCross(Vector2 startA, Vector2 endA, Vector2 startB, Vector2 endB)
        {
            var denominator = ((endA.X - startA.X) * (endB.Y - startB.Y)) - ((endA.Y - startA.Y) * (endB.X - startB.X));

            if (denominator == 0)
                return false;

            var numerator1 = ((startA.Y - startB.Y) * (endB.X - startB.X)) - ((startA.X - startB.X) * (endB.Y - startB.Y));
            var numerator2 = ((startA.Y - startB.Y) * (endA.X - startA.X)) - ((startA.X - startB.X) * (endA.Y - startA.Y));

            if (numerator1 == 0 || numerator2 == 0)
                return false;

            var r = numerator1 / denominator;
            var s = numerator2 / denominator;

            return r >= 0 && r <= 1 && s >= 0 && s <= 1;
        }

        // NormalizeAngle
        public static float NormalizeAngle(float degrees)
        {
            return (degrees %= 360) >= 0 ? degrees : (degrees + 360);
        }

        // SimplifyPolygon
        public static List<Vector2> SimplifyPolygon(Vector2[] polygon)
        {
            if (polygon.Length < 3)
                return new List<Vector2>(polygon);

            var result = new List<Vector2>();

            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 prev = polygon[(i - 1 + polygon.Length) % polygon.Length];
                Vector2 curr = polygon[i];
                Vector2 next = polygon[(i + 1) % polygon.Length];

                if (!Geometry.IsColinear(prev, curr, next))
                    result.Add(curr);
            }

            return result;
        }
    }
}
