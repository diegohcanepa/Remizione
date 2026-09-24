using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// CombatStepState
    /// </summary>
    public sealed class CombatStepState : State<Actor>
    {
        // Enter
        public override void Enter()
        {
            if (Owner.Session.Player is not Actor target)
                return;

            if (Owner.CombatBehavior?.Archetype is not { } arch)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            // Limit move distance based on the archetype
            float distance = Owner.DistanceToTarget(target);
            float stepDistance = Math.Min(distance, arch.MaxStepPerTurn);

            Owner.MoveTowards(target.Position, stepDistance);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.IsTargetedByPlayer || !Owner.IsMoving)
            {
                if (Owner.IsTargetedByPlayer)
                    Owner.StopMoving();

                Machine.ChangeState<CombatExposedState>();
            }
        }
    }
}
