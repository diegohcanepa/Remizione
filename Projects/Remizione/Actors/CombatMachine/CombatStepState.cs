using Engendro;
using Microsoft.Xna.Framework;

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
            var target = Owner.Session.Player;
            if (target == null || Owner.CombatBehavior?.Archetype is not { } arch)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            float distance = Owner.DistanceToTarget(target);

            // Usamos MaxStepPerTurn como la magnitud del paso (ya sea para avanzar o huir)
            float stepDistance = arch.MaxStepPerTurn;

            // Si estamos avanzando, limitamos el paso para no traspasar al objetivo.
            // (Si huimos, simplemente caminamos el paso entero hacia atrás).
            if (distance < stepDistance)
                stepDistance = distance;

            // Delegamos la decisión direccional al arquetipo
            arch.ExecuteStepMovement(Owner, target, stepDistance, distance);
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