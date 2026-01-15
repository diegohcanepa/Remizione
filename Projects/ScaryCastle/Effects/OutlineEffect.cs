using Engendro;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle.Effects
{
    /// <summary>
    /// OutlineEffect
    /// </summary>
    public sealed class OutlineEffect : ShaderEffect
    {
        // Constrcutor
        public OutlineEffect(EngendroGame game)
            : base(game, "Effects/Outline")
        {
            Color = Effect.Parameters["outlineColor"];
            TextureSize = Effect.Parameters["textureSize"];
            Thickness = Effect.Parameters["outlineThickness"];
        }

        // Color
        public EffectParameter Color { get; }

        // TextureSize
        public EffectParameter TextureSize { get; }

        // Thickness
        public EffectParameter Thickness { get; }
    }
}
