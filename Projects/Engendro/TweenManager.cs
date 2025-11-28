using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// TweenManager
    /// </summary>
    public sealed class TweenManager
    {
        #region Private fields


        #endregion

        #region Constructor

        // Constructor
        internal TweenManager(Sprite sprite)
        {
            this.Sprite = sprite;
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            // AltitudeTween
            if (AltitudeTween is FloatTween altitudeTween)
            {
                altitudeTween.Update(gameTime, Sprite.TimeScale);
                Sprite.Altitude = altitudeTween.CurrentValue;
                if (altitudeTween.State == RunningState.Stopped && altitudeTween == this.AltitudeTween)
                    AltitudeTween = null;
            }

            // ColorTween
            if (ColorTween is ColorTween colorTween)
            {
                colorTween.Update(gameTime, Sprite.TimeScale);
                Sprite.Color = colorTween.CurrentValue;
                if (colorTween.State == RunningState.Stopped && colorTween == this.ColorTween)
                    ColorTween = null;
            }

            // OpacityTween
            if (OpacityTween is FloatTween opacityTween)
            {
                opacityTween.Update(gameTime, Sprite.TimeScale);
                Sprite.Opacity = opacityTween.CurrentValue;
                if (opacityTween.State == RunningState.Stopped && opacityTween == this.OpacityTween)
                    OpacityTween = null;
            }

            // PositionTween
            if (PositionTween is Vector2Tween positionTween)
            {
                positionTween.Update(gameTime, Sprite.TimeScale);
                Sprite.Position = positionTween.CurrentValue;
                if (positionTween.State == RunningState.Stopped && positionTween == this.PositionTween)
                    PositionTween = null;
            }

            // XTween
            if (XTween is FloatTween xTween)
            {
                xTween.Update(gameTime, Sprite.TimeScale);
                Sprite.X = xTween.CurrentValue;
                if (xTween.State == RunningState.Stopped && xTween == this.XTween)
                    XTween = null;
            }

            // YTween
            if (YTween is FloatTween yTween)
            {
                yTween.Update(gameTime, Sprite.TimeScale);
                Sprite.Y = yTween.CurrentValue;
                if (yTween.State == RunningState.Stopped && yTween == this.YTween)
                    YTween = null;
            }

            // RotationTween
            if (RotationTween is FloatTween rotationTween)
            {
                rotationTween.Update(gameTime, Sprite.TimeScale);
                Sprite.Degrees = rotationTween.CurrentValue;
                if (rotationTween.State == RunningState.Stopped && rotationTween == this.RotationTween)
                    RotationTween = null;
            }

            // ScaleTween
            if (ScaleTween is Vector2Tween scaleTween)
            {
                scaleTween.Update(gameTime, Sprite.TimeScale);
                Sprite.Scale = scaleTween.CurrentValue;
                if (scaleTween.State == RunningState.Stopped && scaleTween == this.ScaleTween)
                    ScaleTween = null;
            }
        }

        #endregion

        // AltitudeTween
        public FloatTween? AltitudeTween
        {
            get;
            set
            {
                field = value;
                if (field != null && field.State != RunningState.Stopped)
                    Sprite.Altitude = field.CurrentValue;
            }
        }

        // ColorTween
        public ColorTween? ColorTween
        {
            get;
            set
            {
                field = value;
                if (field != null && field.State != RunningState.Stopped)
                    Sprite.Color = field.CurrentValue;
            }
        }

        // IsTweening
        public bool IsTweening => ColorTween != null || PositionTween != null || XTween != null ||
                                  YTween != null || RotationTween != null || ScaleTween != null ||
                                  OpacityTween != null || AltitudeTween != null;

        // IsTweeningAltitude
        public bool IsTweeningAltitude => AltitudeTween != null;

        // IsTweeningColor
        public bool IsTweeningColor => ColorTween != null;

        // IsTweeningOpacity
        public bool IsTweeningOpacity => OpacityTween != null;

        // IsTweeningPosition
        public bool IsTweeningPosition => PositionTween != null || XTween != null || YTween != null;

        // IsTweeningX
        public bool IsTweeningX => XTween != null;

        // IsTweeningY
        public bool IsTweeningY => YTween != null;

        // IsTweeningRotation
        public bool IsTweeningRotation => RotationTween != null;

        // IsTweeningScale
        public bool IsTweeningScale => ScaleTween != null;

        // OpacityTween
        public FloatTween? OpacityTween
        {
            get;
            set
            {
                field = value;
                if (field != null && field.State != RunningState.Stopped)
                    Sprite.Opacity = field.CurrentValue;
            }
        }

        // PositionTween
        public Vector2Tween? PositionTween
        {
            get;
            set
            {
                field = value;

                if (value != null)
                {
                    XTween = null;
                    YTween = null;

                    if (value.State != RunningState.Stopped)
                        Sprite.Position = value.CurrentValue;
                }
            }
        }

        // Reset
        public void Reset()
        {
            AltitudeTween = null;
            ColorTween = null;
            OpacityTween = null;
            PositionTween = null;
            RotationTween = null;
            ScaleTween = null;
            XTween = null;
            YTween = null;
        }

        // RotationTween
        public FloatTween? RotationTween
        {
            get;
            set
            {
                field = value;
                if (field != null && field.State != RunningState.Stopped)
                    Sprite.Rotation = field.CurrentValue;
            }
        }

        // ScaleTween
        public Vector2Tween? ScaleTween
        {
            get;
            set
            {
                field = value;
                if (field != null && field.State != RunningState.Stopped)
                    Sprite.Scale = field.CurrentValue;
            }
        }

        // Sprite
        public Sprite Sprite { get; }

        // XTween
        public FloatTween? XTween
        {
            get;
            set
            {
                field = value;

                if (field != null)
                {
                    PositionTween = null;

                    if (value != null && field.State != RunningState.Stopped)
                        Sprite.X = value.CurrentValue;
                }
            }
        }

        // YTween
        public FloatTween? YTween
        {
            get;
            set
            {
                field = value;

                if (field != null)
                {
                    PositionTween = null;

                    if (value != null && field.State != RunningState.Stopped)
                        Sprite.Y = value.CurrentValue;
                }
            }
        }
    }
}
