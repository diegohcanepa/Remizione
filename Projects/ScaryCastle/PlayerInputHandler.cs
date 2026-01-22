using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
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

        // HandleInput
        private HandleInputResult HandleInput()
        {
            // Interaction
            if (MouseCursor.Target != null && InputBindings.Interact.IsPressed(PlayerIndex.One))
            {
                Actor.Interact();
                return HandleInputResult.Handled;
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
            if (MouseCursor.State == MouseCursorState.Hand)
                return false;

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            if (MouseCursor.Target != null)
            {
                if (MouseCursor.UseWithScript is Script script)
                {
                    MouseCursor.Item = null;
                    Actor.Session.BeginOutcome(script, MouseCursor.Target);
                }
                else
                {
                    if (MouseCursor.Item != null)
                    {
                        Actor.Session.AwaitRoutine(RoutineNames.UseWithFailOutcome);
                    }
                    else
                    {
                        Actor.ApproachAndInteract(MouseCursor.Target);
                    }
                }

                MouseCursor.PerformClick();

                return true;
            }
            else
            {
                MouseCursor.AnimateClick();
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);
                Actor.MoveTo(destination);
                return true;
            }
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            if (MouseCursor.Item != null)
            {
                Sound.Play(SoundNames.Interact);
                MouseCursor.Item = null;
            }

            return true;
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
                return HandleInput();
        }
    }
}
