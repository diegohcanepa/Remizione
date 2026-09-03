using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Remains
    /// </summary>
    public sealed class Remains : GameThing
    {
        private int applyEffectTimer;
        private readonly IList<EffectDescriptor>? effects;
        private readonly List<RemainsPiece> pieces = [];

        // Constructor
        public Remains(GameSession session, string defaultImageName, Vector2 scale, IList<AtlasImage> pieces, int amount, bool shadow, IList<EffectDescriptor>? effects = null)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.DefaultImageName = defaultImageName;
            this.PivotOrigin = RectanglePoint.Center;
            this.Opacity = .6f;
            this.effects = effects;

            if (amount > 0 && pieces.Count > 0)
            {
                for (var i = 0; i < amount; i++)
                {
                    var piece = Session.ObjectPools.RemainsPieces.Get();
                    piece.Image = pieces.GetRandomItem();
                    piece.Opacity = .75f;
                    piece.Scale = scale;
                    piece.Shadow = shadow;
                    this.pieces.Add(piece);
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
                Session.ObjectPools.RemainsPieces.Return(pieces[i]);
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

            if (applyEffectTimer > 0)
            {
                if (Session.Player?.IsInCurrentRoom == true && !BoundingBox.Contains(Session.Player.Position))
                    applyEffectTimer = 0;
                else
                    applyEffectTimer -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            if (Sprite.RenderImage != null && effects != null)
            {
                if (Session.Player?.IsInCurrentRoom == true && BoundingBox.Contains(Session.Player.Position))
                {
                    applyEffectTimer = 5000;
                    EffectDescriptor.Apply(effects, this, Session.Player, EffectContext.RemainsContact);
                }
            }
        }

        #endregion
    }
}
