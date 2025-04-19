namespace Engendro
{
    /// <summary>
    /// DiceBag
    /// </summary>
    public static class DiceBag
    {
        // Dice4
        public static Dice Dice4 { get; } = new Dice(4);

        // Dice6
        public static Dice Dice6 { get; } = new Dice(6);

        // Dice8
        public static Dice Dice8 { get; } = new Dice(8);

        // Dice10
        public static Dice Dice10 { get; } = new Dice(10);

        // Dice12
        public static Dice Dice12 { get; } = new Dice(12);

        // Dice20
        public static Dice Dice20 { get; } = new Dice(20);

        // Dice100
        public static Dice Dice100 { get; } = new Dice(100);

        // GetDice 
        public static Dice GetDice(DiceName diceName)
        {
            return diceName switch
            {
                DiceName.d4 => Dice4,
                DiceName.d6 => Dice6,
                DiceName.d8 => Dice8,
                DiceName.d10 => Dice10,
                DiceName.d12 => Dice12,
                DiceName.d20 => Dice20,
                DiceName.d100 => Dice100,
                _ => Dice.Empty,
            };
        }

        // GetDice
        public static Dice GetDice(string name)
        {
            return name switch
            {
                "d4" => Dice4,
                "d6" => Dice6,
                "d8" => Dice8,
                "d10" => Dice10,
                "d12" => Dice12,
                "d20" => Dice20,
                "d100" => Dice100,
                _ => Dice.Empty,
            };
        }
    }
}
