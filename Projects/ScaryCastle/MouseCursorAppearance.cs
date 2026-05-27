using Engendro;
using Engendro.Input;
using ScaryCastle.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursorAppearance
    /// </summary>
    internal static class MouseCursorAppearance
    {
        #region Private fields

        private static readonly string useVerb = Localization.GetValue(Verb.Use);
        private static readonly string withPreposition = TextRepository.GetValue("Misc.WithPreposition");

        #endregion

        #region Private members

        // RefreshCursor
        private static void RefreshCursor(InteractionContext context)
        {
            if (context.HeldItem?.Definition.UsageScope == ItemUsageScope.Projectile)
            {
                if (context.Session.Player != null)
                {
                    var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(context.Session.Camera);
                    MouseCursor.FlipCustomImage = mousePos.X < context.Session.Player.X;
                }
            }

            // Inventory
            if (context.Session.Game.SceneManager.CurrentScene is InventoryScene)
            {
                MouseCursor.State = MouseCursorState.Hand;
                return;
            }

            // DialogOption
            if (context.Session.Game.SceneManager.CurrentScene is DialogBlockScene)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                return;
            }

            // Modal speech bubble active
            if (SpeechBubble.ModalInstance != null)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                return;
            }

            // Session is awaiting script
            if (context.Session.IsAwaiting)
            {
                if (context.Session.AwaitingScript?.CurrentStatement is AwaitInputCommand)
                    MouseCursor.State = MouseCursorState.Hand;
                else if (context.Session.AwaitingScript?.CurrentStatement is SayCommand)
                    MouseCursor.State = MouseCursorState.Arrow;
                else
                    MouseCursor.State = MouseCursorState.Wait;

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
                // No item 
                if (context.HeldItem == null)
                {
                    MouseCursor.Text = context.Target.DisplaySentence;
                    return;
                }

                if (context.Target == context.Session.Player)
                    MouseCursor.Text = $"{useVerb} {context.HeldItem.Definition.DisplayName}";
                else
                    MouseCursor.Text = $"{useVerb} {context.HeldItem.Definition.DisplayName} {withPreposition} {context.Target.DisplayName}";
            }
        }

        #endregion

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            MouseCursor.Reset();

            RefreshCursor(context);
            RefreshText(context);

            if (context.Target != null)
            {
                if (context.Target.Faction == Faction.Evil && context.HeldItem == null)
                    MouseCursor.Color = ColorPalette.MouseCursor.AttackableTarget;

                MouseCursor.ShowLiftIcon = context.HeldItem == null && context.Target is Prop prop && prop.IsLiftable;
                MouseCursor.IsEnabled = context.Session.Player?.ActiveThrowable == null;
                MouseCursor.HightlightColor = ColorPalette.MouseCursor.HighlightWhite;

                if (context.Session.Player != null && context.HeldItem?.Definition.UsageScope == ItemUsageScope.Projectile)
                {
                    var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(context.Session.Camera);
                    MouseCursor.FlipCustomImage = mousePos.X < context.Session.Player.X;
                }
            }
            else
            {
                MouseCursor.HightlightColor = null;
            }
        }
    }
}
