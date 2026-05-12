using Engendro;

namespace ScaryCastle.Effects
{
    /// <summary>
    /// CRTEffect
    /// </summary>
    public sealed class CRTEffect : ShaderEffect
    {
        private bool isInitialized;

        // Constructor
        public CRTEffect()
            : base("Effects/CRT")
        {
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            if (MonitorStyle)
            {
                Effect.Parameters["Curvature"].SetValue(0.12f);
                Effect.Parameters["ScanlineIntensity"].SetValue(0.07f);
                Effect.Parameters["ChromaticAberration"].SetValue(0.0007f);
            }
            else
            {
                Effect.Parameters["Curvature"].SetValue(0);
                Effect.Parameters["ScanlineIntensity"].SetValue(0.025f);
                Effect.Parameters["ChromaticAberration"].SetValue(0.0002f);
            }

            isInitialized = true;
        }

        #endregion

        // MonitorStyle
        public bool MonitorStyle
        {
            get;
            set
            {
                if (value != field || !isInitialized)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

    }
}
