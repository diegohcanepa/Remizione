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
        private readonly List<DebrisPiece> debrisList = [];

        // Constructor
        public BreakableProp(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            IsAttackable = true;
        }

        #region Protected members

        // OnDie
        protected override void OnDeath()
        {
            Break();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsBroken)
            {
                for (var i = 0; i < debrisList.Count; i++)
                {
                    debrisList[i].Draw(gameTime);
                }
            }
            else
            {
                base.OnDraw(gameTime);
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (debrisList.Count == 0)
            {
                var index = 1;
                while (true)
                {
                    if (Atlas?.FindImage($"{DeclaredName}Piece{index}") is AtlasImage image)
                    {
                        var debris = Session.ObjectPools.DebrisPieces.Get();
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
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            Session.ObjectPools.DebrisPieces.Return(debrisList);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (IsBroken)
            {
                for (var i = 0; i < debrisList.Count; i++)
                {
                    debrisList[i].Update(gameTime);
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
            for (var i = 0; i < debrisList.Count; i++)
            {
                debrisList[i].Launch(this);
            }
        }

        // IsBroken
        public bool IsBroken { get; private set; }
    }
}