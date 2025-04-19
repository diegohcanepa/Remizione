namespace Engendro
{
    /// <summary>
    /// Dice
    /// </summary>
    public sealed class Dice(int sides)
    {
        // Empty
        public static Dice Empty { get; } = new Dice(0);

        // Roll
        public int Roll() => Roll(0);

        // Roll
        public int Roll(int modifier) => Sides == 0 ? modifier : Randomizer.Next(1, Sides) + modifier;

        // Sides
        public int Sides { get; } = sides;
    }
}
