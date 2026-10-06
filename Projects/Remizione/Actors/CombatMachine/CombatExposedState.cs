using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// CombatExposedState - Sirve como ventana de telegrafiado/wind-up previo al ataque
    /// </summary>
    public sealed class CombatExposedState : State<Actor>
    {
        private CombatIntent? pendingIntent;
        private float timer;

        // Target dinámico
        public Actor? Target { get; set; }

        // Enter
        public override void Enter()
        {
            Owner.StopMoving();
            timer = Owner.CombatBehavior?.Archetype?.ExposedPauseDuration ?? 1.2f;
            pendingIntent = null;

            if (Target != null && !Target.IsDead && Owner.CombatBehavior?.Archetype is { } arch)
            {
                float distance = Owner.DistanceToTarget(Target);
                pendingIntent = arch.SelectIntent(Owner, Owner.CombatBehavior.Intents, distance);

                // Notificamos al Actor que empiece su telegrafiado visual
                if (pendingIntent != null)
                {
                    Owner.StartAttackTelegraph(pendingIntent, timer);

                    if (pendingIntent.SoundStart != null)
                        Owner.PlaySound(pendingIntent.SoundStart);
                }
            }
        }

        // Exit
        public override void Exit()
        {
            Owner.StopAttackTelegraph();
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

            if (distance > arch.LoseSightRange || !Owner.HasLineOfSightTo(Target))
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            // 1. Descontamos el timer
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 2. EFECTO VISUAL: Vibración de telegrafiado
            // Si el ataque requiere vibración, el estado solo le pide al Actor que se sacuda
            if (timer > 0f && pendingIntent?.TelegraphKind == AttackTelegraphKind.Vibration)
            {
                float progress = 1f - (timer / arch.ExposedPauseDuration);
                Owner.Vibrate(progress);
            }

            // 3. Transición al ataque
            if (timer <= 0f)
            {
                // Limpiamos el offset por seguridad
                Owner.Offset = Vector2.Zero;

                if (!Target.IsInvulnerable && pendingIntent != null)
                {
                    var attackState = Machine.FindOrCreateState<CombatAttackState>();
                    attackState.Intent = pendingIntent;
                    attackState.Target = Target;
                    Machine.ChangeState(attackState.GetType());
                }
                else
                {
                    var stepState = Machine.FindOrCreateState<CombatStepState>();
                    stepState.Target = Target;
                    Machine.ChangeState(stepState.GetType());
                }
            }
        }
    }
}