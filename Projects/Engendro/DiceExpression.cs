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

        // Constructor
        public DiceExpression(string expression)
        {
            if (TryParse(expression, out int diceCount, out int diceSides, out int modifier))
            {
                DiceCount = diceCount;
                DiceSides = diceSides;
                Modifier = modifier;
            }
            else
                throw new ArgumentException($"Invalid dice expression: {expression}");
        }

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

        // MaximumValue
        public int MaximumValue => DiceCount * DiceSides + Modifier;

        // MinimumValue
        public int MinimumValue => DiceCount * 1 + Modifier;

        // Modifier
        public int Modifier { get; }

        // Roll
        public int Roll()
        {
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
            return $"{DiceCount}d{DiceSides}{(Modifier >= 0 ? "+" : "")}{Modifier}";
        }

        // TryParse
        public static bool TryParse(string expression, out int diceCount, out int diceSides, out int modifier)
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

        // TryParse
        public static bool TryParse(string expression, out DiceExpression? diceRoll)
        {
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