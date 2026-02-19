using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

/*
var lightning = new Lightning(player.Session);
lightning.Show(destination, 1500, Actor, context.HeldItem);
player.Room?.Children.Add(lightning);
context.HeldItem = null;
*/


namespace ScaryCastle
{
    /// <summary>
    /// PlayerInputHandler
    /// </summary>
    public sealed class PlayerInputHandler<T> : InputHandler where T : Actor
    {
        // Constructor
        public PlayerInputHandler(T actor, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            this.Actor = actor;
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
                if (context.HeldItem == null || context.HeldItem.Definition.UsageMode == ItemUsageMode.Default)
                {
                    InteractionData.Clear();
                    Actor.MoveTo(destination);
                }
                return;
            }

            // 2. Outcome interaction: Approach and interact with target using outcome script
            if (context.HeldItem == null || MouseCursor.IsArrow)
            {
                Actor.ApproachAndInteract(context.Target, null);
                return;
            }

            if (context.HeldItem?.Definition.UsageMode == ItemUsageMode.Default)
            {
                if (Actor.ApproachAndInteract(context.Target, context.HeldItem))
                    return;
            }

            MouseCursor.Shake();

            /*
            if (context.Target == null)
            {

                // Walk to destination
                if (context.InteractionType == InteractionType.None)
                {
                    Actor.MoveTo(destination);
                }
                else if (context.Script != null)
                {
                    // Cast item
                    if (context.HeldItem?.Definition.UsageMode == ItemUsageMode.Cast)
                    {
                        Actor.ApproachAndInteract(Actor.Position, context.HeldItem, context.Script);
                    }

                    // Place item
                    else if (context.HeldItem?.Definition.UsageMode == ItemUsageMode.Place)
                    {
                        Actor.ApproachAndInteract(destination, context.HeldItem, context.Script);
                    }
                }

                return;
            }

            if (context.HeldItem != null && !context.CursorOverride.HasValue)
            {
                if (context.Script == null)
                {
                    Sound.Play(SoundNames.Error);
                    MouseCursor.Shake();
                    return;
                }
            }

            if (context.Script != null)
                Actor.ApproachAndInteract(context.Target, context.CursorOverride.HasValue ? null : context.HeldItem, context.Script);
            */
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
