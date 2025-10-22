using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ChargeState
    /// </summary>
    internal class ChargeState : AIState
    {
        // Constructor
        public ChargeState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Charge)
        {
        }

        #region Private members

        // Charge
        private void Charge()
        {
            if (Owner.PerceptionSensor.CurrentTarget is GameThing target)
                Owner.MoveTo(target.Position);
        }

        #endregion

        // Enter
        public override void Enter()
        {
            Charge();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsMoving)
                StateMachine.ChangeState(AIStateName.Decide);
        }
    }
}
