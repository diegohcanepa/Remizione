using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ScaleInfo
    /// </summary>
    internal static class ScaleInfo
    {
        // ContextMenu
        internal static class ContextMenu
        {
            internal static Vector2 Icon { get; } = new(.5f);
            internal static Vector2 Option { get; } = new(.1f);
            internal static Vector2 Title { get; } = new(.11f);
        }

        // ControlLabel
        internal static Vector2 ControlLabel { get; } = new Vector2(.10f);

        // CreditHeading
        internal static Vector2 CreditHeading { get; } = new Vector2(.11f);

        // CreditLine
        internal static Vector2 CreditLine { get; } = new Vector2(.09f);

        // CreditTitle
        internal static Vector2 CreditTitle { get; } = new Vector2(.16f);

        // InteractionMenu
        internal static class InteractionMenu
        {
            internal static Vector2 Option { get; } = new(.09f);
            internal static Vector2 Title { get; } = new(.1f);
        }

        // MenuItemTextActive
        internal static Vector2 MenuItemTextActive { get; } = new Vector2(.14f);

        // MenuItemTextInactive
        internal static Vector2 MenuItemTextInactive { get; } = new Vector2(.12f);

        // MenuOption
        internal static Vector2 MenuOption { get; } = new Vector2(.11f);

        // MessageBoxMessage
        internal static Vector2 MessageBoxMessage { get; } = new Vector2(.14f);

        // MessageBoxSubMessage
        internal static Vector2 MessageBoxSubMessage { get; } = new Vector2(.11f);

        // MessageBoxTitle
        internal static Vector2 MessageBoxTitle { get; } = new Vector2(.19f);

        // PopupText
        internal static Vector2 PopupText { get; } = new Vector2(.1f);

        // PopupTitle
        internal static Vector2 PopupTitle { get; } = new Vector2(.16f);

        // SpeechBubble
        internal static class SpeechBubble
        {
            internal static Vector2 Text { get; } = new(.08f);
        }

        // TextControllerDisconnectedMessage
        internal static Vector2 TextControllerDisconnectedMessage { get; } = new Vector2(.12f);

        // TextControllerDisconnectedTitle
        internal static Vector2 TextControllerDisconnectedTitle { get; } = new Vector2(.24f);

        // Text
        internal static class Text
        {
            internal static Vector2 Tiny { get; } = new(.05f);
            internal static Vector2 Small { get; } = new(.06f);
            internal static Vector2 Medium { get; } = new(.07f);
            internal static Vector2 Large { get; } = new(.08f);
            internal static Vector2 VeryLarge { get; } = new(.09f);
            internal static Vector2 ExtraLarge { get; } = new(.1f);
        }

        // TextMenuContainerTitle
        internal static Vector2 TextMenuContainerTitle { get; } = new Vector2(.2f);

        // TextQuickSlot
        internal static Vector2 TextQuickSlot { get; } = new Vector2(.055f);

        // TextVersionInfo
        internal static Vector2 TextVersionInfo { get; } = new Vector2(.08f);

        // UIIcon
        internal static class UIIcon
        {
            internal static Vector2 Small { get; } = new(.5f);
            internal static Vector2 Medium { get; } = new(.75f);
            internal static Vector2 Large { get; } = Vector2.One;
        }
    }
}
