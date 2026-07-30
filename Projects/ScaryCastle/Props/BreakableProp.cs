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
        private readonly List<AtlasImage> pieces = [];

        // Constructor
        public BreakableProp(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;

            var index = 1;
            while (true)
            {
                if (Atlas.FindImage($"{DeclaredName}Piece{index}") is AtlasImage image)
                    pieces.Add(image);
                else
                    break;

                index++;
            }
        }

        #region Protected members

        // OnDeath
        protected override void OnDeath()
        {
        }

        #endregion
    }
}