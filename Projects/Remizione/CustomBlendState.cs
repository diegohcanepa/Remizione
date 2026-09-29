using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// CustomBlendState
    /// </summary>
    internal static class CustomBlendState
    {
        // LightMax
        // Evita que las luces suaves saturen entre sí.
        internal static BlendState LightMax { get; } = new()
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Max,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.Max
        };

        // LightSoftAdditive
        internal static BlendState LightSoftAdditive { get; } = new()
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceColor,
            ColorBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.InverseSourceAlpha,
            AlphaBlendFunction = BlendFunction.Add
        };

        // SubtractivePlayer
        internal static BlendState SubtractivePlayer { get; } = new()
        {
            ColorSourceBlend = Blend.InverseDestinationColor,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.InverseDestinationAlpha,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.Add
        };
    }
}
