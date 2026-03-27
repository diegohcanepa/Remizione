using Engendro;
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
            // Inventory
            if (context.Session.Game.SceneManager.CurrentScene is InventoryScene)
            {
                MouseCursor.State = MouseCursorState.Hand;
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

            // Cursor override
            if (context.Target?.GetMouseCursorState() is { } customState)
            {
                MouseCursor.State = customState;
                return;
            }

            // Item grabbed
            if (context.HeldItem != null)
                MouseCursor.CustomImage = context.HeldItem.Definition.Image;
            else
                MouseCursor.State = MouseCursorState.Cross;
        }

        // RefreshText
        private static void RefreshText(InteractionContext context)
        {
            if (MouseCursor.State is MouseCursorState.Up or MouseCursorState.Down or MouseCursorState.Left or MouseCursorState.Right)
                return;

            if (context.Target is { } target)
            {
                // No item 
                if (context.HeldItem == null)
                {
                    MouseCursor.Text = target.DisplayName;
                    return;
                }

                if (target == context.Session.Player)
                    MouseCursor.Text = $"{useVerb} {context.HeldItem.Definition.DisplayName}";
                else
                    MouseCursor.Text = $"{useVerb} {context.HeldItem.Definition.DisplayName} {withPreposition} {target.DisplayName}";
            }
        }

        #endregion

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            MouseCursor.Reset();

            RefreshCursor(context);

            RefreshText(context);

            if (context.HeldItem?.Definition.FaithCost > 0)
                MouseCursor.HightlightColor = ColorPalette.MouseCursorHighlightBlue;

            else if (context.Target != null)
                MouseCursor.HightlightColor = ColorPalette.MouseCursorHighlightWhite;

            else
                MouseCursor.HightlightColor = null;
        }
    }
}
