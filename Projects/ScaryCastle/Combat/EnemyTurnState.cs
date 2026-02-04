using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnemyTurnState
    /// </summary>
    public sealed class EnemyTurnState : CombatManagerState
    {
        private bool effectsApplied;
        private bool attackLaunched;

        // Constructor
        public EnemyTurnState(CombatManager manager)
            : base(manager)
        {
        }

        #region Protected members

        // OnEnter
        protected override void OnEnter()
        {
            effectsApplied = false;
            attackLaunched = false;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion
    }
}
