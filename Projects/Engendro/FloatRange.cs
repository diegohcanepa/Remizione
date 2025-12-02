using System;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// FloatRange
    /// </summary>
    public readonly struct FloatRange : IRange<float>, IEquatable<FloatRange>
    {
        #region Constructor

        // Constructor
        public FloatRange(float value)
        {
            Minimum = value;
            Maximum = value;
        }

        // Constructor
        public FloatRange(float minimum, float maximum)
        {
            if (minimum > maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(minimum), "The minimum value cannot be greater than the maximum value.");
            }

            Minimum = minimum;
            Maximum = maximum;
        }

        #endregion

        // Clamp
        public float Clamp(float value)
        {
            if (value < Minimum)
            {
                value = Minimum;
            }
            else if (value > Maximum)
            {
                value = Maximum;
            }

            return value;
        }

        // Contains
        public bool Contains(float value)
        {
            return value >= Minimum && value <= Maximum;
        }

        // Delta
        public float Delta => Maximum - Minimum;

        // Equals
        public override bool Equals(object? obj)
        {
            return obj is FloatRange range && Equals(range);
        }

        // Equals
        public bool Equals(FloatRange other)
        {
            return (Minimum == other.Minimum) && (Maximum == other.Maximum);
        }

        // Empty
        public static FloatRange Empty { get; } = new FloatRange(0);

        // GetHashCode
        public override int GetHashCode()
        {
            return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
        }

        // IsEmpty
        public bool IsEmpty => Minimum == 0 && Maximum == 0;

        // Maximum
        public float Maximum { get; }

        // Minimum
        public float Minimum { get; }

        // Parse
        public static FloatRange Parse(string value)
        {
            return TryParse(value, out var range) ? range : throw new FormatException(nameof(value));
        }

        // RandomValue
        public float RandomValue(Random random)
        {
            return RandomHelper.Next(random, Minimum, Maximum);
        }

        // Separator
        public static readonly string Separator = "|";

        // ToString
        public override string ToString()
        {
            return $"({Minimum}-{Maximum})";
        }

        // TryParse
        public static bool TryParse(string value, out FloatRange result)
        {
            result = Empty;

            // Empty value
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            // Single value
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var singleValue))
            {
                result = new FloatRange(singleValue);
                return true;
            }

            // Two values
            var values = value.Split(Separator);
            if (values.Length != 2)
            {
                return false;
            }

            // Min
            if (values[0] == "..")
                values[0] = float.MinValue.ToString(CultureInfo.InvariantCulture);

            // Max
            if (values[1] == "..")
                values[1] = float.MaxValue.ToString(CultureInfo.InvariantCulture);

            if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var min) &&
                float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var max))
            {
                result = new FloatRange(min, max);
                return true;
            }
            else
            {
                return false;
            }
        }

        // ==
        public static bool operator ==(FloatRange range1, FloatRange range2)
        {
            return range1.Equals(range2);
        }

        // !=
        public static bool operator !=(FloatRange range1, FloatRange range2)
        {
            return !range1.Equals(range2);
        }
    }
}
