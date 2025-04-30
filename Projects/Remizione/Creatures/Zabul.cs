using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Zabul
    /// </summary>
    public sealed class Zabul : Actor
    {
        private int nextDamageCooldown;

        // Constructor
        public Zabul(GameSession session, string name)
            : base(session, name)
        {
            this.BodySize = ActorSize.Small;
            this.Affinity = Affinity.Evil;

            /*
            var charge = new AIChargeState(this);
            var idle = new AIIdleState(this);

            AIStateMachine = new AIStateMachine(this, idle);
            AIStateMachine.AddTransition<AIIdleState>(AIStateSignal.SawPlayer, charge);
            AIStateMachine.AddTransition<AIChargeState>(AIStateSignal.ChargeComplete, idle);
            */

            Skills.Add(ItemName.ZabulContact, 1);
            AttackSkillName = ItemName.ZabulContact;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (nextDamageCooldown > 0)
            {
                nextDamageCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            if (Session.Player != null)
            {
                if (AreHurtBoxesVisuallyOverlapping(Session.Player))
                {
                    if (AttackSkill != null && AttackSkill.BeginUse() == ItemUsageResult.Succeeded)
                        AttackSkill.EndUse(Session.Player);

                    nextDamageCooldown = 400;
                }
            }
        }
    }
}
