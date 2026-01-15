using ScaryCastle.Effects;

namespace ScaryCastle
{
    /// <summary>
    /// GameEffects
    /// </summary>
    public sealed class GameEffects
    {
        // Constructor
        internal GameEffects(ScaryCastleGame game)
        {
            CRT = new CRTEffect(game);
            ColorReduction = new ColorReductionEffect(game);
            ColorSaturation = new ColorSaturationEffect(game);
            Lighting = new LightingEffect(game);
            Outline = new OutlineEffect(game);
        }

        // ColorReduction
        public ColorReductionEffect ColorReduction { get; }

        // ColorSaturation
        public ColorSaturationEffect ColorSaturation { get; }

        // CRT
        public CRTEffect CRT { get; }

        // Lighting
        public LightingEffect Lighting { get; }

        // Outline
        public OutlineEffect Outline { get; }
    }
}
