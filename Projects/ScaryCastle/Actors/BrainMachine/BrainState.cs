using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// BrainState
    /// </summary>
    public abstract class BrainState : State<ProceduralActor>
    {
        // Constrcutor
        protected BrainState()
        {
        }

        // TransitionTo
        protected void TransitionTo<TNextState>() where TNextState : State<ProceduralActor>, new()
        {
            Owner.BrainMachine.FindOrCreateState<TNextState>();
            Owner.BrainMachine.ChangeState<TNextState>();
        }
    }
}