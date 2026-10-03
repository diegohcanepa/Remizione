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
        private const float lockCommitDistance = 20;

        // Enter
        public override void Enter()
        {
            if (Intent == null || Target == null)
            {
                Machine.ChangeState<CombatStepState>();
                return;
            }

            bool isCharge = Intent.IsCharge;
            Owner.IsDealingContactDamage = isCharge;

            // Calculamos el punto destino exacto en el instante en que inicia el ataque
            var interactionPoint = isCharge ? Target.Position : Target.GetApproachPosition(Owner, ApproachBehavior.ClosestSide);

            // Opción 2: Si el jugador ya está más lejos que el MaxRange del ataque, recortamos el recorrido
            Vector2 direction = interactionPoint - Owner.Position;
            float currentDistance = direction.Length();

            if (currentDistance > Intent.MaxRange && currentDistance > 0)
            {
                direction.Normalize();
                interactionPoint = Owner.Position + (direction * Intent.MaxRange);
            }

            // Si el enemigo ya arranca el ataque pegado al jugador (<= 15px), lo clavamos de entrada
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

            if (Target == Owner.Session.Player)
                Owner.Session.AttackingNPC = null;
        }

        // Intent
        public CombatIntent? Intent { get; set; }

        // Target
        public GameThing? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            switch (currentPhase)
            {
                case AttackPhase.Aligning:
                    // Mientras el enemigo avanza hacia el punto de impacto:
                    // Si el jugador entra a la zona de compromiso (15px), anulamos su input
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