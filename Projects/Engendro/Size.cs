using System;

namespace Engendro
{
    /// <summary>
    /// Represents a two-dimensional size with integer precision
    /// </summary>
    public readonly struct Size(int width, int height) : IEquatable<Size>
    {
        #region Operators

        // == operator
        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }

        // != operator
        public static bool operator !=(Size left, Size right)
        {
            return !left.Equals(right);
        }

        // + operator
        public override string ToString()
        {
            return $"{Width} × {Height}";
        }

        #endregion

        // Empty
        public static Size Empty { get; } = new(0, 0);

        // Height
        public int Height { get; } = height;

        // Equals
        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        // Equals
        public override bool Equals(object? obj)
        {
            return obj is Size other && Equals(other);
        }

        // GetHashCode
        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        // Unit
        public static Size Unit { get; } = new(1, 1);

        // Width
        public int Width { get; } = width;
    }
}
