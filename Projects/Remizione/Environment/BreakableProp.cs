using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// BreakableProp
    /// </summary>
    public class BreakableProp : IsometricProp
    {
        private bool broken;
        private readonly List<ShatterPiece> pieces = [];

        // Constructor
        public BreakableProp(GameSession session, string name)
            : base(session, name)
        {
            //HitEffect = HitEffect.Shake;
            HurtShake = new(1.5f, 0);

            var index = 1;
            while (true)
            {
                if (Atlas?.GetImage($"{StaticName}Piece{index}") is AtlasImage image)
                {
                    pieces.Add(new(this, image));
                    index++;
                }
                else
                    break;
            }
        }

        #region Protected members

        // OnDie
        protected override void OnDie() => Break();

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (broken)
            {
                for (var i = 0; i < pieces.Count; i++)
                {
                    pieces[i].Draw(gameTime);
                }
            }
            else
                base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (broken)
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
            broken = true;
            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Launch();
                RenderLayer = RenderLayer.Background;
            }
        }
    }
}
