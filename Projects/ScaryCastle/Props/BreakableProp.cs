using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// BreakableProp
    /// </summary>
    public class BreakableProp : Prop
    {
        private readonly List<ShatterPiece> pieces = [];

        // Constructor
        public BreakableProp(GameSession session, string name)
            : base(session, name)
        {
            HitEffect = HitEffect.Shake;
            HurtShake = new(1.5f, 0);
        }

        #region Protected members

        // OnDie
        protected override void OnDie() => Break();

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsBroken)
            {
                for (var i = 0; i < pieces.Count; i++)
                {
                    pieces[i].Draw(gameTime);
                }
            }
            else
                base.OnDraw(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (pieces.Count == 0)
            {
                var index = 1;
                while (true)
                {
                    if (Atlas?.GetImage($"{StaticName}Piece{index}") is AtlasImage image)
                    {
                        pieces.Add(new(Session.Game, image, Vector2.One));
                        index++;
                    }
                    else
                        break;
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (IsBroken)
            {
                for (var i = 0; i < pieces.Count; i++)
                {
                    pieces[i].Update(gameTime);
                }
            }
        }

        #endregion

        // Break
        public void Break()
        {
            if (IsBroken)
                return;

            RenderLayer = RenderLayer.OverBackground;

            IsBroken = true;
            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Launch(this);
            }
        }

        // IsBroken
        public bool IsBroken { get; private set; }
    }
}
