using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engendro.Input
{
    /// <summary>
    /// InputBinding
    /// </summary>
    public sealed class InputBinding : INamedObject
    {
        // Constructor
        public InputBinding(string name, Buttons? button, MouseButton mouseButton, Keys[] keys)
        {
            this.Name = name;
            this.Button = button;
            this.Keys = keys;
            this.MouseButton = mouseButton;
        }

        // CanTestInput
        private static bool CanTestInput() => EngendroGame.Instance != null && EngendroGame.Instance.IsActive && !InputManager.IsSuspended;

        // Button
        public Buttons? Button { get; set; }

        // IsDown
        public bool IsDown(PlayerIndex playerIndex)
        {
            if (!CanTestInput())
            {
                return false;
            }
            else
            {
                // Gamepad
                var result = Button != null && InputManager.Players[(int)playerIndex].GamePad.IsButtonDown(Button.Value);

                // Mouse
                if (!result && InputManager.AllowMouse && MouseButton != MouseButton.None)
                {
                    result = MouseButton == MouseButton.Left && InputManager.DefaultPlayer.Mouse.IsLeftButtonDown() ||
                             MouseButton == MouseButton.Right && InputManager.DefaultPlayer.Mouse.IsRightButtonDown();
                }

                // Keyboard
                if (!result)
                {
                    result = IsKeyDown() &&
                             (!RequiresAlt || InputManager.DefaultPlayer.Keyboard.IsAltDown()) &&
                             (!RequiresControl || InputManager.DefaultPlayer.Keyboard.IsControlDown()) &&
                             (!RequiresShift || InputManager.DefaultPlayer.Keyboard.IsShiftDown());
                }

                return result;
            }
        }

        // IsEmpty
        public bool IsEmpty => Button == 0 && (Keys.Length == 0 || Keys[0] != Microsoft.Xna.Framework.Input.Keys.None);

        // IsKeyDown
        public bool IsKeyDown()
        {
            if (!CanTestInput())
            {
                return false;
            }
            else if (Keys.Length == 0)
            {
                return false;
            }
            else if (Keys.Length == 1)
            {
                return InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys[0]);
            }

            for (var i = 0; i < Keys.Length; i++)
            {
                if (InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys[i]))
                    return true;
            }

            return false;
        }

        // IsKeyPressed
        public bool IsKeyPressed()
        {
            if (!CanTestInput())
                return false;

            else if (Keys.Length == 0)
                return false;

            else if (Keys.Length == 1)
                return InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys[0]);

            for (var i = 0; i < Keys.Length; i++)
            {
                if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys[i]))
                    return true;
            }

            return false;
        }

        // IsKeyUp
        public bool IsKeyUp()
        {
            if (!CanTestInput())
                return false;

            else if (Keys.Length == 0)
                return false;

            else if (Keys.Length == 1)
                return InputManager.DefaultPlayer.Keyboard.IsKeyUp(Keys[0]);

            for (var i = 0; i < Keys.Length; i++)
            {
                if (InputManager.DefaultPlayer.Keyboard.IsKeyUp(Keys[i]))
                    return true;
            }

            return false;
        }

        // IsPressed
        public bool IsPressed(PlayerIndex playerIndex)
        {
            if (!CanTestInput())
                return false;
            else
            {
                // Gamepad
                var result = Button != null && InputManager.Players[(int)playerIndex].GamePad.IsButtonPressed(Button.Value);

                // Mouse
                if (!result && InputManager.AllowMouse && MouseButton != MouseButton.None)
                {
                    result = MouseButton == MouseButton.Left && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() ||
                             MouseButton == MouseButton.Right && InputManager.DefaultPlayer.Mouse.IsRightButtonPressed();
                }

                // Keyboard
                if (!result)
                {
                    result = IsKeyPressed() &&
                             (!RequiresAlt || InputManager.DefaultPlayer.Keyboard.IsAltDown()) &&
                             (!RequiresControl || InputManager.DefaultPlayer.Keyboard.IsControlDown()) &&
                             (!RequiresShift || InputManager.DefaultPlayer.Keyboard.IsShiftDown());
                }

                return result;
            }
        }

        // IsUp2
        public bool IsUp2(PlayerIndex playerIndex)
        {
            if (!CanTestInput())
                return false;
            else
                return Button != null && (InputManager.Players[(int)playerIndex].GamePad.IsButtonUp(Button.Value) || IsKeyUp());
        }

        // IsUp
        public bool IsUp(PlayerIndex playerIndex)
        {
            if (!CanTestInput())
            {
                return false;
            }
            else
            {
                // Gamepad
                var result = Button != null && InputManager.Players[(int)playerIndex].GamePad.IsButtonUp(Button.Value);

                // Mouse
                if (!result && InputManager.AllowMouse && MouseButton != MouseButton.None)
                {
                    result = MouseButton == MouseButton.Left && InputManager.DefaultPlayer.Mouse.IsLeftButtonUp() ||
                             MouseButton == MouseButton.Right && InputManager.DefaultPlayer.Mouse.IsRightButtonUp();
                }

                // Keyboard
                if (!result)
                    result = IsKeyUp();

                return result;
            }
        }


        // Keys
        public Keys[] Keys { get; set; }

        // MouseButton
        public MouseButton MouseButton { get; set; }

        // Name
        public string Name { get; }

        // RequiresAlt
        public bool RequiresAlt { get; set; }

        // RequiresControl
        public bool RequiresControl { get; set; }

        // RequiresShift
        public bool RequiresShift { get; set; }

        // ToString
        public override string ToString() => Name;
    }
}
