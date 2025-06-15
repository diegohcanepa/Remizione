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
        //private bool inventoryLocked;
        private InventoryScene inventoryScene;

        // Constructor
        public PlayerInputHandler(T actor, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            this.Actor = actor;
            this.inventoryScene = new(actor);
        }

        #region Private members

        // HandleGamePadInput
        private HandleInputResult HandleGamePadInput()
        {
            // CloseAttack
            if (InputBindings.CloseAttack.IsPressed(PlayerIndex.One))
            {
                Actor.CloseAttack();
                return HandleInputResult.Handled;
            }

            // Inventory
            if (InputBindings.ShowInventory.IsPressed(PlayerIndex.One) && !inventoryScene.IsCurrentScene)
            {
                inventoryScene.SceneController.Push();
                return HandleInputResult.Handled;
            }

            // Interaction
            if (Actor.InteractiveTarget != null && InputBindings.Interact.IsPressed(PlayerIndex.One))
            {
                Actor.Interact();
                return HandleInputResult.Handled;
            }
            
            // Movement
            var direction = GetDirectionVectorFromLeftStick();
            if (direction != Vector2.Zero)
            {
                Actor.Move(direction);
            }
            else if (Actor.IsMoving)
            {
                Actor.Stand();
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

            MouseCursor.Instance.AnimateClick();

            if (Actor.InteractiveTarget != null)
            {
                Actor.ApproachAndInteract(Actor.InteractiveTarget);
            }
            else
            {
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);
                Actor.MoveTo(destination);
            }

            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            var result = InputManager.DefaultPlayer.Mouse.IsRightButtonPressed();
            return result;
        }

        #endregion

        // Actor
        public T Actor { get; }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                return HandleMouseInput();

            else if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad)
                return HandleGamePadInput();

            return HandleInputResult.Unhandled;
        }
    }
}
