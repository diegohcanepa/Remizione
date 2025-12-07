using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// Utils
    /// </summary>
    internal static class Utils
    {
        // AssertName
        internal static void AssertName(string name, object sender)
        {
            // Name cannot be a meta item
            if (sender is not MetaItem)
            {
                if (MetaItem.Find(name) != null)
                    throw new InvalidOperationException($"The name '{name}' is already taken by a MetaItem.");
            }

            // Name cannot be a realm 
            if (Enum.IsDefined(typeof(Realm), name))
                throw new InvalidOperationException($"The name '{name}' cannot be used because it is an item realm.");

            // Name cannot be a category
            if (Enum.IsDefined(typeof(ItemCategory), name))
                throw new InvalidOperationException($"The name '{name}' cannot be used because it is an item category.");
        }

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
                LightKind.Fire => ColorTween.Create(TweenStyle.Linear, color, color * .9f, 90, -1),
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

                LightKind.Lantern => Vector2Tween.Create(TweenStyle.Linear, scale, scale * 1.1f, Random.Shared.Next(1100, 1400), -1),

                _ => null,
            };
        }

        // GetVersion
        internal static string GetVersion()
        {
            return $"Build {GameSettings.Build} " + (EngendroGame.DebugMode ? "(dev)" : "(rel)");
        }

        // Intersects
        internal static bool Intersects(ReadOnlyCollection<string> listA, ReadOnlyCollection<string> listB)
        {
            if (listA.Count == 0 || listB.Count == 0)
                return false;

            for (int i = 0; i < listA.Count; i++)
            {
                var va = listA[i];

                for (int j = 0; j < listB.Count; j++)
                {
                    if (string.Equals(va, listB[j], StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        // LayoutControlsHorizontally
        internal static void LayoutControlsHorizontally(UIButton[] controlList, float spacing)
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
                controlList[i].Y = Screen.HUDArea.Bottom;
                x += controlList[i].BoundingBox.Width + spacing;
            }
        }

        // LayoutControlsVertically
        internal static void LayoutControlsVertically(UIButton[] controlList, float spacing)
        {
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom);

            for (var i = 0; i < controlList.Length; i++)
            {
                controlList[i].PivotOrigin = RectanglePoint.RightBottom;
                controlList[i].Position = pos;
                pos.Y -= controlList[i].BoundingBox.Height + spacing;
            }
        }

        // LoadJsonData
        internal static void LoadJsonData<T>(string fileName, Func<JsonElement, T> onCreate, string rootName = "data")
        {
            using var input = TitleContainer.OpenStream(fileName);
            using JsonDocument doc = JsonDocument.Parse(input);
            var root = doc.RootElement;

            if (!root.TryGetProperty(rootName, out JsonElement arrayElement) || arrayElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException();

            foreach (JsonElement element in arrayElement.EnumerateArray())
            {
                onCreate(element);
            }
        }
    }
}
