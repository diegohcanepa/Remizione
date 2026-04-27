using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// RectangleF
    /// </summary>
    public struct RectangleF : IEquatable<RectangleF>
    {
        #region Constructor

        // Constructor
        public RectangleF(Rectangle rect)
            : this(rect.X, rect.Y, rect.Width, rect.Height)
        {
        }

        // Constructor
        public RectangleF(Vector2 position, Vector2 size, bool fromCenter = false)
            : this(position.X - (fromCenter ? size.X / 2 : 0), position.Y - (fromCenter ? size.Y / 2 : 0), size.X, size.Y)
        {
        }

        // Constructor
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        #endregion

        #region Operators

        // =
        public static bool operator ==(RectangleF left, RectangleF right)
        {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }

        // !=
        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return !(left == right);
        }

        // Implicit conversion
        public static implicit operator RectangleF(Rectangle value)
        {
            return new(value.X, value.Y, value.Width, value.Height);
        }

        #endregion

        // Bottom
        public readonly float Bottom => Y + Height;

        // Center
        public readonly Vector2 Center => new(Left + (Width / 2), Top + (Height / 2));

        // Clamp
        public readonly Vector2 Clamp(Vector2 value)
        {
            value.X = MathHelper.Clamp(value.X, Left, Right);
            value.Y = MathHelper.Clamp(value.Y, Top, Bottom);

            return value;
        }

        // Contains
        public readonly bool Contains(float x, float y)
        {
            return X <= x && x < X + Width && Y <= y && y < Y + Height;
        }

        // Contains
        public readonly bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }

        // Contains
        public readonly bool Contains(RectangleF value)
        {
            return (X <= value.X) && ((value.X + value.Width) <= (X + Width)) &&
                                                  (Y <= value.Y) && ((value.Y + value.Height) <= (Y + Height));
        }

        // Empty
        public static readonly RectangleF Empty;

        // Equals
        public override readonly bool Equals(object? obj)
        {
            return obj is RectangleF rect && Equals(rect);
        }

        // Equals
        public readonly bool Equals(RectangleF other)
        {
            return (other.X == X) && (other.Y == Y) && (other.Width == Width) && (other.Height == Height);
        }

        // Height
        public float Height { get; set; }

        // GetHashCode
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }

        // GetPoint
        public readonly Vector2 GetPoint(RectanglePoint origin)
        {
            return GetPoint(origin, 0, 0);
        }

        // GetPoint
        public readonly Vector2 GetPoint(RectanglePoint origin, Vector2 offset)
        {
            return GetPoint(origin, offset.X, offset.Y);
        }

        // GetPoint
        public readonly Vector2 GetPoint(RectanglePoint origin, float xOffset, float yOffset)
        {
            return origin switch
            {
                // Bottom
                RectanglePoint.Bottom => new Vector2(X + (Width / 2) + xOffset, Bottom + yOffset),

                // LeftBottom
                RectanglePoint.LeftBottom => new Vector2(Left + xOffset, Bottom + yOffset),

                // RightBottom
                RectanglePoint.RightBottom => new Vector2(Right + xOffset, Bottom + yOffset),

                // Left
                RectanglePoint.Left => new Vector2(Left + xOffset, Y + (Height / 2) + yOffset),

                // Right
                RectanglePoint.Right => new Vector2(Right + xOffset, Y + (Height / 2) + yOffset),

                // Top
                RectanglePoint.Top => new Vector2(Left + (Width / 2) + xOffset, Top + yOffset),

                // LeftTop
                RectanglePoint.LeftTop => new Vector2(Left + xOffset, Top + yOffset),

                // RightTop
                RectanglePoint.RightTop => new Vector2(Right + xOffset, Top + yOffset),

                // Middle
                RectanglePoint.Center => new Vector2(Center.X + xOffset, Center.Y + yOffset),

                _ => throw new NotImplementedException(),
            };
        }

        // GetPoints
        public readonly Vector2[] GetPoints()
        {
            return [ GetPoint(RectanglePoint.LeftTop),
                     GetPoint(RectanglePoint.Top),
                     GetPoint(RectanglePoint.RightTop),
                     GetPoint(RectanglePoint.Right),
                     GetPoint(RectanglePoint.RightBottom),
                     GetPoint(RectanglePoint.Bottom),
                     GetPoint(RectanglePoint.LeftBottom) ];
        }

        // GetRandomPoint
        public readonly Vector2 GetRandomPoint()
        {
            var result = Vector2.Zero;

            if (Width > 0)
            {
                result.X = RandomHelper.Next(Random.Shared, Left, Right);
            }

            if (Height > 0)
            {
                result.Y = RandomHelper.Next(Random.Shared, Top, Bottom);
            }

            return result;
        }

        // GetVertices
        public readonly void GetVertices(Span<Vector2> destination)
        {
            destination[0] = GetPoint(RectanglePoint.LeftTop);
            destination[1] = GetPoint(RectanglePoint.RightTop);
            destination[2] = GetPoint(RectanglePoint.RightBottom);
            destination[3] = GetPoint(RectanglePoint.LeftBottom);
        }

        // Inflate
        public void Inflate(float x, float y)
        {
            X -= x;
            Y -= y;
            Width += 2 * x;
            Height += 2 * y;
        }

        // Inflate
        public void Inflate(Vector2 size)
        {
            Inflate(size.X, size.Y);
        }

        // Inflate
        public static RectangleF Inflate(RectangleF value, float x, float y)
        {
            var r = value;
            r.Inflate(x, y);
            return r;
        }

        // Intersects
        public static RectangleF Intersects(RectangleF value1, RectangleF value2)
        {
            var x1 = Math.Max(value1.X, value2.X);
            var x2 = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
            var y1 = Math.Max(value1.Y, value2.Y);
            var y2 = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new RectangleF(x1, y1, x2 - x1, y2 - y1);
            }

            return Empty;
        }

        // Intersects
        public readonly bool Intersects(RectangleF value)
        {
            return (value.X < X + Width) && (X < (value.X + value.Width)) && (value.Y < Y + Height) && (Y < value.Y + value.Height);
        }

        // Intersects
        public readonly bool Intersects(Vector2 start, Vector2 end)
        {
            if (IsEmpty)
                return false;

            if (Contains(start) || Contains(end))
                return true;

            // Upper segment
            if (Geometry.LineSegmentsCross(start, end, GetPoint(RectanglePoint.LeftTop), GetPoint(RectanglePoint.RightTop)))
            {
                return true;
            }

            // Right segment
            if (Geometry.LineSegmentsCross(start, end, GetPoint(RectanglePoint.RightTop), GetPoint(RectanglePoint.RightBottom)))
            {
                return true;
            }

            // Bottom segment
            if (Geometry.LineSegmentsCross(start, end, GetPoint(RectanglePoint.LeftBottom), GetPoint(RectanglePoint.RightBottom)))
            {
                return true;
            }

            // Left segment
            if (Geometry.LineSegmentsCross(start, end, GetPoint(RectanglePoint.LeftTop), GetPoint(RectanglePoint.LeftBottom)))
            {
                return true;
            }

            return false;
        }

        // IsEmpty
        public readonly bool IsEmpty => Width <= 0 || Height <= 0;

        // IsInside
        public readonly bool IsInside(RectangleF other)
        {
            return (X >= other.X) && (Right <= other.Right) &&
                   (Y >= other.Y) && (Bottom <= other.Bottom);
        }

        // Left
        public readonly float Left => X;

        // Location
        public Vector2 Location
        {
            readonly get => new(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        // Offset
        public void Offset(Vector2 amount)
        {
            Offset(amount.X, amount.Y);
        }

        // Offset
        public void Offset(float xOffset, float yOffset)
        {
            X += xOffset;
            Y += yOffset;
        }

        // Right
        public readonly float Right => X + Width;

        // Size
        public Vector2 Size
        {
            readonly get => new(Width, Height);
            set
            {
                this.Width = value.X;
                this.Height = value.Y;
            }
        }

        // Top
        public readonly float Top => Y;

        // ToRectangle
        public readonly Rectangle ToRectangle()
        {
            return new((int)X, (int)Y, (int)Width, (int)Height);
        }

        // ToRectangleF
        public static RectangleF ToRectangleF(float left, float top, float right, float bottom)
        {
            return new(left, top, right - left, bottom - top);
        }

        // ToString
        public override readonly string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.InvariantCulture) + ",Y=" + Y.ToString(CultureInfo.InvariantCulture) +
            ",Width=" + Width.ToString(CultureInfo.InvariantCulture) +
            ",Height=" + Height.ToString(CultureInfo.InvariantCulture) + "}";
        }

        // Union
        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            if (a.IsEmpty)
            {
                return b;
            }
            else if (b.IsEmpty)
            {
                return a;
            }

            var x1 = Math.Min(a.X, b.X);
            var x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            var y1 = Math.Min(a.Y, b.Y);
            var y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }

        // Union
        public static RectangleF Union(params RectangleF[] values)
        {
            if (values.Length == 0)
            {
                return Empty;
            }

            if (values.Length == 1)
            {
                return values[0];
            }

            var result = Empty;
            for (var i = 0; i < values.Length; i++)
            {
                result = Union(result, values[i]);
            }

            return result;
        }

        // Width
        public float Width { get; set; }

        // X
        public float X { get; set; }

        // Y
        public float Y { get; set; }
    }
}
