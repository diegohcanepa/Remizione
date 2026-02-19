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
            {
                /*
                if (context.HeldItem.Definition.RequiresDrop)
                {
                    if (context.WalkArea == null)
                    {
                        MouseCursor.State = MouseCursorState.Cast;
                    }
                    else
                    {
                        var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(context.Session.Camera);
                        MouseCursor.State = context.WalkArea.Contains(destination) ? MouseCursorState.Cast : MouseCursorState.Prohibition;
                    }
                }
                else
                */
                {
                    MouseCursor.CustomImage = context.HeldItem.Definition.Image;
                }

                return;
            }

            MouseCursor.State = context.Target?.GetMouseCursorState() ?? MouseCursorState.Cross;
        }

        // RefreshText
        private static void RefreshText(InteractionContext context)
        {
            if (MouseCursor.State == MouseCursorState.Hit)
            {
                MouseCursor.TextColor = ColorPalette.Text.OrangeLight;
                MouseCursor.Text = Localization.GetValue(Verb.Headbutt);
                return;
            }

            if (context.HeldItem != null && context.HeldItem.Definition.UsageMode != ItemUsageMode.Default)
            {
                if (context.HeldItem?.Definition.UsageMode == ItemUsageMode.Place)
                    MouseCursor.Text = castHereText;

                else if (context.HeldItem?.Definition.UsageMode == ItemUsageMode.Place)
                    MouseCursor.Text = placeHereText;

                if (!string.IsNullOrWhiteSpace(MouseCursor.Text))
                {
                    MouseCursor.TextColor = ColorPalette.Text.Yellow;
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
