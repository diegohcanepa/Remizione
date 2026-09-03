using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Screen
    /// </summary>
    public static class Screen
    {
        // Area
        public static Rectangle Area { get; } = new Rectangle(0, 0, NativeWidth, NativeHeight);

        // Center
        public static Vector2 Center { get; } = new Vector2(NativeWidth / 2, NativeHeight / 2);

        // HUDArea
        public static Rectangle HUDArea { get; } = new(4, 4, NativeWidth - 8, NativeHeight - 8);

        // NativeHeight
        public const int NativeHeight = 135;

        // NativeWidth
        public const int NativeWidth = 240;
    }
}
