using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engendro
{
    /// <summary>
    /// Renderer2D
    /// </summary>
    public sealed partial class Renderer2D : IDisposable
    {
        #region Private fields

        private int index;
        private readonly RenderTarget2D[] renderTargets = new RenderTarget2D[2];

        #endregion

        #region Constructor

        // Constructor
        public Renderer2D()
        {
            // Render targets
            renderTargets[0] = CreateRenderTarget(RenderTargetUsage.PreserveContents);
            renderTargets[1] = CreateRenderTarget(RenderTargetUsage.PreserveContents);

            renderTargets[0].Name = "RenderTarget 0";
            renderTargets[1].Name = "RenderTarget 1";

            LightTarget = CreateRenderTarget(RenderTargetUsage.PreserveContents);
            ShadowTarget = CreateRenderTarget(RenderTargetUsage.PreserveContents);
        }

        #endregion

        #region Private members

        // CreateRenderTarget
        private static RenderTarget2D CreateRenderTarget(RenderTargetUsage usage)
        {
            var vp = EngendroGame.Instance.ViewportAdapter;
            return new(EngendroGame.Instance.GraphicsDevice, vp.DisplayWidth, vp.DisplayHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, usage);
        }

        #endregion

        // CurrentTarget
        public RenderTarget2D CurrentTarget => renderTargets[index];

        // Dispose
        public void Dispose()
        {
            for (var i = 0; i < renderTargets.Length; i++)
            {
                renderTargets[i]?.Dispose();
            }

            LightTarget.Dispose();
            ShadowTarget.Dispose();
        }

        // LightTarget
        public RenderTarget2D LightTarget { get; }

        // PreviousTarget
        public RenderTarget2D PreviousTarget => index == 0 ? renderTargets[1] : renderTargets[0];

        // ShadowTarget
        public RenderTarget2D ShadowTarget { get; }

        // Swap
        public RenderTarget2D Swap()
        {
            return Swap(false);
        }

        // Swap
        public RenderTarget2D Swap(bool clear)
        {
            index = index == 0 ? 1 : 0;

            EngendroGame.Instance.GraphicsDevice.SetRenderTarget(CurrentTarget);
            if (clear)
                EngendroGame.Instance.GraphicsDevice.Clear(Color.Black);

            return CurrentTarget;
        }
    }
}
