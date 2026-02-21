using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerInputHandler
    /// </summary>
    public sealed class PlayerInputHandler<T> : InputHandler where T : Actor
    {
        private readonly CombatBehavior combatBehavior;

        // Constructor
        public PlayerInputHandler(T actor, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            this.Actor = actor;
            this.combatBehavior = CombatBehavior.Behaviors.Get(actor.DeclaredName);
        }

        #region Private members

        // HandleMouseInput
        private HandleInputResult HandleMouseInput()
        {
            // Left button
            if (TestMouseLeftButtonClick())
                return HandleInputResult.Handled;

            // Right button
            if (TestMouseRightButtonClick())
                return HandleInputResult.Handled;

            return HandleInputResult.Unhandled;
        }

        // PerformInteraction
        private void PerformInteraction()
        {
            if (!Actor.IsPlayer)
                return;

            var context = Actor.Session.InteractionContext;

            MouseCursor.PerformClick();

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);

            // 1. No target: Basic walk to destination
            if (context.Target == null)
            {
                if (context.HeldItem == null || !context.HeldItem.Definition.IsMagical)
                {
                    Actor.Session.InteractionData.Clear();
                    Actor.MoveTo(destination);
                    return;
                }
            }

            // 2. Outcome interaction: Approach and interact with target using outcome script
            if (context.Target != null)
            {
                if (context.HeldItem == null || MouseCursor.IsArrow)
                {
                    Actor.ApproachAndInteract(context.Target, null);
                    return;
                }

                // 3. Classic "Use with" interaction: Approach and interact with target using held item
                if (context.HeldItem?.Definition.IsMagical == false)
                {
                    if (Actor.ApproachAndInteract(context.Target, context.HeldItem))
                        return;
                }
            }

            // 3. Cast
            if (context.HeldItem?.Definition.IsMagical == true)
            {
                if (Actor.Cast(destination, context.HeldItem))
                    return;
            }

            MouseCursor.Shake();
        }

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            if (MouseCursor.State == MouseCursorState.Hand && MouseCursor.CustomImage == null)
                return false;

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            PerformInteraction();

            return false;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            if (Actor.Session.InteractionContext.HeldItem != null)
            {
                Sound.Play(SoundNames.Interact);
                Actor.Session.InteractionContext.HeldItem = null;
                Actor.Session.InteractionData.Clear();
                Actor.StopMoving();
            }
            else
            {
                Actor.Attack(combatBehavior.Intents[0], null);
            }

            return true;
        }

        #endregion

        // Actor
        public T Actor { get; }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            return HandleMouseInput();
        }
    }
}
