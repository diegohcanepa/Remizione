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
        private readonly List<Debris> debrisList = [];

        // Constructor
        public Guts(GameSession session, bool bloodStain, int amount, Vector2 scale, IList<AtlasImage>? extraImages)
            : base(session, string.Empty)
        {
            Atlas = Atlases.Environment;
            DefaultImageName = Atlases.Environment.GutStains[Random.Shared.Next(0, 2)].Name;
            PivotOrigin = RectanglePoint.Center;
            Opacity = bloodStain ? .8f : 0;

            // Guts pieces
            var guts = Math.Min(amount, Atlases.Environment.Guts.Count);
            if (guts > 0)
            {
                for (var i = 0; i < guts; i++)
                {
                    var debris = Session.ObjectPools.Debris.Get();
                    debris.Image = Atlases.Environment.Guts[i];
                    debris.Scale = scale;
                    debrisList.Add(debris);
                }
            }

            // Extra pieces
            if (extraImages != null)
            {
                for (var i = 0; i < extraImages.Count; i++)
                {
                    var debris = Session.ObjectPools.Debris.Get();
                    debris.Image = extraImages[i];
                    debris.Scale = scale;
                    debrisList.Add(debris);
                }
            }

            RenderLayer = RenderLayer.OverBackground;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            for (var i = 0; i < debrisList.Count; i++)
            {
                debrisList[i].Draw(gameTime);
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            for (var i = 0; i < debrisList.Count; i++)
            {
                debrisList[i].Launch(this);
                RenderLayer = RenderLayer.OverBackground;
            }

            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicOut, 0, 1, 400);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            for (var i = 0; i < debrisList.Count; i++)
            {
                debrisList[i].Update(gameTime);
            }
        }

        #endregion
    }
}
