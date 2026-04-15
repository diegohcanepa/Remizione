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
        private readonly List<Debris> debrisList = [];
        private readonly Prop source;

        // Constructor
        public BrokenPieces(Prop source)
        {
            this.source = source;

            var index = 1;
            while (true)
            {
                if (source.Atlas?.FindImage($"{source.DeclaredName}Piece{index}") is { } image)
                {
                    var debris = source.Session.ObjectPools.Debris.Get();
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
            source.Session.ObjectPools.Debris.Return(debrisList);
        }
    }
}
