using Engendro;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione.Effects
{
    /// <summary>
    /// SepiaEffect
    /// </summary>
    public sealed class SepiaEffect : ShaderEffect
    {
        private readonly EffectParameter intensity;

        // Constrcutor
        public SepiaEffect()
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
