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
        private bool inventoryLocked;
        private readonly InventoryScene inventoryScene;

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
                Actor.PerformCloseAttack();
                return HandleInputResult.Handled;
            }

            // Use item
            if (InputBindings.UseItem.IsPressed(PlayerIndex.One))
            {
                Actor.UseCurrentInventoryItem();
                return HandleInputResult.Handled;
            }

            // Inventory
            if (inventoryLocked)
            {
                if (!InputBindings.Inventory.IsPressed(PlayerIndex.One))
                {
                    inventoryLocked = false;
                    return HandleInputResult.Handled;
                }
            }
            else if (Actor.Session.InventoryEnabled && InputBindings.Inventory.IsPressed(PlayerIndex.One))
            {
                inventoryLocked = true;
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
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad)
            {
                var direction = GetDirectionVectorFromLeftStick();
                if (direction != Vector2.Zero)
                {
                    Actor.Move(direction);
                }
                else if (Actor.IsMoving)
                {
                    Actor.Stand();
                }
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

            // Use item
            if (result)
                Actor.UseCurrentInventoryItem();

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
            else
                return HandleGamePadInput();
        }
    }
}
