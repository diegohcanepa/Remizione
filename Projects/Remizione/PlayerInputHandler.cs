using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// PlayerInputHandler
    /// </summary>
    public sealed class PlayerInputHandler<T> : InputHandler where T : Actor
    {
        // Constructor
        public PlayerInputHandler(T owner, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            this.Owner = owner;
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
            if (!Owner.IsPlayer)
                return;

            var context = Owner.Session.InteractionContext;

            MouseCursor.PerformClick();

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Owner.Session.Camera);

            // 1. Walk to
            if (context.Target == null)
            {
                Owner.Session.InteractionData.Clear();
                Owner.EnforceTurn = true;

                var walkThreshold = Owner.HasHostilesNearby() ? 0 : GameSettings.WalkThreshold;

                Owner.MoveTo(destination, walkThreshold);

                return;
            }

            // 2. Outcome interaction: Approach and interact with target
            if (context.HeldItem == null || context.Target.IsGoToVerb)
            {
                Owner.ResolveInteraction(context.Target, null);
                return;
            }

            // 3. Lift action
            if (context.HeldItem.Name == ItemNames.Lift)
            {
                if (context.Target is not Prop prop || !prop.IsLiftable)
                {
                    Owner.Session.HUD?.Message.Show(MessageKind.LiftNotAllowed);
                    MouseCursor.Shake();
                    return;
                }
            }

            if (Owner.ResolveInteraction(context.Target, context.HeldItem))
                return;

            MouseCursor.Shake();
        }

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
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

            // Drop throwable
            if (Owner.ActiveThrowable != null)
            {
                MouseCursor.PerformClick();
                Owner.DropActiveThrowable();
                return true;
            }

            Owner.StopMoving();

            // Drop held item
            if (Owner.Session.InteractionContext.HeldItem != null)
            {
                MouseCursor.PerformClick(false);
                Owner.Session.InteractionContext.HeldItem = null;
                Owner.Session.InteractionData.Clear();
            }
            else if (Owner.Session.InventoryEnabled)
            {
                MouseCursor.PerformClick();
                Owner.Session.ShowInventory();
            }

            return true;
        }

        #endregion

        // HandleInput
        public override HandleInputResult HandleInput()
        {
            return HandleMouseInput();
        }

        // Owner
        public T Owner { get; }
    }
}
