using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// UIChanceRoll
    /// </summary>
    public sealed class UIChanceRoll : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly float deceleration = .01f;
        private float elapsedSinceChange = 0;
        private int finalNumber = 1;
        private readonly TextSprite labelText;
        private readonly float minInterval = .12f;
        private readonly FloatTween opacityTween = new() { StartDelay = 1000 };
        private readonly Random rng = new();
        private float rollInterval = .04f;
        private readonly Vector2Tween scaleTween = new();
        private int successChance;

        #endregion

        #region Constructor

        // Constructor
        public UIChanceRoll(EngendroGame game)
            : base(game)
        {
            // Amount text
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Orange,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Galactus
            };

            // Label text
            this.labelText = new TextSprite(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.Huge
            };
        }

        #endregion

        #region Private members

        // Reset
        private void Reset()
        {
            opacityTween.Stop();
            amountText.Tweens.Reset();
            Success = false;
            IsRolling = false;
            IsVisible = false;
            rollInterval = .02f;
            finalNumber = rng.Next(1, 101);
            elapsedSinceChange = 0;
            amountText.Color = ColorPalette.Text.Default;
            amountText.Opacity = 1;
            labelText.Opacity = 1;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsVisible)
            {
                amountText.Draw(gameTime);

                if (!IsRolling)
                    labelText.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (IsRolling)
            {
                elapsedSinceChange += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Change display number
                if (elapsedSinceChange >= rollInterval)
                {
                    elapsedSinceChange = 0;
                    amountText.Text = $"{rng.Next(1, 101)}";
                    rollInterval += deceleration;
                }

                // If it slow enough then show final result
                if (rollInterval >= minInterval)
                {
                    amountText.Text = $"{finalNumber}";
                    IsRolling = false;
                    Success = finalNumber <= successChance;
                    amountText.Color = Success ? ColorPalette.Text.Green : ColorPalette.Text.Red;
                    opacityTween.Start(TweenStyle.CubicInOut, 1, 0, 400);

                    if (Success)
                    {
                        labelText.Text = "Success!";
                        labelText.Color = ColorPalette.Text.Green;
                    }
                    else
                    {
                        labelText.Text = "Failed!";
                        labelText.Color = ColorPalette.Text.Red;
                        Sound.Play(SoundNames.ChanceRollFail);
                    }

                    labelText.Position = amountText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -3);
                }
            }
            
            amountText.Update(gameTime);

            if (opacityTween.IsRunning)
            {
                opacityTween.Update(gameTime);
                amountText.Opacity = opacityTween.CurrentValue;
                labelText.Opacity = opacityTween.CurrentValue;
            }

            if (!IsRolling && IsVisible && !opacityTween.IsRunning)
                IsVisible = false;
        }

        #endregion

        // IsRolling
        public bool IsRolling { get; private set; }

        // IsVisible
        public bool IsVisible { get; private set; }

        // Show
        public void Show(Vector2 position, int chance)
        {
            Reset();

            amountText.Position = position;
            successChance = chance;
            IsRolling = true;
            IsVisible = true;
            
            scaleTween.Start(TweenStyle.CubicInOut, ScaleInfo.Text.Galactus, ScaleInfo.Text.Galactus * .8f, 50, 10);
            amountText.Tweens.ScaleTween = scaleTween;

            Sound.Play(SoundNames.ChanceRoll);
        }

        // Success
        public bool Success { get; private set; }
    }
}
