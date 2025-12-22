using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// Renderer2D
    /// </summary>
    public sealed partial class Renderer2D : IDisposable
    {
        #region Private fields

        private readonly List<RenderTarget2D> auxiliaryTargetList = [];
        private readonly EngendroGame game;
        private int index;
        private readonly RenderTarget2D[] renderTargets = new RenderTarget2D[2];

        #endregion

        #region Constructor

        // Constructor
        public Renderer2D(EngendroGame game)
        {
            this.game = game;

            // Render targets
            renderTargets[0] = CreateRenderTarget(RenderTargetUsage.PreserveContents);
            renderTargets[1] = CreateRenderTarget(RenderTargetUsage.PreserveContents);

            renderTargets[0].Name = "RenderTarget 0";
            renderTargets[1].Name = "RenderTarget 1";

            // Create auxiliary targets
            for (var i = 0; i < 4; i++)
            {
                auxiliaryTargetList.Add(CreateRenderTarget(RenderTargetUsage.PreserveContents));
            }

            AuxiliaryTargets = new ReadOnlyCollection<RenderTarget2D>(auxiliaryTargetList);
        }

        #endregion

        #region Private members

        // CreateRenderTarget
        private RenderTarget2D CreateRenderTarget(RenderTargetUsage usage)
        {
            return new RenderTarget2D(game.GraphicsDevice, game.ViewportAdapter.DisplayWidth, game.ViewportAdapter.DisplayHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, usage);
        }

        #endregion

        // AuxiliaryTargets
        public ReadOnlyCollection<RenderTarget2D> AuxiliaryTargets { get; }

        // CurrentTarget
        public RenderTarget2D CurrentTarget => renderTargets[index];

        // Dispose
        public void Dispose()
        {
            for (var i = 0; i < renderTargets.Length; i++)
            {
                renderTargets[i]?.Dispose();
            }

            for (var i = 0; i < auxiliaryTargetList.Count; i++)
            {
                auxiliaryTargetList[i]?.Dispose();
            }
        }

        // PreviousTarget
        public RenderTarget2D PreviousTarget => index == 0 ? renderTargets[1] : renderTargets[0];

        // Swap
        public RenderTarget2D Swap()
        {
            return Swap(false);
        }

        // Swap
        public RenderTarget2D Swap(bool clear)
        {
            index = index == 0 ? 1 : 0;
            game.GraphicsDevice.SetRenderTarget(CurrentTarget);
            if (clear)
                game.GraphicsDevice.Clear(Color.Black);

            return CurrentTarget;
        }
    }
}
