using Microsoft.Xna.Framework;
using ScaryCastle.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursorAppearance
    /// </summary>
    internal static class MouseCursorAppearance
    {
        #region Private members

        // RefreshCursor
        private static void RefreshCursor(InteractionContext context)
        {
            if (context.Target == context.Session.Player)
                return;

            // Modal speech text active
            if (SpeechText.ModalInstance != null)
                return;

            // Session is awaiting script
            if (context.Session.IsAwaiting)
            {
                if (context.Session.AwaitingScript?.CurrentStatement is SayCommand)
                    return;

                if (context.Session.ActiveNPC == null)
                {
                    if (context.Session.IsCurrentScene)
                        MouseCursor.State = MouseCursorState.Wait;
                }
                else if (context.Session.ActiveNPC.CombatDecisionType is CombatDecisionType.Attack or CombatDecisionType.Charge or CombatDecisionType.Curse)
                {
                    MouseCursor.State = MouseCursorState.Skull;
                }

                return;
            }

            if (context.HeldItem != null)
            {
                MouseCursor.CustomImage = context.HeldItem.Definition.Image;
            }
            else
            {
                if (context.Target != null)
                    SyncMouseCursor(context.Target.Interaction);
                else
                    MouseCursor.State = MouseCursorState.Cross;
            }
        }

        // SyncMouseCursor
        private static void SyncMouseCursor(InteractionKind interactionKind)
        {
            switch (interactionKind)
            {
                // Attack
                case InteractionKind.Attack:
                    MouseCursor.State = MouseCursorState.Attack;
                    break;

                // Examine
                case InteractionKind.Examine:
                    MouseCursor.State = MouseCursorState.Examine;
                    break;

                // GoDown
                case InteractionKind.GoDown:
                    MouseCursor.State = MouseCursorState.Down;
                    break;

                // GoLeft
                case InteractionKind.GoLeft:
                    MouseCursor.State = MouseCursorState.Left;
                    break;

                // GoRight
                case InteractionKind.GoRight:
                    MouseCursor.State = MouseCursorState.Right;
                    break;

                // GoUp
                case InteractionKind.GoUp:
                    MouseCursor.State = MouseCursorState.Up;
                    break;

                // Lift
                case InteractionKind.Lift:
                    MouseCursor.State = MouseCursorState.Lift;
                    break;

                // Talk
                case InteractionKind.Talk:
                    MouseCursor.State = MouseCursorState.Talk;
                    break;

                // Use
                case InteractionKind.Use:
                    MouseCursor.State = MouseCursorState.Hand;
                    break;

                default:
                    break;
            }
        }

        #endregion

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            MouseCursor.Reset();

            RefreshCursor(context);

            MouseCursor.Color = Color.White;

            if (context.Target is Actor actor)
            {
                if (context.HeldItem == null && actor.IsHostile && actor.IsAlert)
                    MouseCursor.Color = ColorPalette.MouseCursor.HostileTarget;

                /*
                if (context.Session.Player != null && context.HeldItem?.Definition.ActionKind == ActionKind.Projectile)
                {
                    var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(context.Session.Camera);
                    MouseCursor.FlipCustomImage = mousePos.X < context.Session.Player.X;
                }
                */
            }
        }
    }
}
