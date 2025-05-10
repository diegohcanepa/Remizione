using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Utils
    /// </summary>
    internal static class Utils
    {
        // CreateVersionLabel
        internal static TextSprite CreateVersionLabel(EngendroGame game)
        {
            TextSprite result = new(game, Fonts.Common)
            {
                Color = Color.DarkGray,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -5, 5),
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.TextVersionInfo,
                Text = GetVersion()
            };

            return result;
        }

        // CreateLightColorTween
        internal static ColorTween? CreateLightColorTween(LightKind lightKind, Color color)
        {
            return lightKind switch
            {
                LightKind.Fire => ColorTween.Create(TweenStyle.Linear, color, color * .94f, 90, -1),
                LightKind.Fireplace => ColorTween.Create(TweenStyle.Linear, color, color * .96f, 90, -1),
                LightKind.Lantern => ColorTween.Create(TweenStyle.Linear, color * .98f, color * .96f, 90, -1),
                _ => null,
            };
        }

        // CreateLightScaleTween
        internal static Vector2Tween? CreateLightScaleTween(LightKind lightKind, Vector2 scale)
        {
            return lightKind switch
            {
                LightKind.Fire => Vector2Tween.Create(TweenStyle.Linear, scale, scale * 1.05f, 1200, -1),

                LightKind.Fireplace => Vector2Tween.Create(TweenStyle.Linear, scale, scale * 1.01f, 1200, -1),

                LightKind.Lantern => Vector2Tween.Create(TweenStyle.Linear, scale, scale * 1.1f, Randomizer.Next(1100, 1400), -1),

                _ => null,
            };
        }

        // GetVersion
        internal static string GetVersion() => $"Build {GameSettings.Build} " + (EngendroGame.DebugMode ? "(dev)" : "(rel)");

        // LayoutControlsHorizontally
        public static void LayoutControlsHorizontally(UIControl[] controlList, float spacing)
        {
            float width = 0;

            for (var i = 0; i < controlList.Length; i++)
            {
                //controlList[i].PivotOrigin = RectanglePoint.LeftBottom;
                width += controlList[i].BoundingBox.Width;

                if (i < controlList.Length - 1)
                {
                    width += spacing;
                }
            }

            var x = (Screen.NativeWidth - width) / 2;
            for (var i = 0; i < controlList.Length; i++)
            {
                controlList[i].PivotOrigin = RectanglePoint.LeftBottom;
                controlList[i].X = x;
                controlList[i].Y = Screen.SafeArea.Bottom;
                x += controlList[i].BoundingBox.Width + spacing;
            }
        }

        // LayoutControlsVertically
        public static void LayoutControlsVertically(UIControl[] controlList, float spacing)
        {
            var pos = Screen.SafeArea.GetPoint(RectanglePoint.RightBottom);

            for (var i = 0; i < controlList.Length; i++)
            {
                controlList[i].PivotOrigin = RectanglePoint.RightBottom;
                controlList[i].Position = pos;
                pos.Y -= controlList[i].BoundingBox.Height + spacing;
            }
        }
    }
}
