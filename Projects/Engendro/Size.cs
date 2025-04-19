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
        public static bool operator ==(Size left, Size right) => left.Equals(right);

        // != operator
        public static bool operator !=(Size left, Size right) => !left.Equals(right);

        // + operator
        public override string ToString() => $"{Width} × {Height}";

        #endregion

        // Empty
        public static Size Empty { get; } = new(0, 0);

        // Height
        public int Height { get; } = height;

        // Equals
        public bool Equals(Size other) => Width == other.Width && Height == other.Height;

        // Equals
        public override bool Equals(object? obj) => obj is Size other && Equals(other);

        // GetHashCode
        public override int GetHashCode() => HashCode.Combine(Width, Height);

        // Width
        public int Width { get; } = width;
    }
}
