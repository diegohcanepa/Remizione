using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Guts
    /// </summary>
    public class Guts : GameThing
    {
        private readonly List<DebrisPiece> parts = [];

        // Constructor
        public Guts(GameSession session, int amount, Vector2 scale, IList<AtlasImage>? extraImages)
            : base(session, string.Empty)
        {
            PivotOrigin = RectanglePoint.Center;

            // Guts pieces
            var guts = Math.Min(amount, Atlases.Environment.Guts.Count);
            if (guts > 0)
            {
                for (var i = 0; i < guts; i++)
                {
                    var debris = Session.ObjectPools.DebrisPieces.Get();
                    debris.Image = Atlases.Environment.Guts[i];
                    debris.Opacity = .75f;
                    debris.Scale = scale;
                    parts.Add(debris);
                }
            }

            // Extra pieces
            if (extraImages != null)
            {
                for (var i = 0; i < extraImages.Count; i++)
                {
                    var debris = Session.ObjectPools.DebrisPieces.Get();
                    debris.Image = extraImages[i];
                    debris.Scale = scale;
                    debris.Shadow = true;
                    parts.Add(debris);
                }
            }

            RenderLayer = RenderLayer.OverBackground;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            //base.OnDraw(gameTime);

            for (var i = 0; i < parts.Count; i++)
            {
                parts[i].Draw(gameTime);
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            for (var i = 0; i < parts.Count; i++)
            {
                parts[i].Launch(this);
                RenderLayer = RenderLayer.OverBackground;
            }

            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicOut, 0, 1, 400);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            for (var i = 0; i < parts.Count; i++)
            {
                parts[i].Update(gameTime);
            }
        }

        #endregion
    }
}
