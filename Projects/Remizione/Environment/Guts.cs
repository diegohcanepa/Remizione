using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Guts
    /// </summary>
    public class Guts : GameThing
    {
        private readonly List<ShatterPiece> pieces = [];

        // Constructor
        public Guts(GameSession session, int amount, Vector2 scale, IList<AtlasImage>? extraImages)
            : base(session, string.Empty)
        {
            Atlas = Atlases.Environment;
            DefaultImageName = $"GutStain{Random.Shared.Next(1, 3)}";
            PivotOrigin = RectanglePoint.Center;
            Opacity = .3f;

            // Guts pieces
            var guts = Math.Min(amount, Atlases.Environment.Guts.Count);
            if (guts > 0)
            {
                for (var i = 0; i < guts; i++)
                {
                    var piece = new ShatterPiece(session.Game, Atlases.Environment.Guts[i])
                    {
                        Scale = scale
                    };

                    pieces.Add(piece);
                }
            }

            // Extra pieces
            if (extraImages != null)
            {
                for (var i = 0; i < extraImages.Count; i++)
                {
                    pieces.Add(new ShatterPiece(session.Game, extraImages[i]) { });
                }
            }

            RenderLayer = RenderLayer.OverBackground;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Draw(gameTime);
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            
            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Launch(this);
                RenderLayer = RenderLayer.OverBackground;
            }

            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicOut, 0, 1, 400);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Update(gameTime);
            }
        }

        #endregion
    }
}
