using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// ITransformableExtensions
    /// </summary>
    public static class ITransformExtensions
    {
        extension(ITransform transform)
        {
            // GetAbsoluteBounds
            public RectangleF GetAbsoluteBounds(Rectangle bounds, float yOffset = 0)
            {
                if (bounds.IsEmpty)
                    return RectangleF.Empty;

                var bbox = transform.BoundingBox;
                var lt = bbox.GetPoint(RectanglePoint.LeftTop, 0, yOffset);

                RectangleF result = new(lt.X + (bounds.X * transform.ScaleX),
                                            lt.Y + (bounds.Y * transform.ScaleY),
                                            bounds.Width * transform.ScaleX,
                                            bounds.Height * transform.ScaleY);

                if (transform.IsFlippedHorizontally)
                    result.X = bbox.Left + (bbox.Right - result.Right);

                if (transform.IsFlippedVertically)
                    result.Y = bbox.Top + (bbox.Bottom - result.Bottom);

                return result;
            }

            // GetAnchoredPosition
            public Vector2 GetAnchoredPosition(Vector2 localOffset)
            {
                var bbox = transform.BoundingBox;

                // 1. Aplicar la escala al desplazamiento local primero.
                // Esto asegura que si el objeto es el doble de grande, el offset también lo sea.
                float scaledX = localOffset.X * transform.ScaleX;
                float scaledY = localOffset.Y * transform.ScaleY;

                Vector2 result = new();

                // 2. Proyectar sobre el BoundingBox considerando el Flip
                if (transform.IsFlippedHorizontally)
                    result.X = bbox.Right - scaledX;
                else
                    result.X = bbox.Left + scaledX;

                if (transform.IsFlippedVertically)
                    result.Y = bbox.Bottom - scaledY;
                else
                    result.Y = bbox.Top + scaledY;

                return result;
            }

            // GetAnchoredPosition
            public Vector2 GetAnchoredPosition(float x, float y)
            {
                return GetAnchoredPosition(transform, new Vector2(x, y));
            }
        }

        // HasBottomPivot
        public static bool HasBottomPivot(RectanglePoint value)
        {
            return value is RectanglePoint.LeftBottom or
                   RectanglePoint.RightBottom or
                   RectanglePoint.Bottom;
        }

        // HasLeftPivot
        public static bool HasLeftPivot(RectanglePoint value)
        {
            return value is RectanglePoint.LeftBottom or
                   RectanglePoint.LeftTop or
                   RectanglePoint.Left;
        }

        // HasRightPivot
        public static bool HasRightPivot(RectanglePoint value)
        {
            return value is RectanglePoint.RightBottom or
                   RectanglePoint.RightTop or
                   RectanglePoint.Right;
        }

        // HasTopPivot
        public static bool HasTopPivot(RectanglePoint value)
        {
            return value is RectanglePoint.LeftTop or
                   RectanglePoint.RightTop or
                   RectanglePoint.Top;
        }
    }
}
