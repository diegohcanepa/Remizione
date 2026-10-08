using Engendro;

namespace Remizione.Effects
{
    /// <summary>
    /// CRTEffect
    /// </summary>
    public sealed class CRTEffect : ShaderEffect
    {
        private const string param_chromaticAberration = "ChromaticAberration";
        private const string param_curvature = "Curvature";
        private const string param_scanlineCount = "ScanlineCount";
        private const string param_scanlineIntensity = "ScanlineIntensity";

        // Constructor
        public CRTEffect()
            : base("Shaders/CRT")
        {
            Effect.Parameters[param_curvature].SetValue(0);
            Effect.Parameters[param_scanlineCount].SetValue(80);
            Effect.Parameters[param_scanlineIntensity].SetValue(0.04f);
            Effect.Parameters[param_chromaticAberration].SetValue(.0005f);
        }
    }
}
