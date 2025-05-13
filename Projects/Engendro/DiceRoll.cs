using System;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// DiceRoll
    /// </summary>
    public sealed class DiceRoll
    {
        #region Constructor

        // Constructor
        public DiceRoll(Dice dice, int diceCount, int modifier)
        {
            // Check
            if (diceCount < 0)
                throw new ArgumentOutOfRangeException(nameof(diceCount), "Value must be equal or greater than zero.");

            this.Dice = dice;
            this.DiceCount = diceCount;
            this.Modifier = modifier;

            Initialize();
        }

        // Constructor
        public DiceRoll(string expression)
        {
            // Try to parse expression
            if (!TryParse(expression, out var dice, out var diceCount, out var modifier))
            {
                throw new ArgumentException($"'{expression}' is not a valid expression.", nameof(expression));
            }

            this.Dice = dice;
            this.DiceCount = diceCount;
            this.Modifier = modifier;

            Initialize();
        }

        #endregion

        #region Private members

        // Initialize
        private void Initialize()
        {
            AsExpression = $"{DiceCount}d{Dice.Sides}";
            if (Modifier != 0)
            {
                AsExpression += (Modifier < 0 ? "-" : "+") + Modifier.ToString(CultureInfo.InvariantCulture);
            }

            AsRange = MinimumValue == MaximumValue ? MinimumValue.ToString("+#;-#;0", CultureInfo.InvariantCulture) : $"{MinimumValue}-{MaximumValue}";
        }

        // TryParse
        private static bool TryParse(string expression, out Dice dice, out int diceCount, out int modifier)
        {
            dice = Dice.Empty;
            diceCount = 0;
            modifier = 0;

            if (string.IsNullOrWhiteSpace(expression))
                return true;

            expression = expression.Trim();
            ReadOnlySpan<char> span = expression.AsSpan();

            // Single number?
            if (int.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out modifier))
            {
                dice = Dice.Empty;
                return true;
            }

            // "d"
            int dIndex = span.IndexOf('d');
            if (dIndex == -1)
                return false;

            diceCount = dIndex == 0 ? 1 : int.Parse(span.Slice(0, dIndex), CultureInfo.InvariantCulture);

            // Modifier
            int signIndex = span.IndexOfAny("+-");
            if (signIndex > 0)
            {
                modifier = int.Parse(span.Slice(signIndex), CultureInfo.InvariantCulture);
            }

            // Dice kind
            ReadOnlySpan<char> diceName = signIndex == -1 ? span.Slice(dIndex) : span.Slice(dIndex, signIndex - dIndex);
            dice = DiceBag.GetDice(diceName.ToString());

            return dice != null;
        }

        #endregion

        // AsExpression
        public string AsExpression { get; private set; } = string.Empty;

        // AsRange
        public string AsRange { get; private set; } = string.Empty;

        // Dice
        public Dice Dice { get; }

        // DiceCount
        public int DiceCount { get; }

        // IsEmpty
        public bool IsEmpty => DiceCount == 0;

        // Empty
        public static DiceRoll Empty { get; } = new DiceRoll("0");

        // LastRoll
        public int LastRoll { get; private set; }

        // MaximumValue
        public int MaximumValue => (Dice.Sides * DiceCount) + Modifier;

        // MinimumValue
        public int MinimumValue => (DiceCount * (Dice.Sides == 0 ? 0 : 1)) + Modifier;

        // Modifier
        public int Modifier { get; }

        // Roll
        public int Roll()
        {
            var result = 0;
            for (var i = 0; i < DiceCount; i++)
            {
                result += Dice.Roll();
            }

            LastRoll = result + Modifier;

            return LastRoll;
        }

        // ToString
        public override string ToString()
        {
            return AsExpression;
        }

        // TryParse
        public static bool TryParse(string expression, out DiceRoll? diceRoll)
        {
            diceRoll = null;

            if (TryParse(expression, out var dice, out var diceCount, out var modifier))
            {
                diceRoll = new DiceRoll(dice, diceCount, modifier);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
