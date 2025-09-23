using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remizione.Props
{
    /// <summary>
    /// RightTower
    /// </summary>
    public sealed class RightTower : IsometricProp
    {
        // Constructor
        public RightTower(GameSession session, string name)
            : base(session, name)
        {
        }
    }
}
