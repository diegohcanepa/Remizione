using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engendro
{
    /// <summary>
    /// SpriteBatchExtensions
    /// </summary>
    public static class SpriteBatchExtensions
    {
        extension(SpriteBatch spriteBatch)
        {
            // Begin
            public void Begin(Camera camera)
            {
                Begin(spriteBatch, camera, SamplerState.PointClamp, null);
            }

            // Begin
            public void Begin(Camera camera, SamplerState samplerState)
            {
                Begin(spriteBatch, camera, samplerState, null);
            }

            // Begin
            public void Begin(Camera camera, SamplerState samplerState, Effect? effect)
            {
                Begin(spriteBatch, camera, samplerState, BlendState.AlphaBlend, effect, null);
            }

            // Begin
            public void Begin(Camera camera, SamplerState samplerState, BlendState? blendState, Effect? effect, RasterizerState? rasterizerState = null)
            {
                Begin(spriteBatch, camera.GetTransformationMatrix(), samplerState, blendState, effect, rasterizerState);
            }

            // Begin
            public void Begin(Matrix matrix, SamplerState samplerState, BlendState? blendState, Effect? effect, RasterizerState? rasterizerState = null)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, blendState: blendState, samplerState: samplerState, transformMatrix: matrix, effect: effect, rasterizerState: rasterizerState);
            }
        }
    }
}
