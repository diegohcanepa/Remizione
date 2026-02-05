using Adberration.Scripting;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnemyTurnState
    /// </summary>
    public sealed class EnemyTurnState : CombatantTurnState
    {
        private bool scriptLaunched;

        // PlayerTurnState
        public EnemyTurnState(CombatManager manager)
            : base(manager)
        {
        }

        #region Protected members

        // OnEnter
        protected override void OnEnter()
        {
            base.OnEnter();
            scriptLaunched = false;
        }

        // OnScriptCompleted
        protected override void OnScriptCompleted()
        {
            Manager.TransitionTo(new PlayerTurnState(Manager));
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (!scriptLaunched && TimeInState > 2)
            {
                scriptLaunched = true;
                if (Manager.Session.ScriptLibrary.FindOutcome(Manager.Enemy.DeclaredName) is Script script)
                    AwaitScript(script);
            }
        }
        
        #endregion
    }
}
