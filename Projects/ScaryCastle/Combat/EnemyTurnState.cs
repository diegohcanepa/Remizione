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

        #region Protected members

        // OnEnter
        protected override void OnEnter()
        {
            attackDone = false;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (!attackDone && TimeInState > 2)
            {
                if (Manager.Enemy.Definition != null)
                    EffectDescriptor.Apply(Manager.Enemy.Definition.Effects, Manager.Enemy, Manager.Player, AttackType.Contact);

                Manager.Session.HUD.CombatFeedback.Show("Un zarpazo te desgarra la manga.", 2500);

                attackDone = true;
            }

            if (TimeInState > 5)
                Manager.TransitionTo(new PlayerTurnState(Manager));
        }

        #endregion
    }
}
