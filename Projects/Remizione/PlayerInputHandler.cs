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
        private int consecutiveActionCount;
        private float actionWindowTimer;
        private const float ActionWindowDuration = 1.2f;
        private const int MaxConsecutiveActions = 3;

        // Constructor
        public PlayerInputHandler(PlayerIndex playerIndex)
            : base(playerIndex)
        {
        }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            if (Owner == null || !Owner.IsInCurrentRoom || Owner.IsDead || Owner.IsFatigued || Owner.Session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (actionWindowTimer > 0)
            {
                actionWindowTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (actionWindowTimer <= 0)
                    consecutiveActionCount = 0;
            }

            var mouse = InputManager.DefaultPlayer.Mouse;

            bool isPrimary = mouse.IsLeftButtonPressed();
            bool isSecondary = mouse.IsRightButtonPressed();

            if (isPrimary || isSecondary)
            {
                if (isPrimary)
                {
                    if (consecutiveActionCount >= MaxConsecutiveActions)
                    {
                        consecutiveActionCount = 0;
                        actionWindowTimer = 0;
                        Owner.Fatigue();
                        return HandleInputResult.Handled;
                    }

                    Owner.Session.InteractionData.ProcessPrimaryAction(Owner);
                }
                else
                {
                    Owner.Session.InteractionData.ProcessSecondaryAction(Owner);
                }

                if (isPrimary && Owner.IsMoving)
                {
                    consecutiveActionCount++;
                    actionWindowTimer = ActionWindowDuration;
                }

                return HandleInputResult.Handled;
            }

            if (InputBindings.Inventory.IsPressed(0))
            {
                Owner.Session.Overlays.ShowInventory();
                return HandleInputResult.Handled;
            }

            if (InputBindings.Map.IsPressed(0))
            {
                Owner.Session.Overlays.ShowMiniMap();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Owner
        public T? Owner { get; set; }

        // WillFatigueOnNextAction
        public bool WillFatigueOnNextAction => Owner != null && actionWindowTimer > 0 && consecutiveActionCount >= MaxConsecutiveActions;
    }
}