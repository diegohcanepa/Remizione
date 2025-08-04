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
        private readonly List<ShatterPiece> pieces = [];

        // Constructor
        public BreakableProp(GameSession session, string name)
            : base(session, name)
        {
            var index = 1;
            while (true)
            {
                if (Atlas?.GetImage($"{StaticName}Piece{index}") is AtlasImage image)
                {
                    pieces.Add(new(session.Game, image));
                    index++;
                }
                else
                    break;
            }
        }

        #region Protected members

        // OnDeath
        protected override void OnDeath()
        {
            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Launch(this);
            }
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsDead)
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

            if (IsDead)
            {
                for (var i = 0; i < pieces.Count; i++)
                {
                    pieces[i].Update(gameTime);
                }
            }
        }

        #endregion
    }
}
