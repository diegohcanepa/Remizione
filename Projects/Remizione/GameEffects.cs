using Remizione.Effects;

namespace Remizione
{
    /// <summary>
    /// GameEffects
    /// </summary>
    public sealed class GameEffects
    {
        // Constructor
        internal GameEffects(RemizioneGame game)
        {
            CRT = new CRTEffect(game);
            ColorReduction = new ColorReductionEffect(game);
            ColorSaturation = new ColorSaturationEffect(game);
            Lighting = new LightingEffect(game);
        }

        // ColorReduction
        public ColorReductionEffect ColorReduction { get; }

        // ColorSaturation
        public ColorSaturationEffect ColorSaturation { get; }

        // CRT
        public CRTEffect CRT { get; }

        // Lighting
        public LightingEffect Lighting { get; }
    }
}
