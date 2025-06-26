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
            if (Actor.Session.InventoryEnabled && InputBindings.Inventory.IsPressed(PlayerIndex.One))
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
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && !InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return HandleInputResult.Unhandled;

            MouseCursor.Instance.AnimateClick();

            if (Actor.InteractiveTarget != null)
            {
                Actor.ApproachAndInteract(Actor.InteractiveTarget, InputManager.DefaultPlayer.Mouse.IsRightButtonPressed());
                return HandleInputResult.Handled;
            }
            else
            {
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);
                Actor.MoveTo(destination);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
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
