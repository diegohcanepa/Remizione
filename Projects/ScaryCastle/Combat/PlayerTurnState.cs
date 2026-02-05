using Adberration.Scripting;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerTurnState
    /// </summary>
    public sealed class PlayerTurnState : CombatantTurnState
    {
        // PlayerTurnState
        public PlayerTurnState(CombatManager manager)
            : base(manager)
        {
        }

        #region Protected members

        // OnEnter
        protected override void OnEnter()
        {
            base.OnEnter();
            Manager.Enemy.DecideCombatIntent();
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
                            AwaitScript(context.UseWithScript);
                        }
                        else if (Manager.Session.ScriptLibrary.FindRoutine($"Use{item.Name}") is Script script)
                        {
                            AwaitScript(script);
                        }
                    }

                    if (IsAwaitingScript)
                        context.HeldItem = null;
                }
            }

            return HandleInputResult.Handled;
        }

        // OnScriptCompleted
        protected override void OnScriptCompleted()
        {
            Manager.TransitionTo(new EnemyTurnState(Manager));
        }
        
        #endregion
    }
}
