using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Colors
    /// </summary>
    internal static class Colors
    {
        // BackgroundColor
        internal static Color BackgroundColor { get; } = new(0, 2, 5);

        // BloodColor
        internal static class BloodColor
        {
            internal static Color Guts { get; } = new(102, 24, 37);
            internal static Color ToxicGuts { get; } = new(70, 110, 70);
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

        // DestinationMark
        internal static Color DestinationMark { get; } = Palette.Green;

        // HPMeterActor
        internal static class HPMeterActor
        {
            internal static Color Back { get; } = Palette.TerraDarker;
            internal static Color Border { get; } = Palette.AlmostBlack;
            internal static Color Diff { get; } = Palette.Orange;
            internal static Color Fore { get; } = new(164, 60, 60);
        }

        // HPMeterProp
        internal static class HPMeterProp
        {
            internal static Color Back { get; } = Palette.TerraDarker;
            internal static Color Border { get; } = Palette.AlmostBlack;
            internal static Color Diff { get; } = Palette.TerraLight;
            internal static Color Fore { get; } = Palette.Terra;
        }

        // Inventory
        internal static class Inventory
        {
            internal static Color EffectDescription { get; } = MouseCursor.Tooltip;
            internal static Color KeyItem { get; } = Palette.SkyBlue * .7f;
            internal static Color Item { get; } = Palette.Terra;
        }

        // MenuItemTextActive
        internal static Color MenuItemTextActive { get; } = new(240, 240, 240);

        // MenuItemTextInactive
        internal static Color MenuItemTextInactive { get; } = new(187, 180, 207);

        // MenuOptionLabel
        internal static Color MenuOptionLabel { get; } = new(130, 130, 160);

        // MessageBoxRedText
        internal static Color MessageBoxRedText { get; } = new(224, 144, 144);

        // MiniMap
        internal static class MiniMap
        {
            // Bonfire
            internal static Color Bonfire { get; } = new(96, 96, 18);

            // Default
            internal static Color Default { get; } = Palette.Terra;

            // KeyItem
            internal static Color KeyItem { get; } = Palette.SkyBlue;

            // Player
            internal static Color Player { get; } = new(172, 167, 144);

            // Torch
            internal static Color Torch { get; } = Palette.Red;
        }

        // MouseCursor
        internal static class MouseCursor
        {
            // Highlight
            internal static Vector4 Highlight { get; } = (Color.WhiteSmoke * .3f).ToVector4();

            // SubText
            internal static Color SubText { get; } = Palette.YellowDark;

            // Tooltip
            internal static Color Tooltip { get; } = Palette.Text;
        }

        // Palette
        internal static class Palette
        {
            internal static Color AlmostBlack { get; } = new(25, 25, 25);
            internal static Color Grace { get; } = new Color(172, 167, 144) * .7f;
            internal static Color Green { get; } = new(59, 125, 79);
            internal static Color Orange { get; } = new(171, 81, 48);
            internal static Color Red { get; } = new(164, 60, 60);
            internal static Color RedBloody { get; } = new(155, 56, 50);
            internal static Color SkyBlue { get; } = new(79, 164, 184);
            internal static Color SkyBlueLight { get; } = new(76, 104, 133);
            internal static Color TerraLight { get; } = new(189, 106, 98);
            internal static Color Terra { get; } = new(143, 77, 87);
            internal static Color TerraDark { get; } = new(82, 51, 63);
            internal static Color TerraDarker { get; } = new(61, 41, 54);
            internal static Color TerraDarkest { get; } = new(41, 29, 43);
            internal static Color Text { get; } = new(172, 167, 144);
            internal static Color TextDark { get; } = new(124, 120, 104);
            internal static Color Yellow { get; } = new(255, 238, 131);
            internal static Color YellowDark { get; } = new(240, 181, 65);
        }

        // Shadow
        internal static Color Shadow { get; } = Color.Black * .3f;

        // ShadowOpacity
        public const float ShadowOpacity = .3f;

        // ShadowSpot
        internal static Color ShadowSpot { get; } = Color.Black * .7f;
    }
}
