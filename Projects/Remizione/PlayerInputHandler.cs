using Engendro;
using Engendro.Audio;
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
            this.Owner = owner;
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

        // ResolveInteraction
        private void ResolveInteraction(Verb verb)
        {
            if (!Owner.IsPlayer)
                return;

            var context = Owner.Session.InteractionContext;

            MouseCursor.PerformClick();

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Owner.Session.Camera);

            // 1. Walk to
            if (context.Target == null)
            {
                Owner.Session.InteractionData.Clear();
                Owner.EnforceTurn = true;

                var walkThreshold = Owner.HasHostilesNearby() ? 0 : GameSettings.WalkThreshold;

                Owner.MoveTo(destination, walkThreshold);

                return;
            }

            // 2. Outcome interaction: Approach and interact with target
            if (context.HeldItem == null || Utils.IsGoToVerb(context.Target.Verb))
            {
                Owner.ResolveInteraction(context.Target, verb, null);
                return;
            }

            if (Owner.ResolveInteraction(context.Target, verb, context.HeldItem))
                return;

            MouseCursor.Shake();
        }

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            var context = Owner.Session.InteractionContext;

            ResolveInteraction(context.Target?.Verb ?? Verb.None);

            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            // Drop held item
            if (Owner.Session.InteractionContext.HeldItem != null)
            {
                MouseCursor.PerformClick(false);
                Owner.StopMoving();
                Sound.Play(SoundNames.Interact);
                Owner.Session.InteractionContext.HeldItem = null;
                Owner.Session.InteractionData.Clear();
            }

            if (Owner.Session.InteractionContext.Target is GameThing target)
            {
                Sound.Play(SoundNames.Interact);
                ResolveInteraction(Verb.Attack);
            }

            return true;
        }

        #endregion

        // HandleInput
        public override HandleInputResult HandleInput()
        {
            return HandleMouseInput();
        }

        // Owner
        public T Owner { get; }
    }
}
