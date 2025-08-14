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
        private readonly TextSprite amountText;
        private readonly float deceleration = .01f; // cuanto más lento se vuelve
        private float elapsedSinceChange = 0;
        private int finalNumber = 1;
        private readonly float minInterval = .12f;  // velocidad final
        private readonly FloatTween opacityTween = new() { StartDelay = 1000 };
        private readonly Random rng = new();
        private float rollInterval = .04f; // velocidad inicial del cambio
        private readonly Vector2Tween scaleTween = new();
        private int successChance; // % de éxito

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
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsVisible)
                amountText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (IsRolling)
            {
                elapsedSinceChange += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Cambia el número mostrado cada cierto intervalo
                if (elapsedSinceChange >= rollInterval)
                {
                    elapsedSinceChange = 0;
                    amountText.Text = $"{rng.Next(1, 101)}";
                    rollInterval += deceleration;
                }

                // Si ya está lo suficientemente lento, mostramos el número real y paramos
                if (rollInterval >= minInterval)
                {
                    amountText.Text = $"{finalNumber}";
                    IsRolling = false;
                    Success = finalNumber <= successChance;
                    amountText.Color = Success ? ColorPalette.Text.Green : ColorPalette.Text.Red;
                    opacityTween.Start(TweenStyle.CubicInOut, 1, 0, 400);
                    amountText.Tweens.OpacityTween = opacityTween;

                    if (!Success)
                        Sound.Play(SoundNames.ChanceRollFail);
                }
            }

            amountText.Update(gameTime);

            if (!IsRolling && IsVisible && !amountText.Tweens.IsTweening)
                IsVisible = false;
        }

        #endregion

        // IsRolling
        public bool IsRolling { get; private set; }

        // IsVisible
        public bool IsVisible { get; private set; }

        // Show
        public void Show(Vector2 position, int successChance)
        {
            amountText.Tweens.Reset();

            this.amountText.Position = position;
            this.successChance = successChance;
            this.Success = false;
            this.IsRolling = true;
            this.IsVisible = true;
            this.rollInterval = .02f;
            this.finalNumber = rng.Next(1, 101); // número real de la tirada
            this.elapsedSinceChange = 0;
            this.amountText.Color = ColorPalette.Text.Default;
            this.amountText.Opacity = 1;
            
            Sound.Play(SoundNames.ChanceRoll);
            
            scaleTween.Start(TweenStyle.CubicInOut, ScaleInfo.Text.Galactus, ScaleInfo.Text.Galactus * .8f, 50, 10);
            amountText.Tweens.ScaleTween = scaleTween;
        }

        // Success
        public bool Success { get; private set; }
    }
}
