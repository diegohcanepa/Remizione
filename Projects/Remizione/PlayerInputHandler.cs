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
        public PlayerInputHandler(T actor, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            this.Actor = actor;
        }

        #region Private members

        // HandleGamePadInput
        private HandleInputResult HandleGamePadInput()
        {
            // Interaction
            if (Actor.InteractiveTarget != null && InputBindings.Interact.IsPressed(PlayerIndex.One))
            {
                Actor.Interact();
                return HandleInputResult.Handled;
            }

            // Movement
            var direction = GetDirectionVectorFromLeftStick();
            if (direction == Vector2.Zero && InputManager.AllowKeyboard)
                direction = GetDirectionVectorFromKeyboard(InputBindings.KeyboardMoveLeft, InputBindings.KeyboardMoveUp, InputBindings.KeyboardMoveRight, InputBindings.KeyboardMoveDown);

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
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && !InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return HandleInputResult.Unhandled;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                Actor.UseEquippedItem(ItemCategory.Junk);
            else
                Actor.UseEquippedItem(ItemCategory.Gadgets);

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
