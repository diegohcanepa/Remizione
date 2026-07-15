using Engendro;

namespace ScaryCastle.Effects
{
    /// <summary>
    /// CRTEffect
    /// </summary>
    public sealed class CRTEffect : ShaderEffect
    {
        private bool isInitialized;
        private const string param_chromaticAberration = "ChromaticAberration";
        private const string param_curvature = "Curvature";
        private const string param_scanlineIntensity = "ScanlineIntensity";

        // Constructor
        public CRTEffect()
            : base("Shaders/CRT")
        {
        }

        // ChromaticAberration
        public float ChromaticAberration
        {
            get => Effect.Parameters[param_chromaticAberration].GetValueSingle();
            set => Effect.Parameters[param_chromaticAberration].SetValue(value);
        }

        // MonitorStyle
        public bool MonitorStyle
        {
            get;
            set
            {
                if (value != field || !isInitialized)
                {
                    field = value;
                    Reset();
                }
            }
        }

        // Reset
        public void Reset()
        {
            if (MonitorStyle)
            {
                Effect.Parameters[param_curvature].SetValue(0);
                Effect.Parameters[param_scanlineIntensity].SetValue(0.05f);
                Effect.Parameters[param_chromaticAberration].SetValue(0.0004f);

                //Effect.Parameters[param_curvature].SetValue(0.12f);
                //Effect.Parameters[param_scanlineIntensity].SetValue(0.09f);
                //Effect.Parameters[param_chromaticAberration].SetValue(0.0007f);
            }
            else
            {
                Effect.Parameters[param_curvature].SetValue(0);
                Effect.Parameters[param_scanlineIntensity].SetValue(0.03f);
                Effect.Parameters[param_chromaticAberration].SetValue(0.0004f);
            }

            isInitialized = true;
        }
    }
}
