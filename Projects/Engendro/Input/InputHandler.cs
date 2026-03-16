using Microsoft.Xna.Framework;

namespace Engendro.Input
{
    /// <summary>
    /// InputHandler
    /// </summary>
    public abstract class InputHandler(PlayerIndex playerIndex)
    {
        #region Protected members

        // GetDirectionVectorFromLeftStick
        protected Vector2 GetDirectionVectorFromLeftStick()
        {
            var direction = PlayerInputManager.GamePad.LeftThumbStickPosition;

            if (direction.X.IsBetween(-.05f, .05f))
                direction.X = 0;

            if (direction.Y.IsBetween(-.05f, .05f))
                direction.Y = 0;

            if (direction != Vector2.Zero)
                return Vector2.Normalize(direction * InvertYAxis);
            else
                return direction;
        }

        // GetDirectionVectorFromKeyboard
        protected static Vector2 GetDirectionVectorFromKeyboard(InputBinding left, InputBinding up, InputBinding right, InputBinding down)
        {
            if (!InputManager.AllowKeyboard)
                return Vector2.Zero;

            // Movement
            float x = 0;
            float y = 0;

            // Up
            if (up.IsKeyDown())
            {
                y = -1;
            }

            // Down
            else if (down.IsKeyDown())
            {
                y = 1;
            }

            // Left
            if (left.IsKeyDown())
            {
                x = -1;
            }

            // Right
            else if (right.IsKeyDown())
            {
                x = 1;
            }

            if (x != 0 || y != 0)
                return Vector2.Normalize(new Vector2(x, y));
            else
                return Vector2.Zero;
        }

        // InvertYAxis
        protected static readonly Vector2 InvertYAxis = new(1, -1);

        // PlayerInputManager
        protected PlayerInputManager PlayerInputManager { get; } = InputManager.Players[(int)playerIndex];

        #endregion

        // HandleInput
        public abstract HandleInputResult HandleInput();
    }
}
