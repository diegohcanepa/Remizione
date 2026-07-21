using Adberration.Scripting;
using System.Linq;

namespace ScaryCastle.Props
{
    /// <summary>
    /// GateLever
    /// </summary>
    public sealed class GateLever : Prop
    {
        // Constructor
        public GateLever(GameSession session, string name)
            : base(session, name)
        {
        }

        // OpenGates
        [ScriptMethod]
        public void OpenGates()
        {
            if (Room != null)
            {
                foreach (var door in Room.Children.OfType<RideDoor>())
                {
                    if (door.LockType == LockType.GateLever)
                    {
                        door.LockType = LockType.None;
                        door.Open();
                    }
                }
            }
        }
    }
}
