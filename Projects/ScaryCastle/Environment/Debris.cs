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
        private readonly List<DebrisPiece> pieces = [];

        // Constructor
        public Debris(GameSession session, string defaultImageName, Vector2 scale, IList<AtlasImage> pieces, int amount, bool shadow)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.DefaultImageName = defaultImageName;
            this.PivotOrigin = RectanglePoint.Center;
            this.Opacity = .6f;

            if (amount > 0 && pieces.Count > 0)
            {
                for (var i = 0; i < amount; i++)
                {
                    var debrisPiece = Session.ObjectPools.DebrisPieces.Get();
                    debrisPiece.Image = pieces.GetRandomItem();
                    debrisPiece.Opacity = .75f;
                    debrisPiece.Scale = scale;
                    debrisPiece.Shadow = shadow;
                    this.pieces.Add(debrisPiece);
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

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            for (var i = 0; i < pieces.Count; i++)
            {
                Session.ObjectPools.DebrisPieces.Return(pieces[i]);
            }
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
