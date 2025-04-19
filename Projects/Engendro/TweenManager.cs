using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// TweenManager
    /// </summary>
    public sealed class TweenManager
    {
        #region Private fields

        private FloatTween? altitudeTween;
        private ColorTween? colorTween;
        private FloatTween? opacityTween;
        private Vector2Tween? positionTween;
        private FloatTween? rotationTween;
        private Vector2Tween? scaleTween;
        private FloatTween? xTween;
        private FloatTween? yTween;

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
            get => altitudeTween;
            set
            {
                altitudeTween = value;
                if (altitudeTween != null && altitudeTween.State != RunningState.Stopped)
                    Sprite.Altitude = altitudeTween.CurrentValue;
            }
        }

        // ColorTween
        public ColorTween? ColorTween
        {
            get => colorTween;
            set
            {
                colorTween = value;
                if (colorTween != null && colorTween.State != RunningState.Stopped)
                    Sprite.Color = colorTween.CurrentValue;
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
            get => opacityTween;
            set
            {
                opacityTween = value;
                if (opacityTween != null && opacityTween.State != RunningState.Stopped)
                    Sprite.Opacity = opacityTween.CurrentValue;
            }
        }

        // PositionTween
        public Vector2Tween? PositionTween
        {
            get => positionTween;
            set
            {
                positionTween = value;

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
            get => rotationTween;
            set
            {
                rotationTween = value;
                if (rotationTween != null && rotationTween.State != RunningState.Stopped)
                    Sprite.Rotation = rotationTween.CurrentValue;
            }
        }

        // ScaleTween
        public Vector2Tween? ScaleTween
        {
            get => scaleTween;
            set
            {
                scaleTween = value;
                if (scaleTween != null && scaleTween.State != RunningState.Stopped)
                    Sprite.Scale = scaleTween.CurrentValue;
            }
        }

        // Sprite
        public Sprite Sprite { get; }

        // XTween
        public FloatTween? XTween
        {
            get => xTween;
            set
            {
                xTween = value;

                if (xTween != null)
                {
                    PositionTween = null;

                    if (value != null && xTween.State != RunningState.Stopped)
                        Sprite.X = value.CurrentValue;
                }
            }
        }

        // YTween
        public FloatTween? YTween
        {
            get => yTween;
            set
            {
                yTween = value;

                if (yTween != null)
                {
                    PositionTween = null;

                    if (value != null && yTween.State != RunningState.Stopped)
                        Sprite.Y = value.CurrentValue;
                }
            }
        }
    }
}
