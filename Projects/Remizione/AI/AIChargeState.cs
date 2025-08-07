using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIChargeState
    /// </summary>
    public sealed class AIChargeState : AIState
    {
        // Constructor
        public AIChargeState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Charge)
        {
        }

        #region Private members

        // MoveTowardsTarget
        private void MoveTowardsTarget()
        {
            if (Actor.Target is GameThing target)
            {
                var destination = target.GetApproachPosition(Actor, false);
                Actor.MoveTo(destination);
            }
        }

        #endregion

        // Enter
        public override void Enter()
        {
            base.Enter();
            MoveTowardsTarget();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
        }
    }
}
