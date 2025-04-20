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
        private void PerformMoveAction(bool fastMove)
        {
            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);
            GameThing? moveToTarget = null;

            if (Actor.Room is GameRoom room)
            {
                for (int i = room.CulledThings.Count - 1; i >= 0; i--)
                {
                    if (Actor.Session.Player == room.CulledThings[i])
                        continue;

                    else if (room.CulledThings[i] is GameThing target && target.HotspotBox.Contains(destination))
                    {
                        destination = target.GetApproachPosition(Actor);
                        moveToTarget = target;
                        break;
                    }
                }
            }

            if (Actor.IsMoving)
                Actor.Stand();

            Actor.FastMove = fastMove;
            Actor.MoveTo(destination, moveToTarget);
            if (Actor.FollowingPathDestination.HasValue)
            {
                Actor.Session.HUD.DestinationMark.Color = moveToTarget != null ? ColorPalette.DestinationMark.Target : ColorPalette.DestinationMark.Default;
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

        // TestQuickSlots
        private bool TestQuickSlots(GameTime gameTime)
        {
            return Actor.Session.HUD.QuickSlots.HandleInput(gameTime) == HandleInputResult.Handled;
        }

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            PerformMoveAction(false);
            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            PerformMoveAction(true);
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
                if (TestInventory() || TestQuickSlots(gameTime))
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
