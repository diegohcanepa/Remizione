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

            // 1. Clavamos al jugador SOLO si es un ataque melee normal.
            // Si es un Charge, dejamos al jugador libre para que pueda clickear y esquivar.
            if (!isCharge && Target == Owner.Session.Player)
            {
                Target.StopMoving();
                Owner.Session.AttackingNPC = Owner;
            }

            // 2. Calculamos el punto destino en base a donde está el jugador AHORA MISMO.
            // Como tu motor no recalcula la ruta en el camino, el NPC irá ciegamente hacia acá.
            var interactionPoint = isCharge ? Target.Position : Target.GetApproachPosition(Owner, ApproachBehavior.ClosestSide);

            // 3. Le ordenamos al cuerpo del NPC moverse. Si es charge, va con fast = true.
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
                    // Mientras se mueve (Aligning), si es un Charge y lo toca, 
                    // tu motor de colisiones aplicará el daño por contacto por fuera de acá.

                    if (!Owner.IsMoving && Intent != null)
                    {
                        if (Intent.IsCharge)
                        {
                            // Si terminó de correr la embestida, ya sea que haya impactado o chocado la pared,
                            // pasamos directamente al cooldown sin ejecutar animación estática de golpe.
                            Machine.ChangeState<CombatCooldownState>();
                        }
                        else
                        {
                            // Llegó al punto exacto para un ataque normal: iniciamos la animación
                            currentPhase = AttackPhase.Striking;
                            Owner.ExecuteAction(Intent, Target);
                        }
                    }
                    break;

                case AttackPhase.Striking:
                    // Mientras dure la animación de ataque estático, esperamos a que el cuerpo termine
                    if (!Owner.IsExecutingAction)
                    {
                        Machine.ChangeState<CombatCooldownState>();
                    }
                    break;
            }
        }
    }
}