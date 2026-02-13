using Adberration.Scripting;
using Engendro;
using ScaryCastle.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursorAppearance
    /// </summary>
    internal static class MouseCursorAppearance
    {
        private static readonly string useVerb = Localization.GetValue(Verb.Use);
        private static readonly string withPreposition = TextRepository.GetValue("Misc.WithPreposition");

        // RefreshCursor
        private static void RefreshCursor(InteractionContext context)
        {
            // Modal speech bubble active
            if (SpeechBubble.ModalInstance != null)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                return;
            }

            // Player is recharging will
            if (context.Session.Player?.IsTired == true)
            {
                MouseCursor.State = MouseCursorState.Wait;
                return;
            }

            // Session is awaiting script
            if (context.Session.IsAwaiting)
            {
                MouseCursor.State = context.Session.AwaitingScript?.CurrentStatement is AwaitInputCommand ? MouseCursorState.Hand : MouseCursorState.Wait;
                return;
            }

            // Inventory is visible
            if (context.Session.HUD.Inventory.IsVisible)
            {
                MouseCursor.State = MouseCursorState.Hand;
                return;
            }

            if (context.CursorOverride.HasValue)
            {
                MouseCursor.State = context.CursorOverride.Value;
                return;
            }

            // Item grabbed
            if (context.HeldItem != null)
            {
                MouseCursor.CustomImage = context.HeldItem.Definition.Image;
                return;
            }

            MouseCursor.State = context.Target?.GetMouseCursorState() ?? MouseCursorState.Cross;
        }

        // RefreshText
        private static void RefreshText(InteractionContext context)
        {
            if (context.Target is not { } target || context.CursorOverride.HasValue)
                return;

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

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            MouseCursor.Reset();

            RefreshCursor(context);

            RefreshText(context);

            if (context.Target == null || context.CursorOverride.HasValue)
                MouseCursor.HightlightState = MouseCursorHightlightState.None;
            else
                MouseCursor.HightlightState = context.IsValidInteraction ? MouseCursorHightlightState.Green : MouseCursorHightlightState.Red;
        }
    }
}
