using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// Sack
    /// </summary>
    public sealed class Sack : Prop
    {
        // Constructor
        public Sack(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            DepthOffset = -2;
            DisplayNameKey = "Prop.Sack";
            Hotspot =  new Polygon("0,0;7,0;7,7;0,7");
        }

        // Item
        public Item? Item { get; set; }
    }
}
