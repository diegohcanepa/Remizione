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
                MouseCursor.State = context.Session.AwaitingScript?.CurrentStatement is AwaitInputCommand ? MouseCursorState.Hand : MouseCursorState.Wait;
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
                if (target == context.Session.Player && context.HeldItem?.Definition.Verb != ItemVerb.None)
                {
                    MouseCursor.Text = context.HeldItem?.Definition.LocalizedVerbSentence;
                    return;
                }

                // Get sentence
                var sentence = target.LocalizedDisplayName;

                // No item 
                if (context.HeldItem == null)
                {
                    MouseCursor.Text = sentence;
                    return;
                }

                MouseCursor.Text = $"{useVerb} {context.HeldItem.Definition.LocalizedDisplayName} {withPreposition} {sentence}";
            }
        }

        #endregion

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            MouseCursor.Reset();

            RefreshCursor(context);

            RefreshText(context);

            if (context.HeldItem?.Definition.IsMagical == true)
                MouseCursor.HightlightColor = ColorPalette.MouseCursorHighlightBlue;
            
            else if (context.Target != null)
                MouseCursor.HightlightColor = ColorPalette.MouseCursorHighlightWhite;

            else
                MouseCursor.HightlightColor = null;
        }
    }
}
