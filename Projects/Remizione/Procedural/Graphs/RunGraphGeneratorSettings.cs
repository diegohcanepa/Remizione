namespace Remizione.Procedural.Graphs
{
    // RunGraphGeneratorSettings
    public sealed class RunGraphGeneratorSettings
    {
        // MaxLength
        public int MaxLength { get; init; } = 3;

        // MaxSidePerRoom
        public int MaxSidePerRoom { get; init; } = 1;

        // MaxSidePerPath
        public int MaxSidePerPath { get; init; } = int.MaxValue;

        // MinLength
        public int MinLength { get; init; } = 2;

        // PathCount
        public int PathCount { get; init; } = 3;

        // Pools
        public Tags Pools { get; init; } = new([]);

        // Seed
        public int Seed { get; init; }

        // SideChancePercent
        public int SideChancePercent { get; init; } = 50;
    }
}
