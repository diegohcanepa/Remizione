using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Guts
    /// </summary>
    public class Guts : GameThing
    {
        private readonly ShatterPiece[] pieces;

        // Constructor
        public Guts(GameSession session, int amount)
            : base(session, string.Empty)
        {
            Atlas = Atlases.Environment;
            DefaultImageName = $"GutStain{Randomizer.Next(1, 2)}";
            PivotOrigin = RectanglePoint.Center;
            Opacity = .3f;

            pieces = new ShatterPiece[Math.Min(amount, Atlases.Environment.Guts.Count)];
            if (pieces.Length > 0)
            {
                for (var i = 0; i < pieces.Length; i++)
                {
                    pieces[i] = new(this, Atlases.Environment.Guts[i])
                    {
                        Opacity = Randomizer.Next(.7f, 1)
                    };
                }
            }

            RenderLayer = RenderLayer.Background;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            for (var i = 0; i < pieces.Length; i++)
            {
                pieces[i].Draw(gameTime);
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            for (var i = 0; i < pieces.Length; i++)
            {
                pieces[i].Launch();
                RenderLayer = RenderLayer.Background;
            }

            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicOut, 0, 1, 400);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            for (var i = 0; i < pieces.Length; i++)
            {
                pieces[i].Update(gameTime);
            }
        }

        #endregion
    }
}
