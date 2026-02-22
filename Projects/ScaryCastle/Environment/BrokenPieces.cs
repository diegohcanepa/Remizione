using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// BrokenPieces
    /// </summary>
    public sealed class BrokenPieces : GameObject
    {
        private readonly List<ShatterPiece> pieces = [];
        private readonly float scale;
        private readonly GameThing source;

        // Constructor
        public BrokenPieces(GameThing source, float scale = 1)
            : base(source.Game)
        {
            this.source = source;
            this.scale = scale;

            var index = 1;
            while (true)
            {
                if (source.Atlas?.FindImage($"{source.DeclaredName}Piece{index}") is { } image)
                {
                    pieces.Add(new ShatterPiece(Game, image, new(scale)));
                    index++;
                }
                else
                {
                    break;
                }
            }
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Opacity = source.Opacity;
                pieces[i].Update(gameTime);
            }
        }

        #endregion

        // Launch
        public void Launch()
        {
            for (var i = 0; i < pieces.Count; i++)
            {
                pieces[i].Launch(source);
            }
        }
    }
}
