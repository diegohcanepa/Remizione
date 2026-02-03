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

        // PlayerTurnState
        public PlayerTurnState(CombatManager manager)
            : base(manager)
        {
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

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (awaitingScript != null)
            {
                if (!Manager.Session.IsAwaitingScript(awaitingScript))
                    Manager.TransitionTo(new EnemyTurnState(Manager));
            }
            else if (Manager.Enemy.Definition != null && TimeInState > Manager.Enemy.Definition.Patience)
            {
                Manager.TransitionTo(new EnemyTurnState(Manager));
            }
        }
    }
}
