using Microsoft.Xna.Framework.Graphics;

namespace Engendro
{
    /// <summary>
    /// SpriteBatchExtensions
    /// </summary>
    public static class SpriteBatchExtensions
    {
        // Begin
        public static void Begin(this SpriteBatch spriteBatch, Camera camera)
        {
            Begin(spriteBatch, camera, SamplerState.PointClamp, null);
        }

        // Begin
        public static void Begin(this SpriteBatch spriteBatch, Camera camera, SamplerState samplerState)
        {
            Begin(spriteBatch, camera, samplerState, null);
        }

        // Begin
        public static void Begin(this SpriteBatch spriteBatch, Camera camera, SamplerState samplerState, Effect? effect)
        {
            Begin(spriteBatch, camera, samplerState, BlendState.AlphaBlend, effect);
        }

        // Begin
        public static void Begin(this SpriteBatch spriteBatch, Camera camera, SamplerState samplerState, BlendState blendState, Effect? effect)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState: blendState, samplerState: samplerState, transformMatrix: camera.GetTransformationMatrix(), effect: effect);
        }
    }
}
