using System;

namespace Engendro
{
    /// <summary>
    /// Ratio
    /// </summary>
    public readonly struct Ratio
    {
        // Constructor
        public Ratio(float value)
        {
            Value = Math.Clamp(value, 0f, 1f);
        }

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
            return Random.Shared.NextDouble() <= Value;
        }

        // Roll
        public bool Roll(Random random)
        {
            return random.NextDouble() <= Value;
        }

        // ToString
        public override string ToString()
        {
            return Value.ToString();
        }

        // Value
        public float Value { get; }
    }
}

