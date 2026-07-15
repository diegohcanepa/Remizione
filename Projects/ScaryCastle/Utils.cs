using Adberration;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// Utils
    /// </summary>
    internal static class Utils
    {
        // ApplySoundEmitter
        internal static void ApplySoundEmitter(Entity emitter, SoundInstance instance, float effectiveVolume)
        {
            const int margin = 80; // margen en píxeles fuera del VisibleBox donde el volumen cae linealmente a 0

            RectangleF visible = emitter.Session.Camera.VisibleBox;
            float spriteX = emitter.Position.X;

            // límites extendidos (visible box + margen a ambos lados)
            float leftLimit = visible.Left - margin;
            float rightLimit = visible.Right + margin;

            // Denominador para normalizar pan respecto al centro (ancho/2 + margin).
            // Protegemos contra ancho 0.
            float halfRange = (visible.Width * 0.5f) + margin;
            if (halfRange <= 0.0001f) halfRange = 1f; // fallback seguro

            // Pan: -1 en leftLimit, 0 en el centro de la cámara, +1 en rightLimit
            float centerX = visible.Center.X;
            float pan = MathHelper.Clamp((spriteX - centerX) / halfRange, -1f, 1f);

            // Volumen:
            // - Si está dentro del VisibleBox => 1
            // - Si está fuera del leftLimit/rightLimit => 0
            // - Si está entre VisibleBox y límite extendido => interpolación lineal 1 -> 0
            float volume;
            if (visible.Contains(emitter.Position))
            {
                volume = 1f;
            }
            else if (spriteX <= leftLimit || spriteX >= rightLimit)
            {
                // completamente fuera del rango extendido
                volume = 0f;
            }
            else
            {
                // Está fuera del VisibleBox pero dentro del margen extendido.
                if (spriteX < visible.Left)
                {
                    // se encuentra a la izquierda del VisibleBox
                    float t = (visible.Left - spriteX) / margin; // 0..1
                    volume = MathHelper.Clamp(1f - t, 0f, 1f);
                }
                else // spriteX > visible.Right
                {
                    float t = (spriteX - visible.Right) / margin; // 0..1
                    volume = MathHelper.Clamp(1f - t, 0f, 1f);
                }
            }

            // Aplicar valores al SoundInstance
            instance.Pan = pan;
            instance.Volume.Current = volume * effectiveVolume;
        }

        // CreateVersionLabel
        internal static TextSprite CreateVersionLabel()
        {
            TextSprite result = new(Fonts.Common)
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

        // CreateLightOpacityTween
        internal static FloatTween? CreateLightOpacityTween(LightKind lightKind)
        {
            return lightKind switch
            {
                LightKind.Fire or
                LightKind.Fireplace or
                LightKind.Lantern => FloatTween.Create(TweenStyle.Linear, 1, .98f, 80, -1),
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
