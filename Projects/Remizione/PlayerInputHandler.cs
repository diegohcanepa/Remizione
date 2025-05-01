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

        // Constructor
        public PlayerInputHandler(T actor, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            this.Actor = actor;
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

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            if (!Actor.CanPerformAction)
                return false;

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);

            MouseCursor.Instance.AnimateClick();

            if (Actor.Session.CombatMode && Actor.InteractionTarget != null)
            {
                Actor.StartCombatTurn();
                return true;
            }

            Actor.FastMove = true;
            if (Actor.InteractionTarget != null)
                Actor.ApproachAndInteract(Actor.InteractionTarget);
            else
                Actor.MoveTo(destination);

            if (Actor.FollowingPathDestination.HasValue)
            {
                Actor.Session.HUD.DestinationMark.Color = Actor.InteractionTarget != null ? ColorPalette.DestinationMark.Target : ColorPalette.DestinationMark.Default;
                Actor.Session.HUD.DestinationMark.Position = Actor.FollowingPathDestination;
            }

            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            var result = InputManager.DefaultPlayer.Mouse.IsRightButtonPressed();
            
            if (result)
                Actor.Session.CombatMode = !Actor.Session.CombatMode;

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

            return HandleInputResult.Unhandled;
        }
    }
}
