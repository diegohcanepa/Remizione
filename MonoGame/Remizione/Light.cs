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

        private readonly Sprite lightSprite;
        private int litTweenDuration;
        private int unlitTweenDuration;
        private readonly FloatTween fadeTween = new();
        private float currentFade = 1;

        #endregion

        #region Constructor

        // Constructor
        public Light(string name, LightKind lightKind = LightKind.Default)
        {
            Name = name;
            lightSprite = new()
            {
                RenderImage = Atlases.Environment.DefaultLight,
                PivotOrigin = RectanglePoint.Center
            };
            this.LightKind = lightKind;
            this.Color = GetInitialColor(lightKind);
            
            Setup();
        }

        #endregion

        #region Static members

        // GetInitialColor
        private static Color GetInitialColor(LightKind lightKind)
        {
            return lightKind switch
            {
                LightKind.GroundTorch => new Color(235, 170, 170),
                LightKind.Fire => new Color(255, 160, 160),
                LightKind.KeyItemOrb => Colors.Palette.SkyBlue,
                LightKind.CommonItemOrb => Colors.Palette.TerraLight,
                LightKind.OrganicItemOrb => Colors.Palette.Green,
                LightKind.SulfurBonfire => new(184, 196, 81),
                LightKind.Player => new Color(235, 170, 170) * .7f,
                _ => Color.White
            };
        }

        #endregion

        #region Private members

        // LitCore
        private void LitCore(int duration = -1)
        {
            int targetDuration = duration < 0 ? litTweenDuration : duration;

            if (targetDuration == 0)
            {
                fadeTween.Stop();
                currentFade = 1;
                return;
            }

            if (currentFade == 1 || (fadeTween.IsRunning && fadeTween.EndValue == 1))
                return;

            fadeTween.Start(TweenStyle.CubicIn, currentFade, 1f, targetDuration);
        }

        // Setup
        private void Setup()
        {
            fadeTween.Stop();
            litTweenDuration = 0;
            unlitTweenDuration = 0;

            lightSprite.Tweens.Reset();
            lightSprite.Color = Color;
            lightSprite.Scale = Scale;

            switch (LightKind)
            {
                case LightKind.GroundTorch:
                    litTweenDuration = 2000;
                    unlitTweenDuration = 300;
                    break;

                case LightKind.Fire:
                    litTweenDuration = 1000;
                    unlitTweenDuration = 300;
                    break;

                case LightKind.Lantern:
                    litTweenDuration = 500;
                    unlitTweenDuration = 500;
                    break;

                case LightKind.Player:
                    break;

                case LightKind.SulfurBonfire:
                    litTweenDuration = 1500;
                    unlitTweenDuration = 2500;
                    break;

                case LightKind.KeyItemOrb:
                case LightKind.CommonItemOrb:
                case LightKind.OrganicItemOrb:
                    litTweenDuration = 1000;
                    unlitTweenDuration = 1000;
                    break;

                case LightKind.MuzzleFlash:
                default:
                    break;
            }
        }

        // UnlitCore
        private void UnlitCore(int duration = -1)
        {
            int targetDuration = duration < 0 ? unlitTweenDuration : duration;

            if (targetDuration == 0)
            {
                fadeTween.Stop();
                currentFade = 0;
                return;
            }

            if (currentFade == 0 || (fadeTween.IsRunning && fadeTween.EndValue == 0))
                return;

            fadeTween.Start(TweenStyle.CubicIn, currentFade, 0, targetDuration);
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

            lightSprite.Draw(gameTime);

            // Restauramos la opacidad original para no romper la evolución del tween de flicker del sprite
            lightSprite.Opacity = originalSpriteOpacity;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
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

        // BlendMode
        public LightBlendMode BlendMode { get; set; }

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
                }
            }
        }

        // IsEmitting
        public bool IsEmitting => currentFade > 0;

        // LightKind
        public LightKind LightKind
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Color = GetInitialColor(field);
                    Setup();
                }
            }
        }

        // Lit
        public void Lit(bool immediate)
        {
            LitCore(immediate ? 0 : litTweenDuration);
        }

        // Lit
        public void Lit(int duration)
        {
            LitCore(Math.Max(0, duration));
        }

        // Name
        public string Name { get; }

        // Opacity
        public float Opacity
        {
            get => lightSprite.Opacity;
            set => lightSprite.Opacity = value;
        }

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
        public void Unlit(bool immediate)
        {
            UnlitCore(immediate ? 0 : unlitTweenDuration);
        }

        // Unlit
        public void Unlit(int duration)
        {
            UnlitCore(Math.Max(0, duration));
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