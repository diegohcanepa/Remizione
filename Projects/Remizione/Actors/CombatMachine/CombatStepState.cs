using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatStepState
    /// </summary>
    public sealed class CombatStepState : State<Actor>
    {
        // Target dinámico
        public Actor? Target { get; set; }

        // Enter
        public override void Enter()
        {
            if (Target == null || Target.IsDead || Owner.CombatBehavior?.Archetype is not { } arch)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            float distance = Owner.DistanceToTarget(Target);

            // Usamos MaxStepPerTurn como la magnitud del paso
            float stepDistance = arch.MaxStepPerTurn;

            // Limitamos el paso si estamos más cerca que el máximo
            if (distance < stepDistance)
                stepDistance = distance;

            // Iniciamos el movimiento
            arch.ExecuteStepMovement(Owner, Target, stepDistance, distance);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Target == null || Target.IsDead)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            // 1. Interrupción por evento externo o fin natural del paso
            if (!Owner.IsMoving)
            {
                var exposedState = Machine.FindOrCreateState<CombatExposedState>();
                exposedState.Target = Target;
                Machine.ChangeState(exposedState.GetType());
                return;
            }

            // 2. Interrupción activa (Radar): Evaluar si durante la caminata entramos en rango de ataque
            if (Owner.CombatBehavior?.Archetype is { } arch && Owner.HasLineOfSightTo(Target))
            {
                float distance = Owner.DistanceToTarget(Target);
                var intent = arch.SelectIntent(Owner, Owner.CombatBehavior.Intents, distance);

                // Si encontramos un ataque válido a mitad de camino, abortamos la caminata
                if (intent != null)
                {
                    Owner.StopMoving();
                    var exposedState = Machine.FindOrCreateState<CombatExposedState>();
                    exposedState.Target = Target;
                    Machine.ChangeState(exposedState.GetType());
                }
            }
        }
    }
}