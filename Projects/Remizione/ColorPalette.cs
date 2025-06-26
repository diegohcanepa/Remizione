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
            internal static Color OptionText { get; } = new(189, 106, 98);
            internal static Color OptionBack { get; } = Color.Black;
            internal static Color OptionHighlight { get; } = new Color(223, 224, 232) * .7f;
            internal static Color SceneShade { get; } = Color.Black * .4f;
        }

        // Cycle
        internal static class Cycle
        {
            internal static Color Indulgence { get; } = new(104, 111, 153);
            internal static Color Penance { get; } = new(157, 81, 81);
            internal static Color PenanceActive { get; } = new(180, 67, 67);
        }

        // CreditHeading
        internal static Color CreditHeading { get; } = new Color(230, 190, 90);

        // CreditLine
        internal static Color CreditLine { get; } = new Color(230, 230, 212);

        // DestinationMark
        internal static Color DestinationMark { get; } = new(143, 77, 87);

        // FaithMeter
        internal static class FaithMeter
        {
            internal static Color Back { get; } = new(20, 24, 46);
            internal static Color Fore { get; } = new(82, 51, 63);
        }

        // HighlightedText
        internal static Color HighlightedText { get; } = new Color(215, 215, 170);

        // HUDMessage
        internal static Color HUDMessage { get; } = new Color(227, 213, 200);

        // InteractiveTargetOutline
        internal static Vector4 InteractiveTargetOutline { get; } = (Color.LightCyan * .4f).ToVector4();

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

        // SceneShade
        internal static Color SceneShade { get; } = Color.Black * .5f;

        // ShadowOpacity
        public const float ShadowOpacity = .4f;

        // ShadowSpot
        internal static Color ShadowSpot { get; } = Color.Black * .7f;

        // SpeechBubble
        internal static class SpeechBubble
        {
            internal static Color Fill { get; } = new(200, 200, 210);
            internal static Color Shadow { get; } = Color.Black * .2f;
            internal static Color Text { get; } = new Color(27, 31, 33) * .7f;
        }

        // HPMeter
        internal static class HPMeter
        {
            internal static Color Back { get; } = new(82, 51, 63);
            internal static Color Fore { get; } = new(173, 47, 69);
        }

        // StatMeter
        internal static class StatMeter
        {
            internal static Color CurrentValue { get; } = Text.Terra;
            internal static Color MaximumValue { get; } = Text.TerraDark;
        }

        // Text
        internal static class Text
        {
            internal static Color Dark { get; } = new(163, 122, 123);
            internal static Color Default { get; } = new(163, 167, 194);
            internal static Color Disabled { get; } = new(80, 76, 76);
            internal static Color Fill { get; } = new(15, 42, 63);
            internal static Color Green { get; } = new(59, 125, 79);
            internal static Color Highlight { get; } = new(190, 170, 150);
            internal static Color Hover { get; } = new(167, 143, 145);
            internal static Color Light { get; } = new(200, 165, 138);
            internal static Color Red { get; } = new(230, 69, 57);
            internal static Color TerraDarker { get; } = new(61, 41, 54);
            internal static Color TerraDarkest { get; } = new(41, 29, 43);
            internal static Color TerraDark { get; } = new(82, 51, 63);
            internal static Color Terra { get; } = new(143, 77, 87);
            internal static Color TerraLight { get; } = new(189, 106, 98);
            internal static Color TerraLighter { get; } = new(255, 174, 112);
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

        // UIDerivedStatIcon
        internal static class UIDerivedStatIcon
        {
            internal static Color NegativeAmount { get; } = new(173, 47, 69);
            internal static Color PositiveAmount { get; } = new(59, 125, 79);
        }
    }
}
