using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// CombatantTurnState
    /// </summary>
    public abstract class CombatantTurnState : CombatManagerState
    {
        private Script? awaitingScript;
        private int awaitScriptCooldown;

        // PlayerTurnState
        protected CombatantTurnState(CombatManager manager)
            : base(manager)
        {
        }

        // AwaitScript
        protected void AwaitScript(Script script)
        {
            awaitingScript = script;
            Manager.Session.AwaitScript(awaitingScript);
            IsAwaitingScript = true;
        }

        // IsAwaitingScript
        protected bool IsAwaitingScript { get; private set; }

        // OnEnter
        protected override void OnEnter()
        {
            awaitScriptCooldown = 100;
            ScriptCompleted = false;
        }

        // OnScriptCompleted
        protected virtual void OnScriptCompleted()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (awaitingScript != null)
            {
                if (awaitScriptCooldown > 0)
                {
                    awaitScriptCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else if (!Manager.Session.IsAwaitingScript(awaitingScript))
                {
                    IsAwaitingScript = false;
                    ScriptCompleted = true;
                    OnScriptCompleted();
                }
            }
        }

        // ScriptCompleted
        protected bool ScriptCompleted { get; private set; }
    }
}
