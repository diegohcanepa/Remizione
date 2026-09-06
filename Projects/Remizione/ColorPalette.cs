using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ColorPalette
    /// </summary>
    internal static class ColorPalette
    {
        // BackgroundColor
        internal static Color BackgroundColor { get; } = new(0, 2, 5);

        // ContextMenu
        internal static class ContextMenu
        {
            internal static Color OptionText { get; } = new(189, 106, 98);
            internal static Color OptionBack { get; } = Color.Black;
            internal static Color OptionHighlight { get; } = new Color(223, 224, 232) * .7f;
            internal static Color SceneShade { get; } = Color.Black * .6f;
        }

        // CreditHeading
        internal static Color CreditHeading { get; } = new(230, 190, 90);

        // CreditLine
        internal static Color CreditLine { get; } = new(230, 230, 212);

        // DestinationMark
        internal static Color DestinationMark { get; } = new(143, 77, 87);

        // HighlightedText
        internal static Color HighlightedText { get; } = new(215, 215, 170);

        // HPMeter
        internal static class HPMeter
        {
            internal static Color Back { get; } = Text.TerraDarker;
            internal static Color Diff { get; } = Text.TerraDark;
            internal static Color Fore { get; } = Text.Terra;
        }

        // MenuItemTextActive
        internal static Color MenuItemTextActive { get; } = new(240, 240, 240);

        // MenuItemTextInactive
        internal static Color MenuItemTextInactive { get; } = new(187, 180, 207);

        // MenuOptionLabel
        internal static Color MenuOptionLabel { get; } = new(130, 130, 160);

        // MessageBoxRedText
        internal static Color MessageBoxRedText { get; } = new(224, 144, 144);

        // MouseCursor
        internal static class MouseCursor
        {
            // Highlight
            internal static Vector4 Highlight { get; } = (Color.WhiteSmoke * .6f).ToVector4();

            // SubText
            internal static Color SubText { get; } = Text.TerraLight;

            // Tooltip
            internal static Color Tooltip { get; } = new(247, 232, 213);
        }

        // OutdoorLight
        internal static Color OutdoorLight { get; } = new(75, 95, 220);

        // PopupTitle
        internal static Color PopupTitle { get; } = new(116, 95, 75);

        // SceneShade
        internal static Color SceneShade { get; } = Color.Black * .6f;

        // Shadow
        internal static Color Shadow { get; } = Color.Black * .3f;

        // ShadowOpacity
        public const float ShadowOpacity = .4f;

        // ShadowSpot
        internal static Color ShadowSpot { get; } = Color.Black * .7f;

        // SpeechText
        internal static class SpeechText
        {
            internal static Color Fill { get; } = new(196, 195, 191);
            internal static Color Shadow { get; } = Color.Black * .2f;
            internal static Color Text { get; } = MouseCursor.Tooltip;
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
            internal static Color Gold { get; } = new(240, 181, 65);
            internal static Color Green { get; } = new(59, 125, 79);
            internal static Color GreenLight { get; } = new(99, 171, 63);
            internal static Color Highlight { get; } = new(172, 167, 144);
            internal static Color Hover { get; } = new(167, 143, 145);
            internal static Color Light { get; } = new(200, 165, 138);
            internal static Color MouseCursor { get; } = new(247, 232, 213);
            internal static Color Orange { get; } = new(171, 81, 48);
            internal static Color OrangeLight { get; } = new(207, 117, 43);
            internal static Color Purple { get; } = new(156, 42, 112);
            internal static Color Red { get; } = new(173, 47, 69);
            internal static Color RedLight { get; } = new(230, 69, 57);
            internal static Color Sentence { get; } = new(172, 167, 144);
            internal static Color SteelBlue { get; } = new(76, 104, 133);
            internal static Color TerraDarker { get; } = new(61, 41, 54);
            internal static Color TerraDarkest { get; } = new(41, 29, 43);
            internal static Color TerraDark { get; } = new(82, 51, 63);
            internal static Color Terra { get; } = new(143, 77, 87);
            internal static Color TerraLight { get; } = new(189, 106, 98);
            internal static Color TerraLighter { get; } = new(255, 174, 112);
            internal static Color Yellow { get; } = new(255, 238, 131);
        }

        // TextWhite
        internal static Color TextWhite { get; } = new(223, 224, 232);

        // TextStandardMenuTitle
        internal static Color TextStandardMenuTitle { get; } = new(227, 213, 200);
    }
}
