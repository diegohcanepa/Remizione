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

        // ResolveInteraction
        private void ResolveInteraction()
        {
            if (!Actor.IsPlayer)
                return;

            var context = Actor.Session.InteractionContext;

            MouseCursor.PerformClick();

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);

            // 1. Walk to
            if (context.Target == null)
            {
                Actor.Session.InteractionData.Clear();
                Actor.EnforceTurn = true;
                Actor.MoveTo(destination);
                return;
            }

            // 2. Outcome interaction: Approach and interact with target
            if (context.HeldItem == null || context.Target.HasArrowCursor)
            {
                Actor.ResolveInteraction(context.Target, null);
                return;
            }

            // 3. Lift action
            if (context.HeldItem.Name == ItemNames.Lift)
            {
                if (context.Target is not Prop prop || !prop.IsLiftable)
                {
                    Actor.Session.TextHUD.Message.Show(MessageKind.LiftNotAllowed);
                    MouseCursor.Shake();
                    return;
                }
            }

            // 4. Check for goo if item requires it
            var gooCost = context.HeldItem.Definition.EnergyCost;
            if (gooCost > 0)
            {
                if (Actor.Energy < gooCost)
                {
                    Actor.Session.TextHUD.Message.Show(MessageKind.NotEnoughGoo);
                    return;
                }
            }

            if (Actor.ResolveInteraction(context.Target, context.HeldItem))
                return;

            MouseCursor.Shake();
        }

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            //if (MouseCursor.State == MouseCursorState.Hand && MouseCursor.CustomImage == null)
              //  return false;

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            ResolveInteraction();

            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            MouseCursor.PerformClick();

            // Drop throwable
            if (Actor.ActiveThrowable != null)
            {
                Actor.DiscardActiveThrowable();
                return true;
            }

            Actor.StopMoving();

            // Drop held item
            if (Actor.Session.InteractionContext.HeldItem != null)
            {
                Sound.Play(SoundNames.Interact);
                Actor.Session.InteractionContext.HeldItem = null;
                Actor.Session.InteractionData.Clear();
            }
            else
            {
                Actor.Session.ShowActions();
            }

            return true;
        }

        #endregion

        // Actor
        public T Actor { get; }

        // HandleInput
        public override HandleInputResult HandleInput()
        {
            return HandleMouseInput();
        }
    }
}
