using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatExposedState
    /// </summary>
    public sealed class CombatExposedState : State<Actor>
    {
        private float timer;

        // Enter
        public override void Enter()
        {
            Owner.StopMoving();
            timer = Owner.CombatBehavior?.Archetype.ExposedPauseDuration ?? 1.2f;
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

            // If targeted by player, remains exposed until impact
            if (Owner.IsTargetedByPlayer)
                return;

            float distance = Owner.DistanceToTarget(target);
            bool isTargetInvulnerable = target is Actor a && a.IsInvulnerable;

            // 2. EVALUACIÓN DE ATAQUE:
            // Solo atacamos si el jugador no está invulnerable (parpadeando) y hay línea de visión sin obstáculos.
            if (!isTargetInvulnerable && Owner.HasLineOfSightTo(target))
            {
                var intent = arch.SelectIntent(Owner, Owner.CombatBehavior.Intents, distance);
                if (intent != null)
                {
                    // A) Detenemos el movimiento previo del jugador
                    target.StopMoving();

                    // B) Preparamos y pasamos al estado de alineación + ataque
                    var attackState = Machine.FindOrCreateState<CombatAttackState>();
                    attackState.Intent = intent;
                    attackState.Target = target;

                    Machine.ChangeState(attackState.GetType());
                    return;
                }
            }

            // 3. TEMPORIZADOR DE EXPOSICIÓN:
            // Si no ataca ni está targeteado, consume la pausa táctica antes de reevaluar
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer <= 0f)
            {
                // Si el jugador se alejó demasiado o se cubrió detrás de una cobertura (sin LoS),
                // el NPC pierde la hostilidad y vuelve a Idle.
                if (distance > arch.LoseSightRange || !Owner.HasLineOfSightTo(target))
                {
                    Owner.IsHostile = false;
                    Machine.ChangeState<CombatIdleState>();
                }
                else
                {
                    Machine.ChangeState<CombatStepState>();
                }
            }
        }
    }
}