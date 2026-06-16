using ScaryCastle.Effects;

namespace ScaryCastle
{
    /// <summary>
    /// ShaderEffects
    /// </summary>
    public sealed class ShaderEffects
    {
        // ColorReduction
        public ColorReductionEffect ColorReduction { get; } = new();

        // ColorSaturation
        public ColorSaturationEffect ColorSaturation { get; } = new();

        // CRT
        public CRTEffect CRT { get; } = new();

        // Lighting
        public LightingEffect Lighting { get; } = new();
    }
}
