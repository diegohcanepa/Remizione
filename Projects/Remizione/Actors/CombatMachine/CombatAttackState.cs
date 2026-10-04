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

            // Zona de no-retorno: clava el input SOLO si el target es el jugador humano
            if (currentDistance <= lockCommitDistance && Target == Owner.Session.Player)
            {
                Target.StopMoving();
                Owner.Session.AttackingNPC = Owner;
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

            if (Target != null && Target == Owner.Session.Player)
                Owner.Session.AttackingNPC = null;
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
                    // Verificamos de nuevo SOLO si el target es el humano
                    if (Target != null && Target == Owner.Session.Player && Owner.Session.AttackingNPC == null)
                    {
                        if (Owner.DistanceToTarget(Target) <= lockCommitDistance)
                        {
                            Target.StopMoving();
                            Owner.Session.AttackingNPC = Owner;
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
                            Owner.ExecuteAction(Intent, Target);
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