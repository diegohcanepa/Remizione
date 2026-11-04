using Engendro;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.Effects
{
    /// <summary>
    /// GrainEffect
    /// </summary>
    public sealed class GrainEffect : ShaderEffect
    {
        // Constructor
        public GrainEffect()
            : base("Grain")
        {
            CameraOffset = Effect.Parameters["CameraOffset"];
        }

        // CameraOffset
        public EffectParameter CameraOffset { get; }
    }
}
