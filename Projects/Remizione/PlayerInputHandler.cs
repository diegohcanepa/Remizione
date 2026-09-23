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
            Owner = owner;
        }

        // HandleInput
        public override HandleInputResult HandleInput()
        {
            var mouse = InputManager.DefaultPlayer.Mouse;

            // Left button
            if (mouse.IsLeftButtonPressed())
            {
                Owner.Session.InteractionData.ProcessPrimaryAction(Owner);
                return HandleInputResult.Handled;
            }

            // Rigght button
            if (mouse.IsRightButtonPressed())
            {
                Owner.Session.InteractionData.ProcessSecondaryAction(Owner);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Owner
        public T Owner { get; }
    }
}