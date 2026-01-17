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

        // HandleInput
        private HandleInputResult HandleInput()
        {
            // Interaction
            if (Actor.InteractiveTarget != null && InputBindings.Interact.IsPressed(PlayerIndex.One))
            {
                Actor.Interact();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

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

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            MouseCursor.AnimateClick();

            if (Actor.InteractiveTarget != null)
            {
                Actor.ApproachAndInteract(Actor.InteractiveTarget);
                return true;
            }
            else
            {
                Actor.Session.Inventory.HeldItem = null;
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);
                Actor.MoveTo(destination);
                return true;
            }
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            if (Actor.Session.Inventory.HeldItem == null)
                Actor.Session.ShowInventory();
            else
            {
                Sound.Play(SoundNames.UISelectD);
                Actor.Session.Inventory.HeldItem = null;
            }

            return true;
        }

        #endregion

        // Actor
        public T Actor { get; }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                return HandleMouseInput();
            else
                return HandleInput();
        }
    }
}
