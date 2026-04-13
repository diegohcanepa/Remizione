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
        private readonly List<Debris> debrisList = [];
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

        // Dispose
        public void Dispose()
        {
            source.Session.ObjectPools.Debris.Return(debrisList);
        }

        // Launch
        public void Launch()
        {
            for (var i = 0; i < debrisList.Count; i++)
            {
                debrisList[i].Launch(source);
            }
        }
    }

}
