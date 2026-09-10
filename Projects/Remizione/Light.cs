using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Light
    /// </summary>
    public class Light : GameObject, INamedObject
    {
        #region Private fields

        private float durationMs;
        private readonly Sprite lightSprite;
        private int litTweenDuration;
        private int unlitTweenDuration;
        private readonly FloatTween fadeTween = new();
        private float currentFade = 1;

        #endregion

        #region Constructor

        // Constructor
        public Light(string name, LightKind lightKind)
        {
            Name = name;
            LightKind = lightKind;

            lightSprite = new()
            {
                RenderImage = Atlases.Environment.DefaultLight,
                PivotOrigin = RectanglePoint.Center
            };

            Color = GetInitialColor(lightKind);

            Setup();
        }

        #endregion

        #region Static members

        // CreateColorTween
        private static ColorTween? CreateColorTween(LightKind lightKind, Color color) => lightKind switch
        {
            LightKind.Fire => ColorTween.Create(TweenStyle.Linear, color, color * .9f, 90, -1),
            LightKind.SulfurBonfire => ColorTween.Create(TweenStyle.Linear, color, color * .96f, 90, -1),
            LightKind.Lantern => ColorTween.Create(TweenStyle.Linear, color * .98f, color * .96f, 90, -1),
            _ => null,
        };

        // CreateFlickerTween
        private static FloatTween? CreateFlickerTween(LightKind lightKind) => lightKind switch
        {
            LightKind.Fire or
            LightKind.SulfurBonfire or
            LightKind.Lantern => FloatTween.Create(TweenStyle.Linear, 1f, .98f, 80, -1),
            LightKind.LootOrb => FloatTween.Create(TweenStyle.Linear, .5f, .55f, 70, -1),
            _ => null,
        };

        // CreateScaleTween
        private static Vector2Tween? CreateScaleTween(LightKind lightKind, Vector2 scale) => lightKind switch
        {
            LightKind.Fire => Vector2Tween.Create(TweenStyle.Linear, scale, scale * 1.05f, 1200, -1),
            LightKind.SulfurBonfire => Vector2Tween.Create(TweenStyle.Linear, scale, scale * 1.01f, 1200, -1),
            LightKind.Lantern => Vector2Tween.Create(TweenStyle.Linear, scale, scale * 1.1f, Random.Shared.Next(1100, 1400), -1),
            _ => null,
        };

        // GetInitialColor
        private static Color GetInitialColor(LightKind lightKind) => lightKind switch
        {
            LightKind.SulfurBonfire => new Color(134, 146, 31),
            LightKind.Outdoor => ColorPalette.OutdoorLight,
            _ => Color.White
        };

        #endregion

        #region Private members

        // Setup
        private void Setup()
        {
            litTweenDuration = 0;
            unlitTweenDuration = 0;
            Passes = 1;

            lightSprite.Tweens.Reset();
            lightSprite.Color = Color;
            lightSprite.Scale = Scale;

            switch (LightKind)
            {
                case LightKind.Fire:
                    Passes = 2;
                    lightSprite.Tweens.OpacityTween = CreateFlickerTween(LightKind);
                    litTweenDuration = 2000;
                    unlitTweenDuration = 2000;
                    break;

                case LightKind.SulfurBonfire:
                    Passes = 2;
                    lightSprite.Tweens.ColorTween = CreateColorTween(LightKind, Color);
                    lightSprite.Tweens.ScaleTween = CreateScaleTween(LightKind, Scale);
                    lightSprite.Tweens.OpacityTween = CreateFlickerTween(LightKind);
                    litTweenDuration = 1500;
                    unlitTweenDuration = 1500;
                    break;

                case LightKind.LootOrb:
                    lightSprite.Tweens.OpacityTween = CreateFlickerTween(LightKind);
                    litTweenDuration = 1000;
                    unlitTweenDuration = 1000;
                    break;

                case LightKind.MuzzleFlash:
                case LightKind.Outdoor:
                case LightKind.Player:
                default:
                    break;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsEmitting)
                return;

            // Guardamos la opacidad base del sprite (que puede estar haciendo flicker en loop)
            float originalSpriteOpacity = lightSprite.Opacity;

            // Aplicamos la opacidad combinada (Flicker * Fade de encendido)
            lightSprite.Opacity = originalSpriteOpacity * currentFade;

            for (int i = 0; i < Passes; i++)
            {
                lightSprite.Draw(gameTime);
            }

            // Restauramos la opacidad original para no romper la evolución del tween de flicker del sprite
            lightSprite.Opacity = originalSpriteOpacity;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (durationMs > 0)
            {
                durationMs -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (durationMs <= 0)
                    Unlit();
            }

            // El sprite actualiza su escala, color y su flicker loopeado alegremente
            lightSprite.Update(gameTime);

            // La luz actualiza su nivel de encendido progresivo
            if (fadeTween.IsRunning)
            {
                fadeTween.Update(gameTime);
                currentFade = fadeTween.CurrentValue;
            }
        }

        #endregion

        // Ambient
        public bool Ambient { get; set; }

        // BoundingBox
        public RectangleF BoundingBox => lightSprite.BoundingBox;

        // Color
        public Color Color
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    lightSprite.Color = value;

                    if (lightSprite.Tweens.ColorTween != null)
                        lightSprite.Tweens.ColorTween = CreateColorTween(LightKind, value);
                }
            }
        }

        // IsEmitting
        public bool IsEmitting => currentFade > 0;

        // LightKind
        public LightKind LightKind { get; }

        // Lit
        public void Lit(bool immediate = false, int duration = 0)
        {
            durationMs = duration <= 0 ? -1 : duration;

            if (immediate)
            {
                fadeTween.Stop();
                currentFade = 1f;
                return;
            }

            if (currentFade == 1f || (fadeTween.IsRunning && fadeTween.EndValue == 1f))
                return;

            if (litTweenDuration == 0)
                currentFade = 1f;
            else
                fadeTween.Start(TweenStyle.CubicIn, currentFade, 1f, litTweenDuration);
        }

        // Name
        public string Name { get; }

        // Passes
        public int Passes { get; set; } = 1;

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => lightSprite.PivotOrigin;
            set => lightSprite.PivotOrigin = value;
        }

        // Position
        public Vector2 Position
        {
            get => lightSprite.Position;
            set => lightSprite.Position = value;
        }

        // Scale
        public Vector2 Scale
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    lightSprite.Scale = value;

                    if (lightSprite.Tweens.ScaleTween != null)
                        lightSprite.Tweens.ScaleTween = CreateScaleTween(LightKind, value);
                }
            }
        } = Vector2.One;

        // ScaleTo
        public void ScaleTo(TweenStyle style, Vector2 value, int duration)
        {
            lightSprite.Tweens.ScaleTween = Vector2Tween.Create(style, lightSprite.Scale, value, duration);
        }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Unlit
        public void Unlit(bool immediate = false)
        {
            if (immediate)
            {
                fadeTween.Stop();
                currentFade = 0f;
                return;
            }

            if (currentFade == 0f || (fadeTween.IsRunning && fadeTween.EndValue == 0f))
                return;

            if (unlitTweenDuration == 0)
                currentFade = 0f;
            else
                fadeTween.Start(TweenStyle.CubicIn, currentFade, 0f, unlitTweenDuration);
        }

        // X
        public float X
        {
            get => lightSprite.X;
            set => lightSprite.X = value;
        }

        // Y
        public float Y
        {
            get => lightSprite.Y;
            set => lightSprite.Y = value;
        }
    }
}