using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    public class CombatAttackState : State<Actor>
    {
        public CombatIntent? Intent { get; set; }
        public GameThing? Target { get; set; }

        private enum AttackPhase { Aligning, Striking }
        private AttackPhase currentPhase;

        public override void Enter()
        {
            if (Intent == null || Target == null)
            {
                Machine.ChangeState<CombatStepState>();
                return;
            }

            // 1. Clavamos al jugador de entrada para que no siga caminando
            if (Target == Owner.Session.Player)
            {
                Target.StopMoving();
                Owner.Session.AttackingNPC = Owner;
            }

            // 2. Calculamos el punto de contacto ideal
            var interactionPoint = Target.GetApproachPosition(Owner, ApproachBehavior.ClosestSide);

            // 3. Le ordenamos al cuerpo del NPC caminar fluidamente hacia ese punto
            Owner.MoveTo(interactionPoint);
            currentPhase = AttackPhase.Aligning;
        }

        public override void Update(GameTime gameTime)
        {
            switch (currentPhase)
            {
                case AttackPhase.Aligning:
                    // Esperamos a que el NPC termine de caminar físicamente hasta el punto de contacto
                    if (!Owner.IsMoving && Intent != null)
                    {
                        // Llegó al punto exacto: pasamos a la fase de golpe e iniciamos la animación
                        currentPhase = AttackPhase.Striking;
                        Owner.ExecuteAction(Intent, Target);
                    }
                    break;

                case AttackPhase.Striking:
                    // Mientras dure la animación de ataque, esperamos a que el cuerpo termine
                    if (!Owner.IsExecutingAction)
                    {
                        Machine.ChangeState<CombatCooldownState>();
                    }
                    break;
            }
        }

        public override void Exit()
        {
            if (Target == Owner.Session.Player)
                Owner.Session.AttackingNPC = null;
        }
    }
}