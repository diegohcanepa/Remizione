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

        // HandleCombatModeInput
        private bool HandleCombatModeInput(Vector2 destination)
        {
            if (Actor.InteractiveTarget == null || !Actor.InteractiveTarget.CanBeTargeted)
            {
                Actor.DoMoveTurn(destination);
                return true;
            }
            else if (Actor.CanBeTargeted)
            {
                Actor.DoAttackTurn();
                return true;
            }

            return false;
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

            if (!Actor.CanChangeState)
                return false;

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);

            MouseCursor.Instance.AnimateClick();

            if (Actor.Session.CombatManager.IsActive)
                return HandleCombatModeInput(destination);

            Actor.FastMove = true;
            if (Actor.InteractiveTarget != null)
                Actor.ApproachAndInteract(Actor.InteractiveTarget);
            else
                Actor.MoveTo(destination);

            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            var result = InputManager.DefaultPlayer.Mouse.IsRightButtonPressed();

            if (result)
            {
                if (Actor.Session.CombatManager.TurnList.Count <= 1)
                {
                    if (Actor.Session.CombatManager.IsActive)
                        Actor.Session.CombatManager.Terminate();
                    else
                        Actor.Session.CombatManager.Start(Actor);
                }
                else
                    MouseCursor.Instance.Shake();
            }

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
