using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// BrokenPieces
    /// </summary>
    public sealed class BrokenPieces : GameObject, IDisposable
    {
        private readonly List<ShatterPiece> pieces = [];
        private readonly ThrownProp source;

        // Constructor
        public BrokenPieces(ThrownProp source)
        {
            this.source = source;

            var index = 1;
            while (true)
            {
                if (source.Prop.Atlas?.FindImage($"{source.Prop.DeclaredName}Piece{index}") is { } image)
                {
                    var shatterPiece = source.Session.ObjectPools.ShatterPieces.Get();
                    shatterPiece.Image = image;
                    pieces.Add(shatterPiece);
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
                //pieces[i].Opacity = source.Opacity;
                pieces[i].Update(gameTime);
            }
        }

        #endregion

        // Dispose
        public void Dispose()
        {
            source.Session.ObjectPools.ShatterPieces.Return(pieces);
        }

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
