using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ColorPalette
    /// </summary>
    internal static class ColorPalette
    {
        // BackgroundColor
        internal static Color BackgroundColor { get; } = new Color(0, 2, 5);

        // BackgroundShade
        internal static Color BackgroundShade { get; } = Color.Black * .2f;

        // ContextMenu
        internal static class ContextMenu
        {
            internal static Color Option { get; } = new(231, 230, 212);
            internal static Color OptionBack { get; } = Color.Black * .8f;
            internal static Color OptionHighlight { get; } = new(230, 190, 90);
            internal static Color Title { get; } = Color.White;
        }

        // Cycle
        internal static class Cycle
        {
            internal static Color Indulgence { get; } = new(76, 104, 133);
            internal static Color Penance { get; } = new(59, 32, 39);
        }

        // CreditHeading
        internal static Color CreditHeading { get; } = new Color(230, 190, 90);

        // CreditLine
        internal static Color CreditLine { get; } = new Color(230, 230, 212);

        // DestinationMark
        internal static class DestinationMark
        {
            internal static Color Default { get; } = new(59, 125, 79);
            internal static Color Target { get; } = new(200, 212, 93);
        }

        // FPMeter
        internal static class FPMeter
        {
            internal static Color Back { get; } = new(34, 63, 60);
            internal static Color Fore { get; } = new(47, 87, 83);
        }

        // HighlightedText
        internal static Color HighlightedText { get; } = new Color(215, 215, 170);

        // HPMeter
        internal static class HPMeter
        {
            internal static Color Back { get; } = new(43, 43, 69);
            internal static Color Fore { get; } = new(173, 47, 69);
        }

        // HUDMessage
        internal static Color HUDMessage { get; } = new Color(227, 213, 200);

        // MenuItemTextActive
        internal static Color MenuItemTextActive { get; } = new Color(240, 240, 240);

        // MenuItemTextInactive
        internal static Color MenuItemTextInactive { get; } = new Color(187, 180, 207);

        // MenuOptionLabel
        internal static Color MenuOptionLabel { get; } = new Color(130, 130, 160);

        // MessageBoxRedText
        internal static Color MessageBoxRedText { get; } = new Color(224, 144, 144);

        // OutdoorLight
        internal static Color OutdoorLight { get; } = new Color(75, 95, 220);

        // PopupTitle
        internal static Color PopupTitle { get; } = new Color(116, 95, 75);

        // ReactionMeter
        internal static class ReactionMeter
        {
            internal static Color Back { get; } = new(125, 56, 51);
            internal static Color Fore { get; } = new(171, 81, 48);
        }

        // ScenePausedShade
        internal static Color ScenePausedShade { get; } = Color.Black * .4f;

        // ShadowOpacity
        public const float ShadowOpacity = .4f;

        // ShadowSpot
        internal static Color ShadowSpot { get; } = Color.Black * .7f;

        // SpeechBubble
        internal static class SpeechBubble
        {
            internal static Color Fill { get; } = new(68, 62, 37);
            internal static Color Shadow { get; } = Color.Black * .2f;
            internal static Color Text { get; } = new(200, 200, 200);
            internal static Color Title { get; } = new(130, 130, 130);
        }

        // Text
        internal static class Text
        {
            internal static Color Dark { get; } = new(189, 106, 98);
            internal static Color DarkRed { get; } = new(82, 51, 63);
            internal static Color Green { get; } = new(59, 125, 79);
            internal static Color Highlight { get; } = new(240, 181, 65);
            internal static Color Light { get; } = new(223, 224, 232);
            internal static Color LightRed { get; } = new(143, 77, 87);
            internal static Color Title { get; } = new(59, 125, 79);
            internal static Color Yellow { get; } = new(255, 238, 131);
        }

        // TextWhite
        internal static Color TextWhite { get; } = new Color(223, 224, 232);

        // TextLastSave
        internal static Color TextLastSave { get; } = new Color(71, 115, 80);

        // TextStandardMenuTitle
        internal static Color TextStandardMenuTitle { get; } = new Color(227, 213, 200);

        // TextShadow
        internal static Color TextShadow { get; } = Color.Black * .5f;

        // UIControlShadow
        internal static Color UIControlShadow { get; } = Color.Black * .5f;

        // WillpowerMeter
        internal static class WillpowerMeter
        {
            internal static Color Back { get; } = new(59, 32, 39);
            internal static Color Fore { get; } = new(105, 36, 100);
        }
    }
}
