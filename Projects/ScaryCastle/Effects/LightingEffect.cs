using Engendro;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ScaryCastle.Effects
{
    /// <summary>
    /// LightingEffect
    /// </summary>
    public sealed class LightingEffect : ShaderEffect
    {
        // Constrcutor
        public LightingEffect()
            : base("Shaders/Lighting")
        {
            LightMask = Effect.Parameters["lightMask"] ?? throw new InvalidOperationException();
        }

        // LightMask
        public EffectParameter LightMask { get; }
    }
}
