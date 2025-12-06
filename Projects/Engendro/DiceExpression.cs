using System;
using System.Text.RegularExpressions;

namespace Engendro
{
    /// <summary>
    /// DiceExpression
    /// </summary>
    public class DiceExpression
    {
        private static readonly Random random = new();

        #region Constructor

        // Constructor
        public DiceExpression(string expression)
        {
            if (int.TryParse(expression, out int value))
            {
                IsFixedValue = true;
                FixedValue = value;
            }
            else if (TryParse(expression, out int diceCount, out int diceSides, out int modifier))
            {
                DiceCount = diceCount;
                DiceSides = diceSides;
                Modifier = modifier;
            }
            else
                throw new ArgumentException($"Invalid dice expression: {expression}");
        }

        #endregion

        #region Private members

        // TryParse
        private static bool TryParse(string expression, out int diceCount, out int diceSides, out int modifier)
        {
            diceCount = 0;
            diceSides = 0;
            modifier = 0;

            var match = Regex.Match(expression.Trim(), @"^(\d*)d(\d+)([+-]\d+)?$", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new ArgumentException("Invalid dice expression. Use formats like '2d6+1' or 'd8-2'.");

            diceCount = string.IsNullOrEmpty(match.Groups[1].Value) ? 1 : int.Parse(match.Groups[1].Value);
            diceSides = int.Parse(match.Groups[2].Value);
            modifier = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

            if (diceCount <= 0 || diceSides <= 0)
                return false;

            return true;
        }

        #endregion

        // Dice4
        public static DiceExpression Dice4 { get; } = new("d4");

        // Dice6
        public static DiceExpression Dice6 { get; } = new("d6");

        // Dice8
        public static DiceExpression Dice8 { get; } = new("d8");

        // Dice10
        public static DiceExpression Dice10 { get; } = new("d10");

        // Dice12
        public static DiceExpression Dice12 { get; } = new("d12");

        // Dice20
        public static DiceExpression Dice20 { get; } = new("d20");

        // Dice100
        public static DiceExpression Dice100 { get; } = new("d100");

        // DiceCount
        public int DiceCount { get; }

        // DiceSides
        public int DiceSides { get; }

        // FixedValue
        public int FixedValue { get; }

        // GetValueRangeAsString
        public string GetValueRangeAsString(int modifier = 0)
        {
            var result = $"{MinimumValue + modifier}";
            if (!IsFixedValue)
                result += $"-{MaximumValue + modifier}";

            return result;
        }

        // IsFixedValue
        public bool IsFixedValue { get; }

        // MaximumValue
        public int MaximumValue => IsFixedValue ? FixedValue : (DiceCount * DiceSides) + Modifier;

        // MinimumValue
        public int MinimumValue => IsFixedValue ? FixedValue : (DiceCount * 1) + Modifier;

        // Modifier
        public int Modifier { get; }

        // Roll
        public int Roll()
        {
            if (IsFixedValue)
                return FixedValue;

            int total = 0;

            for (int i = 0; i < DiceCount; i++)
            {
                total += random.Next(1, DiceSides + 1);
            }

            return total + Modifier;
        }

        // ToString
        public override string ToString()
        {
            if (IsFixedValue)
                return FixedValue.ToString();
            else
                return $"{DiceCount}d{DiceSides}{(Modifier >= 0 ? "+" : "")}{Modifier}";
        }

        // TryParse
        public static bool TryParse(string expression, out DiceExpression? diceRoll)
        {
            if (int.TryParse(expression, out int value))
            {
                diceRoll = new DiceExpression(expression);
                return true;
            }

            if (TryParse(expression, out _, out _, out _))
            {
                diceRoll = new DiceExpression(expression);
                return true;
            }
            else
            {
                diceRoll = null;
                return false;
            }
        }
    }
}