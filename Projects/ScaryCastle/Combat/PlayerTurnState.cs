using Adberration.Scripting;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerTurnState
    /// </summary>
    public sealed class PlayerTurnState : CombatManagerState
    {
        private Script? awaitingScript;
        private int awaitScriptCooldown;

        // PlayerTurnState
        public PlayerTurnState(CombatManager manager)
            : base(manager)
        {
        }

        #region Protected members

        // OnEnter
        protected override void OnEnter()
        {
            awaitScriptCooldown = 100;
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            var context = Manager.Session.InteractionContext;

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                context.HeldItem = null;
                return HandleInputResult.Handled; 
            }

            if (context.Target != null)
            {
                if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                {
                    if (context.HeldItem is Item item)
                    {
                        if (context.UseWithScript != null)
                        {
                            awaitingScript = context.UseWithScript;
                        }
                        else if (Manager.Session.ScriptLibrary.FindRoutine($"Use{item.Name}") is Script script)
                        {
                            awaitingScript = script;
                        }
                    }

                    if (awaitingScript != null)
                    {
                        context.HeldItem = null;
                        Manager.Session.AwaitScript(awaitingScript);
                    }
                }
            }

            return HandleInputResult.Handled;
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
                    Manager.TransitionTo(new EnemyTurnState(Manager));
                }
            }
            else if (Manager.Enemy.Definition is ThingDefinition def)
            {
                if (TimeInState > def.Patience)
                    Manager.TransitionTo(new EnemyTurnState(Manager));
            }
        }

        #endregion
    }
}
