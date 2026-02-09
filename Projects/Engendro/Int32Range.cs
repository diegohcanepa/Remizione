using System;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// Int32Range
    /// </summary>
    public readonly struct Int32Range : IRange<int>, IEquatable<Int32Range>
    {
        #region Constructors

        // Constructor
        public Int32Range(int value)
        {
            Minimum = value;
            Maximum = value;
        }

        // Constructor
        public Int32Range(int minimum, int maximum)
        {
            if (minimum > maximum)
                throw new ArgumentOutOfRangeException(nameof(minimum), "The minimum value cannot be greater than the maximum value.");

            Minimum = minimum;
            Maximum = maximum;
        }

        #endregion

        // Clamp
        public int Clamp(int value)
        {
            if (value < Minimum)
                value = Minimum;
            else if (value > Maximum)
                value = Maximum;

            return value;
        }

        // Contains
        public bool Contains(int value)
        {
            return value >= Minimum && value <= Maximum;
        }

        // Delta
        public int Delta => Maximum - Minimum;

        // Equals
        public override bool Equals(object? obj)
        {
            return obj is Int32Range range && Equals(range);
        }

        // Equals
        public bool Equals(Int32Range other)
        {
            return (Minimum == other.Minimum) && (Maximum == other.Maximum);
        }

        // Empty
        public static Int32Range Empty { get; } = new Int32Range(0);

        // GetHashCode
        public override int GetHashCode()
        {
            return Minimum ^ Maximum;
        }

        // GetRandomValue
        public int GetRandomValue()
        {
            return GetRandomValue(Random.Shared);
        }

        // GetRandomValue
        public int GetRandomValue(Random random)
        {
            return random.Next(Minimum, Maximum + 1);
        }

        // IsEmpty
        public bool IsEmpty => Minimum == 0 && Maximum == 0;

        // Maximum
        public int Maximum { get; }

        // Minimum
        public int Minimum { get; }

        // Parse
        public static Int32Range Parse(string value)
        {
            return TryParse(value, out var range) ? range : throw new FormatException(nameof(value));
        }

        // Separator
        public static readonly string Separator = "|";

        // ToString
        public override string ToString()
        {
            return $"({Minimum}-{Maximum})";
        }

        // TryParse
        public static bool TryParse(string value, out Int32Range result)
        {
            result = Empty;

            // Empty value
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Single value
            if (int.TryParse(value, out var singleValue))
            {
                result = new Int32Range(singleValue);
                return true;
            }

            // Two values
            var values = value.Split(Separator);
            if (values.Length != 2)
                return false;

            // Min
            if (values[0] == "..")
                values[0] = int.MinValue.ToString(CultureInfo.InvariantCulture);

            // Max
            if (values[1] == "..")
                values[1] = int.MaxValue.ToString(CultureInfo.InvariantCulture);

            if (int.TryParse(values[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var min) &&
                int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var max))
            {
                result = new Int32Range(min, max);
                return true;
            }
            else
            {
                return false;
            }
        }

        // ==
        public static bool operator ==(Int32Range range1, Int32Range range2)
        {
            return range1.Equals(range2);
        }

        // !=
        public static bool operator !=(Int32Range range1, Int32Range range2)
        {
            return !range1.Equals(range2);
        }
    }
}
