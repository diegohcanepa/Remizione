using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ColorPalette
    /// </summary>
    internal static class ColorPalette
    {
        // BackgroundColor
        internal static Color BackgroundColor { get; } = new(0, 2, 5);

        // BossMeter
        internal static class BossMeter
        {
            internal static Color Back { get; } = new(77, 34, 34);
            internal static Color Diff { get; } = Text.Orange;
            internal static Color Fore { get; } = new(168, 45, 45);
        }

        // Condition
        internal static class Condition
        {
            internal static Color Curse { get; } = new(119, 103, 211);
            internal static Color Poison { get; } = new(110, 224, 31);
        }

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

        // DefaultDarknessLight
        internal static Color DefaultDarknessLight { get; } = Color.WhiteSmoke * .75f;

        // GlobalLight
        internal static class GlobalLight
        {
            internal static Color Critical { get; } = new(255, 64, 64);
            internal static Color Default { get; } = Color.White;
        }

        // HighlightedText
        internal static Color HighlightedText { get; } = new(215, 215, 170);

        // HUDMessage
        internal static Color HUDMessage { get; } = new(227, 213, 200);

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
            // AttackableTarget
            internal static Color AttackableTarget { get; } = Text.OrangeLight;

            // HighlightGreen
            internal static Vector4 HighlightGreen { get; } = (new Color(79, 164, 79) * .7f).ToVector4();

            // HighlightRed
            internal static Vector4 HighlightRed { get; } = (new Color(164, 79, 79) * .7f).ToVector4();

            // HighlightWhite
            internal static Vector4 HighlightWhite { get; } = (Color.White * .75f).ToVector4();
        }

        // OutdoorLight
        internal static Color OutdoorLight { get; } = new(75, 95, 220);

        // PopupTitle
        internal static Color PopupTitle { get; } = new(116, 95, 75);

        // SceneShade
        internal static Color SceneShade { get; } = Color.Black * .5f;

        // ShadowOpacity
        public const float ShadowOpacity = .4f;

        // ShadowSpot
        internal static Color ShadowSpot { get; } = Color.Black * .7f;

        // SpeechBubble
        internal static class SpeechBubble
        {
            internal static Color Fill { get; } = new(190, 170, 150);
            internal static Color Shadow { get; } = Color.Black * .2f;
            internal static Color Text { get; } = new Color(27, 31, 33) * .6f;
            internal static Color Title { get; } = new Color(27, 31, 33) * .5f;
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
            internal static Color Orange { get; } = new(171, 81, 48);
            internal static Color OrangeLight { get; } = new(207, 117, 43);
            internal static Color Purple { get; } = new(156, 42, 112);
            internal static Color Red { get; } = new(173, 47, 69);
            internal static Color RedLight { get; } = new(230, 69, 57);
            internal static Color Sentence { get; } = new(190, 170, 150);
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

        // TextLastSave
        internal static Color TextLastSave { get; } = new(71, 115, 80);

        // TextStandardMenuTitle
        internal static Color TextStandardMenuTitle { get; } = new(227, 213, 200);
    }
}
