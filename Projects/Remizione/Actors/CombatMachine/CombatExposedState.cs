using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatExposedState - Sirve como ventana de telegrafiado/wind-up previo al ataque
    /// </summary>
    public sealed class CombatExposedState : State<Actor>
    {
        private float timer;
        private CombatIntent? pendingIntent;

        // Target dinámico
        public Actor? Target { get; set; }

        // Enter
        public override void Enter()
        {
            Owner.StopMoving();
            timer = Owner.CombatBehavior?.Archetype.ExposedPauseDuration ?? 1.2f;
            pendingIntent = null;

            if (Target != null && !Target.IsDead && Owner.CombatBehavior?.Archetype is { } arch)
            {
                float distance = Owner.DistanceToTarget(Target);
                pendingIntent = arch.SelectIntent(Owner, Owner.CombatBehavior.Intents, distance);
            }
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Target == null || Target.IsDead || Owner.CombatBehavior?.Archetype is not { } arch)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            float distance = Owner.DistanceToTarget(Target);

            // 1. Si el objetivo se fue de rango o perdió LoS, aborta y resetea aggro
            if (distance > arch.LoseSightRange || !Owner.HasLineOfSightTo(Target))
            {
                float leash = Owner.CombatBehavior?.LeashRadius ?? 0f;
                if (leash > 0f)
                {
                    Owner.MoveRandomlyAround(Owner.HomePosition, leash);
                }

                Machine.ChangeState<CombatIdleState>();
                return;
            }

            // 2. Consume el tiempo de telegrafiado/preparación visual
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 3. Cuando el timer llega a 0, lanza el ataque o da un paso
            if (timer <= 0f)
            {
                if (!Target.IsInvulnerable && pendingIntent != null)
                {
                    var attackState = Machine.FindOrCreateState<CombatAttackState>();
                    attackState.Intent = pendingIntent;
                    attackState.Target = Target;
                    Machine.ChangeState(attackState.GetType());
                }
                else
                {
                    // Si el objetivo es invulnerable, intenta dar un paso
                    var stepState = Machine.FindOrCreateState<CombatStepState>();
                    stepState.Target = Target;
                    Machine.ChangeState(stepState.GetType());
                }
            }
        }
    }
}