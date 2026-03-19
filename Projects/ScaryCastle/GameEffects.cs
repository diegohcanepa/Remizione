using ScaryCastle.Effects;

namespace ScaryCastle
{
    /// <summary>
    /// GameEffects
    /// </summary>
    public sealed class GameEffects
    {
        // ColorReduction
        public ColorReductionEffect ColorReduction { get; } = new();

        // ColorSaturation
        public ColorSaturationEffect ColorSaturation { get; } = new();

        // CRT
        public CRTEffect CRT { get; } = new();

        // Lighting
        public LightingEffect Lighting { get; } = new();

        // Outline
        public OutlineEffect Outline { get; } = new();
    }
}
