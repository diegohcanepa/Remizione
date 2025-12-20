using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// ExpendingMachine
    /// </summary>
    public sealed class ExpendingMachine : Prop
    {
        // Constructor
        public ExpendingMachine(GameSession session, string name)
            : base(session, name)
        {
        }

        // Use
        [ScriptMethod]
        public void Use()
        {
            AllowInteraction = false;
        }
    }
}
