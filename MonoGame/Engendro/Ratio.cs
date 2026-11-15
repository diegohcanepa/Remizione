using System;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// Ratio
    /// </summary>
    public readonly struct Ratio(float value)
    {
        #region Operators

        public static implicit operator float(Ratio r)
        {
            return r.Value;
        }

        public static implicit operator Ratio(float v)
        {
            return new(v);
        }

        #endregion

        // Roll
        public bool Roll()
        {
            return Roll(Random.Shared);
        }

        // Roll
        public bool Roll(Random random)
        {
            return random.NextDouble() < Value;
        }

        // ToString
        public override string ToString()
        {
            return Value.ToString(CultureInfo.CurrentCulture);
        }

        // Value
        public float Value { get; } = Math.Clamp(value, 0f, 1f);
    }
}

