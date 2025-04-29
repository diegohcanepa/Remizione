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

        // Constructor
        public PlayerInputHandler(T actor, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            this.Actor = actor;
        }

        #region Private members

        // Interact
        private bool Interact()
        {
            if (Actor.InteractionTarget != null)
            {
                var interact = false;

                // Triggered by player
                if (InputBindings.Interact.IsPressed(0))
                    interact = true;

                if (interact)
                    Actor.Interact(null);
            }

            return false;
        }

        // PerformMoveAction
        private void PerformMoveAction(bool fastMove, bool attack)
        {
            if (!Actor.CanPerformAction)
                return;

            if (Actor.IsActiveCombatant)
                Actor.Session.CombatManager.IsTurnInProgress = true;

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);
            GameThing? interactionTarget = Actor.InteractionTarget;

            Actor.FastMove = fastMove;
            if (interactionTarget != null)
                Actor.ApproachAndInteract(interactionTarget, attack);
            else
                Actor.MoveTo(destination);

            if (Actor.FollowingPathDestination.HasValue)
            {
                Actor.Session.HUD.DestinationMark.Color = interactionTarget != null ? ColorPalette.DestinationMark.Target : ColorPalette.DestinationMark.Default;
                Actor.Session.HUD.DestinationMark.Position = Actor.FollowingPathDestination;
            }
            
            Actor.Session.HUD.EchoMessage.Hide();
        }

        // TestInventory
        private bool TestInventory()
        {
            if (inventoryLocked)
            {
                if (InputBindings.ShowInventory.IsUp(PlayerIndex.One))
                    inventoryLocked = false;
            }
            else if (InputBindings.ShowInventory.IsPressed(PlayerIndex.One))
            {
                inventoryLocked = true;
                Actor.Session.ShowInventory();
                return true;
            }

            return false;
        }

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            PerformMoveAction(true, false);

            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            PerformMoveAction(true, true);

            return true;
        }

        #endregion

        // Actor
        public T Actor { get; }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            // Mouse left button
            if (TestMouseLeftButtonClick())
                return HandleInputResult.Handled;

            // Mouse right button
            if (TestMouseRightButtonClick())
                return HandleInputResult.Handled;

            /*
            if (Actor.Session.FullHUD)
            {
                if (TestInventory())
                    return HandleInputResult.Handled;
            }
            */

            /*
            // Get direction from keyboard -or- left stick
            if (!Actor.IsFollowingPath)
            {
                var direction = GetDirectionVectorFromLeftStick();
                if (direction == Vector2.Zero && PlayerInputManager.PlayerNumber == 0)
                    direction = GetDirectionVectorFromKeyboard(InputBindings.KeyboardMoveLeft, InputBindings.KeyboardMoveUp, InputBindings.KeyboardMoveRight, InputBindings.KeyboardMoveDown);

                if (direction == Vector2.Zero)
                    Actor.Stand();
                else
                    Actor.Move(direction);
            }
            */

            if (Interact())
                return HandleInputResult.Handled;

            return HandleInputResult.Handled;
        }
    }
}
