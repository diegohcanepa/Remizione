using Engendro;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione.Effects
{
    /// <summary>
    /// SepiaShaderEffect
    /// </summary>
    public sealed class SepiaShaderEffect : ShaderEffect
    {
        private readonly EffectParameter intensity;

        // Constrcutor
        public SepiaShaderEffect()
            : base("Sepia")
        {
            intensity = Effect.Parameters[nameof(intensity)] ?? throw new InvalidOperationException();
            intensity.SetValue(0);
        }

        // Intensity
        public float Intensity
        {
            get => intensity.GetValueSingle();
            set
            {
                if (value != intensity.GetValueSingle())
                    intensity.SetValue(value);
            }
        }
    }
}
