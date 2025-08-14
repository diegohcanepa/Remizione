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
        private GameSession session;
        private Item? item;
        private readonly TextSprite labelText;
        private readonly float minInterval = .12f;
        private readonly FloatTween opacityTween = new() { StartDelay = 1000 };
        private Prop? prop;
        private readonly Random rng = new();
        private float rollInterval = .04f;
        private readonly Vector2Tween scaleTween = new();
        private int successChance;
        private PropState? successState;

        #endregion

        #region Constructor

        // Constructor
        public UIChanceRoll(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Amount text
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Orange,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.Text.Galactus
            };

            // Label text
            this.labelText = new TextSprite(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.VeryLarge
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
            prop = null;
            item = null;
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
                        labelText.Text = TextRepository.GetValue("Misc.Success");
                        labelText.Color = ColorPalette.Text.Green;

                        if (prop != null && successState != null)
                            prop.PropState = successState.Value;
                    }
                    else
                    {
                        labelText.Text = TextRepository.GetValue("Misc.Failed");
                        labelText.Color = ColorPalette.Text.Red;
                        Sound.Play(SoundNames.ChanceRollFail);
                    }

                    labelText.Position = amountText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -2);

                    if (item != null)
                    {
                        item.Use();
                        
                        if (item.MetaItem.IsStackable)
                            session.HUD.Log.Show(LogVerb.Lost, item.DisplayText, item.MetaItem.Image);
                    }
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
        public void Show(Vector2 position, Item item, Prop prop, PropState? successState)
        {
            Reset();

            this.item = item;
            this.prop = prop;
            this.successState = successState;

            amountText.Position = position;
            successChance = item.Chance - prop.ChancePenalty;
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
