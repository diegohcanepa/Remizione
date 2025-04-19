using Engendro;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione.Effects
{
    /// <summary>
    /// LightingEffect
    /// </summary>
    public sealed class LightingEffect : ShaderEffect
    {
        // Constrcutor
        public LightingEffect(EngendroGame game)
            : base(game, "Effects/Lighting")
        {
            LightMask = Effect.Parameters["lightMask"] ?? throw new InvalidOperationException();
        }

        // LightMask
        public EffectParameter LightMask { get; }
    }
}
