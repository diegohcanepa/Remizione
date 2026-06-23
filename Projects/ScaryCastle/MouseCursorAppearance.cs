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
            /*
            if (context.HeldItem?.Definition.ActionKind == ActionKind.Projectile)
            {
                if (context.Session.Player != null)
                {
                    var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(context.Session.Camera);
                    MouseCursor.FlipCustomImage = mousePos.X < context.Session.Player.X;
                }
            }
            */

            // DialogOption
            if (context.Session.Game.SceneManager.CurrentScene is DialogBlockScene)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                return;
            }

            // Modal speech text active
            if (SpeechText.ModalInstance != null)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                return;
            }

            // Session is awaiting script
            if (context.Session.IsAwaiting)
            {
                if (context.Session.AwaitingScript?.CurrentStatement is AwaitInputCommand)
                {
                    MouseCursor.State = MouseCursorState.Hand;
                }
                else if (context.Session.AwaitingScript?.CurrentStatement is SayCommand)
                {
                    MouseCursor.State = MouseCursorState.Arrow;
                }
                else if (context.Session.ActiveNPC == null)
                {
                    MouseCursor.State = MouseCursorState.Wait;
                }
                else if (context.Session.IsAwaiting)
                {
                    MouseCursor.State = MouseCursorState.Skull;
                }

                return;
            }

            if (context.HeldItem != null && (context.Target == null || context.Target.Cursor == MouseCursorState.Cross))
            {
                MouseCursor.CustomImage = context.HeldItem.Definition.Image;
            }
            else
            {
                MouseCursor.State = context.Target != null ? context.Target.Cursor : MouseCursorState.Cross;
            }
        }

        // RefreshText
        private static void RefreshText(InteractionContext context)
        {
            if (MouseCursor.IsArrow)
                return;

            if (context.Target != null)
            {
                if (context.Target.Faction == Faction.Evil && context.Target is Actor actor)
                {
                    MouseCursor.HealthAmount = actor.HP;

                    if (actor != context.Session.Player)
                        context.Session.StatusHUD.ThingInfo.Actor = actor;
                    else
                        context.Session.StatusHUD.ThingInfo.Actor = null;
                }

                // No item 
                MouseCursor.Text = context.Target.DisplaySentence;
            }
            else
            {
                context.Session.StatusHUD.ThingInfo.Actor = null;
            }
        }

        #endregion

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            MouseCursor.Reset();

            RefreshCursor(context);
            RefreshText(context);

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
