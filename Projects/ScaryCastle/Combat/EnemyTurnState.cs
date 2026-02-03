using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnemyTurnState
    /// </summary>
    public sealed class EnemyTurnState : CombatManagerState
    {
        private bool attackDone;

        // Constructor
        public EnemyTurnState(CombatManager manager)
            : base(manager)
        {
        }

        private void ApplyDamage()
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            attackDone = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!attackDone && TimeInState > 2)
            {
                if (Manager.Enemy.Definition != null)
                    EffectDescriptor.Apply(Manager.Enemy.Definition.Effects, Manager.Enemy, Manager.Player, AttackType.Contact);

                attackDone = true;
            }

            if (TimeInState > 5)
                Manager.TransitionTo(new PlayerTurnState(Manager));
        }
    }
}
