using Engendro;

namespace Remizione
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
