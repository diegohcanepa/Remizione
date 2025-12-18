using Engendro;

namespace ScaryCastle.Effects
{
    /// <summary>
    /// CRTEffect
    /// </summary>
    public sealed class CRTEffect : ShaderEffect
    {
        // Constructor
        public CRTEffect(EngendroGame game)
            : base(game, "Effects/CRT")
        {
            //Effect.Parameters["ScreenSize"].SetValue(new Vector2(1920, 1080));
            //Effect.Parameters["CurvatureAmount"].SetValue(0.1f);
            //Effect.Parameters["ScanlineIntensity"].SetValue(0.5f);
            //Effect.Parameters["ChromaticAberrationAmount"].SetValue(0.005f);
        }
    }
}
