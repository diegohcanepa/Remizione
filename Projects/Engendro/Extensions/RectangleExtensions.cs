using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// RectangleExtensions
    /// </summary>
    public static class RectangleExtensions
    {
        // GetPoint
        public static Vector2 GetPoint(this Rectangle rectangle, RectanglePoint origin)
        {
            return GetPoint(rectangle, origin, 0, 0);
        }

        // GetPoint
        public static Vector2 GetPoint(this Rectangle rectangle, RectanglePoint origin, Vector2 offset)
        {
            return GetPoint(rectangle, origin, offset.X, offset.Y);
        }

        // GetPoint
        public static Vector2 GetPoint(this Rectangle rectangle, RectanglePoint origin, float xOffset, float yOffset)
        {
            return origin switch
            {
                // Bottom
                RectanglePoint.Bottom => new Vector2(rectangle.X + (rectangle.Width / 2) + xOffset, rectangle.Bottom + yOffset),

                // LeftBottom
                RectanglePoint.LeftBottom => new Vector2(rectangle.Left + xOffset, rectangle.Bottom + yOffset),

                // RightBottom
                RectanglePoint.RightBottom => new Vector2(rectangle.Right + xOffset, rectangle.Bottom + yOffset),

                // Left
                RectanglePoint.Left => new Vector2(rectangle.Left + xOffset, rectangle.Y + (rectangle.Height / 2) + yOffset),

                // Right
                RectanglePoint.Right => new Vector2(rectangle.Right + xOffset, rectangle.Y + (rectangle.Height / 2) + yOffset),

                // Top
                RectanglePoint.Top => new Vector2(rectangle.Left + (rectangle.Width / 2) + xOffset, rectangle.Top + yOffset),

                // LeftTop
                RectanglePoint.LeftTop => new Vector2(rectangle.Left + xOffset, rectangle.Top + yOffset),

                // RightTop
                RectanglePoint.RightTop => new Vector2(rectangle.Right + xOffset, rectangle.Top + yOffset),

                // Middle
                _ => new Vector2(rectangle.Center.X + xOffset, rectangle.Center.Y + yOffset),
            };
        }

        // GetRandomPoint
        public static Vector2 GetRandomPoint(this Rectangle rectangle)
        {
            var result = Vector2.Zero;

            if (rectangle.Width > 0)
                result.X = Random.Shared.Next(rectangle.Left, rectangle.Right + 1);

            if (rectangle.Height > 0)
                result.Y = Random.Shared.Next(rectangle.Top, rectangle.Bottom + 1);

            return result;
        }
    }
}
