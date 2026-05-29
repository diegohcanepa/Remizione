using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Debris
    /// </summary>
    public sealed class Debris : GameObject
    {
        private readonly List<DebrisPiece> debrisList = [];
        private readonly Prop source;

        // Constructor
        public Debris(Prop source)
        {
            this.source = source;

            var index = 1;
            while (true)
            {
                if (source.Atlas?.FindImage($"{source.DeclaredName}Piece{index}") is { } image)
                {
                    var debris = source.Session.ObjectPools.DebrisPieces.Get();
                    debris.Image = image;
                    debrisList.Add(debris);
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
            for (var i = 0; i < debrisList.Count; i++)
            {
                debrisList[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            for (var i = 0; i < debrisList.Count; i++)
            {
                //pieces[i].Opacity = source.Opacity;
                debrisList[i].Update(gameTime);
            }
        }

        #endregion

        // Launch
        public void Launch()
        {
            for (var i = 0; i < debrisList.Count; i++)
            {
                debrisList[i].Launch(source);
            }
        }

        // Release
        public void Release()
        {
            source.Session.ObjectPools.DebrisPieces.Return(debrisList);
        }
    }
}
