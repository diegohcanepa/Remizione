using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engendro.Input
{
    /// <summary>
    /// PlayerInputManager
    /// </summary>
    public sealed class PlayerInputManager
    {
        private static readonly List<PlayerIndex> assignedGamePads = [];

        // Constructor
        internal PlayerInputManager(int playerNumber)
        {
            this.PlayerNumber = playerNumber;
            this.GamePad = new GamePadDevice(this);
            this.Keyboard = new KeyboardDevice(this);
            this.Mouse = new MouseDevice(this);

            AssignAvailableGamepad();

            if (GamePad.IsConnected)
                LastInputMethod = InputMethod.GamePad;
        }

        #region Private members

        // AssignAvailableGamepad
        private void AssignAvailableGamepad()
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerIndex index = (PlayerIndex)i;
                if (Microsoft.Xna.Framework.Input.GamePad.GetState(index).IsConnected && !assignedGamePads.Contains(index))
                {
                    AssignedGamepad = index;
                    assignedGamePads.Add(index);
                    return;
                }
            }
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            // Check if gamepad is disconnected and assign a new one
            if (AssignedGamepad.HasValue)
            {
                if (!GamePad.IsConnected)
                {
                    if (InputManager.AutoAssignGamepad)
                    {
                        assignedGamePads.Remove(AssignedGamepad.Value);
                        AssignedGamepad = null;
                        Reset();
                        AssignAvailableGamepad();
                    }
                }
            }
            else if (InputManager.AutoAssignGamepad)
            {
                AssignAvailableGamepad();
            }

            // Mouse & Keyboard
            if (PlayerNumber == 0)
            {
                Mouse.Update(gameTime);
                Keyboard.Update(gameTime);

                if (Mouse.HasInput())
                {
                    if (InputManager.AllowMouse)
                        LastInputMethod = InputMethod.Mouse;
                }

                else if (Keyboard.HasInput())
                {
                    LastInputMethod = InputMethod.Keyboard;
                    InputManager.AllowMouse = true;
                }
            }

            // GamePad
            GamePad.Update(gameTime);
            if (GamePad.HasInput())
            {
                InputManager.AllowMouse = false;
                LastInputMethod = InputMethod.GamePad;
            }
        }

        #endregion

        // AssignedGamepad
        public PlayerIndex? AssignedGamepad { get; private set; }

        // GamePad
        public GamePadDevice GamePad { get; }

        // Keyboard
        public KeyboardDevice Keyboard { get; }

        // LastInputMethod
        public InputMethod LastInputMethod { get; private set; }

        // Mouse
        public MouseDevice Mouse { get; }

        // PlayerNumber
        public int PlayerNumber { get; }

        // Reset
        public void Reset()
        {
            Keyboard.Reset();
            GamePad.Reset();
            Mouse.Reset();
        }
    }
}