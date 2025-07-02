using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remizione
{
    /// <summary>
    /// Baal
    /// </summary>
    public sealed class Baal : Actor
    {
        // Constructor
        public Baal(GameSession session, string name)
            : base(session, name)
        {
            FloatingForce = 1;
        }
    }
}
