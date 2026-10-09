using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatAttackState
    /// </summary>
    public sealed class CombatAttackState : State<Actor>
    {
        private enum AttackPhase { Aligning, Striking }
        private AttackPhase currentPhase;
        private const float lockCommitDistance = 15f; // Valor de diseño establecido

        // Enter
        public override void Enter()
        {
            if (Intent == null || Target == null || Target.IsDead)
            {
                Machine.ChangeState<CombatIdleState>();
                return;
            }

            bool isCharge = Intent.IsCharge;
            Owner.IsDealingContactDamage = isCharge;

            var interactionPoint = isCharge ? Target.Position : Target.GetApproachPosition(Owner, ApproachBehavior.ClosestSide);

            Vector2 direction = interactionPoint - Owner.Position;
            float currentDistance = direction.Length();

            if (currentDistance > Intent.MaxRange && currentDistance > 0)
            {
                direction.Normalize();
                interactionPoint = Owner.Position + (direction * Intent.MaxRange);
            }

            // Zona de no-retorno: Aplica a CUALQUIER Target (Player o NPC)
            if (currentDistance <= lockCommitDistance)
            {
                Target.StopMoving();
                Target.IsLockedByAttacker = true;
            }

            if (isCharge && Intent.SoundStart != null)
                Owner.PlaySound(Intent.SoundStart);

            Owner.MoveTo(interactionPoint, isCharge);
            currentPhase = AttackPhase.Aligning;
        }

        // Exit
        public override void Exit()
        {
            Owner.IsDealingContactDamage = false;
            Target?.IsLockedByAttacker = false;
        }

        // Intent
        public CombatIntent? Intent { get; set; }

        // Target dinámico
        public Actor? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            switch (currentPhase)
            {
                case AttackPhase.Aligning:
                    // Mantiene la verificación de compromiso para cualquier target
                    if (Target != null && !Target.IsLockedByAttacker)
                    {
                        if (Owner.DistanceToTarget(Target) <= lockCommitDistance)
                        {
                            Target.StopMoving();
                            Target.IsLockedByAttacker = true;
                        }
                    }

                    if (!Owner.IsMoving && Intent != null)
                    {
                        if (Intent.IsCharge)
                        {
                            Machine.ChangeState<CombatCooldownState>();
                        }
                        else
                        {
                            currentPhase = AttackPhase.Striking;

                            float currentDistance = Target != null ? Owner.DistanceToTarget(Target) : float.MaxValue;
                            bool isLockedByInitiative = Target != null && Target.IsLockedByAttacker;

                            // Si el objetivo quedó bloqueado por iniciativa O está dentro del MaxRange, el golpe conecta
                            if (isLockedByInitiative || currentDistance <= Intent.MaxRange)
                            {
                                Owner.ExecuteAction(Intent, Target);
                            }
                            else
                            {
                                // if (Intent.SoundMiss != null)
                                //   Owner.PlaySound(Intent.SoundMiss);
                            }
                        }
                    }
                    break;

                case AttackPhase.Striking:
                    if (!Owner.IsExecutingAction)
                    {
                        Machine.ChangeState<CombatCooldownState>();
                    }
                    break;
            }
        }
    }
}