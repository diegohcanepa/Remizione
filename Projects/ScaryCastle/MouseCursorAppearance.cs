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

        private static readonly string castHereText = Localization.GetValue(Verb.Cast);
        private static readonly string placeHereText = Localization.GetValue(Verb.Place);
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
            if (MouseCursor.State == MouseCursorState.Hit)
            {
                MouseCursor.TextColor = ColorPalette.Text.OrangeLight;
                MouseCursor.Text = context.Target?.LocalizedDisplayName;
                return;
            }

            if (context.HeldItem != null && context.HeldItem.Definition.UsageMode != ItemUsageMode.Default)
            {
                if (context.HeldItem?.Definition.UsageMode == ItemUsageMode.Cast)
                {
                    MouseCursor.TextColor = ColorPalette.Text.Purple;
                    MouseCursor.Text = castHereText;
                }
                else if (context.HeldItem?.Definition.UsageMode == ItemUsageMode.Place)
                {
                    MouseCursor.TextColor = ColorPalette.Text.Yellow;
                    MouseCursor.Text = placeHereText;
                }

                return;
            }

            if (MouseCursor.State is MouseCursorState.Up or MouseCursorState.Down or MouseCursorState.Left or MouseCursorState.Right)
                return;

            if (context.Target is { } target)
            {
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

            MouseCursor.Hightlight = context.Target != null;
        }
    }
}
