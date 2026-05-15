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

        // PerformInteraction
        private void PerformInteraction()
        {
            if (!Actor.IsPlayer)
                return;

            var context = Actor.Session.InteractionContext;

            MouseCursor.PerformClick();

            var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(Actor.Session.Camera);

            // 2. No target: Basic walk to destination
            if (context.Target == null)
            {
                Actor.Session.InteractionData.Clear();
                Actor.MoveTo(destination);
                return;
            }

            // 3. Outcome interaction: Approach and interact with target using outcome script
            if (context.HeldItem == null || MouseCursor.IsArrow)
            {
                Actor.ApproachAndInteract(context.Target, null);
                return;
            }

            // 4. Classic "Use with" interaction: Approach and interact with target using held item
            if (context.HeldItem.Definition.GooCost == 0)
            {
                if (Actor.ApproachAndInteract(context.Target, context.HeldItem))
                    return;
            }
            else
            {
                if (Actor.Goo == 0)
                {
                    Actor.Session.TextHUD.Message.Show(MessageKind.NotEnoughGoo);
                }
                else
                {
                    Actor.Goo--;
                    if (Actor.Cast(context.Target, context.HeldItem))
                        return;
                }
            }

            MouseCursor.Shake();
        }

        // TestMouseLeftButtonClick
        private bool TestMouseLeftButtonClick()
        {
            if (MouseCursor.State == MouseCursorState.Hand && MouseCursor.CustomImage == null)
                return false;

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            /*
            if (Actor.Session.InteractionContext.AttackMode && !MouseCursor.IsArrow)
            {
                if (Actor.Session.InteractionContext.Target != null && !Actor.Session.InteractionContext.Target.CanBeHit)
                {
                    MouseCursor.Shake();
                    return false;
                }
            }
            */

            if (Actor.Session.InteractionContext.Target?.Faction == Faction.Evil)
                Actor.Session.InteractionContext.Mode = InteractionContextMode.Attack;

            PerformInteraction();

            return true;
        }

        // TestMouseRightButtonClick
        private bool TestMouseRightButtonClick()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
                return false;

            if (Actor.ActiveThrowable != null)
            {
                Actor.ThrowActiveTrowable();
                return true;
            }

            // Drop held item
            if (Actor.Session.InteractionContext.HeldItem != null)
            {
                Sound.Play(SoundNames.Interact);
                Actor.Session.InteractionContext.HeldItem = null;
                Actor.Session.InteractionData.Clear();
                Actor.StopMoving();
            }
            else if (!MouseCursor.IsArrow && Actor.Session.InteractionContext.Target is Prop prop)
            {
                if (prop.IsLiftable)
                {
                    Actor.Session.InteractionContext.Mode = InteractionContextMode.Lift;
                    PerformInteraction();
                }
                else
                {
                    Actor.Session.TextHUD.Message.Show(MessageKind.LiftNotAllowed);
                    MouseCursor.Shake();
                }
            }
            else
            {
                MouseCursor.Shake();
            }

            return true;
        }

        #endregion

        // Actor
        public T Actor { get; }

        // HandleInput
        public override HandleInputResult HandleInput()
        {
            return HandleMouseInput();
        }
    }
}
