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

            // GetAbsolutePoint
            public Vector2 GetAbsolutePoint(float x, float y)
            {
                return GetAbsolutePoint(transform, new Vector2(x, y));
            }

            // GetAbsolutePoint
            public Vector2 GetAbsolutePoint(Vector2 position)
            {
                return GetAbsolutePoint(transform, position, 0, 0);
            }

            // GetAbsolutePoint
            public Vector2 GetAbsolutePoint(Vector2 position, Vector2 offset)
            {
                return GetAbsolutePoint(transform, position, offset.X, offset.Y);
            }

            // GetAbsolutePoint
            public Vector2 GetAbsolutePoint(Vector2 position, float xOffset, float yOffset)
            {
                var bbox = transform.BoundingBox;

                // X
                if (transform.IsFlippedHorizontally)
                    position.X = bbox.Left + bbox.Width - position.X;
                else
                    position.X += bbox.Left;

                // Y
                if (transform.IsFlippedVertically)
                    position.Y = bbox.Top + bbox.Height - position.Y;
                else
                    position.Y += bbox.Top;

                position.X += xOffset;
                position.Y += yOffset;

                return position;
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
