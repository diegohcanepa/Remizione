using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// ActorState
    /// </summary>
    public class ActorState : State<Actor>
    {
        // Constructor
        public ActorState(Actor owner, string name)
            : base(owner, name)
        {
        }
    }
}
