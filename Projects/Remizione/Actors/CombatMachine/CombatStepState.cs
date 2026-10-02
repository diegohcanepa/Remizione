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

            // Usamos MaxStepPerTurn como la magnitud del paso
            float stepDistance = arch.MaxStepPerTurn;

            // Limitamos el paso si estamos más cerca que el máximo
            if (distance < stepDistance)
                stepDistance = distance;

            // Iniciamos el movimiento ciego
            arch.ExecuteStepMovement(Owner, target, stepDistance, distance);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Session.Player is not Actor target)
                return;

            // 1. Interrupción por evento externo o fin natural del paso
            if (!Owner.IsMoving)
            {
                Machine.ChangeState<CombatExposedState>();
                return;
            }

            // 2. Interrupción activa (Radar): Evaluar si durante la caminata entramos en rango de ataque
            if (Owner.CombatBehavior?.Archetype is { } arch && Owner.HasLineOfSightTo(target))
            {
                float distance = Owner.DistanceToTarget(target);
                var intent = arch.SelectIntent(Owner, Owner.CombatBehavior.Intents, distance);

                // Si encontramos un ataque válido a mitad de camino, abortamos la caminata
                if (intent != null)
                {
                    Owner.StopMoving();
                    Machine.ChangeState<CombatExposedState>();
                }
            }
        }
    }
}