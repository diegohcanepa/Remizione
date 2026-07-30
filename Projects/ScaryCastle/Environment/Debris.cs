using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Debris
    /// </summary>
    public sealed class Debris : GameThing
    {
        private readonly List<DebrisPiece> parts = [];

        // Constructor
        public Debris(GameSession session, string imageName, int amount, Vector2 scale, IList<AtlasImage> pieces, bool shadow)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.DefaultImageName = imageName;
            this.PivotOrigin = RectanglePoint.Center;
            this.Opacity = .6f;

            if (amount > 0 && pieces.Count > 0)
            {
                for (var i = 0; i < amount; i++)
                {
                    var debris = Session.ObjectPools.DebrisPieces.Get();
                    debris.Image = pieces.GetRandomItem();
                    debris.Opacity = .75f;
                    debris.Scale = scale;
                    debris.Shadow = shadow;
                    parts.Add(debris);
                }
            }

            RenderLayer = RenderLayer.OverBackground;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

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
