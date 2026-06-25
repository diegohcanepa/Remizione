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
                    SyncMouseCursor(context.Target.Verb);
                else
                    MouseCursor.State = MouseCursorState.Cross;
            }
        }

        // SyncMouseCursor
        private static void SyncMouseCursor(Verb verb)
        {
            switch (verb)
            {
                // Attack
                case Verb.Attack:
                    MouseCursor.State = MouseCursorState.Attack;
                    break;

                // Examine
                case Verb.Examine:
                    MouseCursor.State = MouseCursorState.Examine;
                    break;

                // GoDown
                case Verb.GoDown:
                    MouseCursor.State = MouseCursorState.Down;
                    break;

                // GoLeft
                case Verb.GoLeft:
                    MouseCursor.State = MouseCursorState.Left;
                    break;

                // GoRight
                case Verb.GoRight:
                    MouseCursor.State = MouseCursorState.Right;
                    break;

                // GoUp
                case Verb.GoUp:
                    MouseCursor.State = MouseCursorState.Up;
                    break;

                // Lift
                case Verb.Lift:
                    MouseCursor.State = MouseCursorState.Lift;
                    break;

                // Talk
                case Verb.Talk:
                    MouseCursor.State = MouseCursorState.Talk;
                    break;

                // Use
                case Verb.Use:
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
