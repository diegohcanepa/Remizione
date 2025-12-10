using System;
using System.Collections.Generic;
using System.Text;

namespace Remizione.Traps
{
    /// <summary>
    /// Hazard
    /// </summary>
    public class Hazard : GameThing
    {
        // Constructor
        public Hazard(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            CollisionDetection = false;
            IgnoreThrowables = true;
        }
    }
}
