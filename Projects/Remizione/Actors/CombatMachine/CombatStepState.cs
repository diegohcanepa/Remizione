using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// CombatIdleState
    /// </summary>
    public class CombatStepState : State<Actor>
    {
        // Enter
        public override void Enter()
        {
            var target = Owner.Session.Player;
            if (target == null || Owner.CombatBehavior?.Archetype is not { } arch)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            // Calculamos el punto destino limitado a MaxStepPerTurn (ej: 50px hacia el jugador)
            float distance = Owner.DistanceToTarget(target);
            float stepDistance = Math.Min(distance, arch.MaxStepPerTurn);

            // Ordenamos al actor caminar este tramo
            Owner.MoveTowards(target.Position, stepDistance);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            // Si el jugador me targeteó, O si ya terminé mi caminata: paso a Exposed
            if (Owner.IsTargetedByPlayer || !Owner.IsMoving)
            {
                if (Owner.IsTargetedByPlayer)
                    Owner.StopMoving();

                Machine.ChangeState<CombatExposedState>();
            }
        }
    }
}
