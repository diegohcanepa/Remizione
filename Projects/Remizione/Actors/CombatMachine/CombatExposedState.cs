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

        // Enter
        public override void Enter()
        {
            Owner.StopMoving();
            timer = Owner.CombatBehavior?.Archetype.ExposedPauseDuration ?? 1.2f;
            pendingIntent = null;

            // Evaluamos de entrada qué ataque va a preparar durante esta pausa
            if (Owner.Session.Player is Actor target && Owner.CombatBehavior?.Archetype is { } arch)
            {
                float distance = Owner.DistanceToTarget(target);
                pendingIntent = arch.SelectIntent(Owner, Owner.CombatBehavior.Intents, distance);
            }
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Session.Player is not Actor target)
                return;

            if (Owner.CombatBehavior?.Archetype is not { } arch)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            float distance = Owner.DistanceToTarget(target);

            // 1. Si el jugador se fue de rango o perdió LoS durante el telegrafiado, aborta la carga
            if (distance > arch.LoseSightRange || !Owner.HasLineOfSightTo(target))
            {
                Owner.IsHostile = false;

                float leash = Owner.CombatBehavior?.LeashRadius ?? 0f;
                if (leash > 0f)
                {
                    Owner.MoveRandomlyAround(Owner.HomePosition, leash);
                }

                Machine.ChangeState<CombatIdleState>();
                return;
            }

            // 2. Consume el tiempo de telegrafiado/preparación visual (los 4s o lo que configures)
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 3. Cuando el timer llega a 0, Lanza el ataque preparado
            if (timer <= 0f)
            {
                bool isTargetInvulnerable = target.IsInvulnerable;

                if (!isTargetInvulnerable && pendingIntent != null)
                {
                    var attackState = Machine.FindOrCreateState<CombatAttackState>();
                    attackState.Intent = pendingIntent;
                    attackState.Target = target;

                    Machine.ChangeState(attackState.GetType());
                }
                else
                {
                    // Si el objetivo se volvió invulnerable o no hay intent, reevalúa dando un paso
                    Machine.ChangeState<CombatStepState>();
                }
            }
        }
    }
}