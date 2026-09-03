using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Light
    /// </summary>
    public class Light : GameObject, INamedObject
    {
        #region Private fields

        private int duration;
        private readonly Sprite lightSprite;
        private int litTweenDuration;
        private readonly FloatTween opacityTween = new();
        private int unlitTweenDuration;

        #endregion

        #region Constructor

        // Constructor
        public Light(string name)
        {
            this.Name = name;

            this.lightSprite = new Sprite()
            {
                RenderImage = Atlases.Environment.DefaultLight,
                PivotOrigin = RectanglePoint.Center
            };
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            litTweenDuration = 0;
            unlitTweenDuration = 0;
            lightSprite.Color = Color;
            lightSprite.Scale = Scale;

            switch (LightKind)
            {
                //  Fire
                case LightKind.Fire:
                    Passes = 2;
                    //lightSprite.Tweens.ColorTween = Utils.CreateLightColorTween(LightKind, Color);
                    //lightSprite.Tweens.ScaleTween = Utils.CreateLightScaleTween(LightKind, Scale);
                    lightSprite.Tweens.OpacityTween = Utils.CreateLightOpacityTween(LightKind);
                    litTweenDuration = 2000;
                    unlitTweenDuration = 2000;
                    break;

                //  Fireplace
                case LightKind.Fireplace:
                    Passes = 2;
                    lightSprite.Tweens.ColorTween = Utils.CreateLightColorTween(LightKind, Color);
                    lightSprite.Tweens.ScaleTween = Utils.CreateLightScaleTween(LightKind, Scale);
                    lightSprite.Tweens.OpacityTween = Utils.CreateLightOpacityTween(LightKind);
                    litTweenDuration = 1000;
                    unlitTweenDuration = 1000;
                    break;

                // MuzzleFlash
                case LightKind.MuzzleFlash:
                    lightSprite.Tweens.Reset();
                    lightSprite.Color = Color * .7f;
                    break;

                // Outdoor
                case LightKind.Outdoor:
                    lightSprite.Tweens.Reset();
                    lightSprite.Color = ColorPalette.OutdoorLight;
                    break;

                // Player
                case LightKind.Player:
                    lightSprite.Tweens.Reset();
                    lightSprite.Color = Color;
                    litTweenDuration = 0;
                    unlitTweenDuration = 0;
                    break;

                // Default
                default:
                    lightSprite.Tweens.Reset();
                    lightSprite.Color = Color;
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

            if (Passes > 1)
            {
                for (int i = 0; i < Passes; i++)
                {
                    lightSprite.Draw(gameTime);
                }
            }
            else
            {
                lightSprite.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (duration > 0)
            {
                duration -= gameTime.ElapsedGameTime.Milliseconds;
                if (duration <= 0)
                    TurnOff();
            }

            lightSprite.Update(gameTime);

            if (opacityTween.IsRunning)
            {
                opacityTween.Update(gameTime);
                lightSprite.Opacity = opacityTween.CurrentValue;
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
                    Invalidate();
                }
            }
        } = Color.White;

        // IsEmitting
        public bool IsEmitting => lightSprite.Opacity > 0 || (opacityTween.IsRunning && opacityTween.EndValue > opacityTween.StartValue);

        // LightKind
        public LightKind LightKind
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
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
                    Invalidate();
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

        // TurnOff
        public void TurnOff()
        {
            TurnOff(false);
        }

        // TurnOff
        public void TurnOff(bool immediate)
        {
            if (immediate)
                opacityTween.Stop();

            if (lightSprite.Opacity == 0 || (opacityTween.IsRunning && opacityTween.EndValue == 0))
                return;

            if (unlitTweenDuration == 0 || immediate)
                lightSprite.Opacity = 0;
            else
                opacityTween.Start(TweenStyle.CubicIn, lightSprite.Opacity, 0, unlitTweenDuration);
        }

        // TurnOn
        public void TurnOn()
        {
            TurnOn(false);
        }

        // TurnOn
        public void TurnOn(bool immediate)
        {
            TurnOn(immediate, 0);
        }

        // TurnOn
        public void TurnOn(bool immediate, int duration)
        {
            if (duration <= 0)
                this.duration = -1;
            else
                this.duration = duration;

            if (immediate)
                opacityTween.Stop();

            if (lightSprite.Opacity == 1 || (opacityTween.IsRunning && opacityTween.EndValue == 1))
                return;

            if (litTweenDuration == 0 || immediate)
                lightSprite.Opacity = 1;
            else
                opacityTween.Start(TweenStyle.CubicIn, lightSprite.Opacity, 1, litTweenDuration);
        }

        // X
        public float X
        {
            get => lightSprite.X;
            set => lightSprite.X = value;
        }

        // Y
        public float PositionY
        {
            get => lightSprite.Y;
            set => lightSprite.Y = value;
        }
    }
}
