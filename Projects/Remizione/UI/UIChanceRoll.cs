using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private readonly ImageSprite bottomGradient;
        private readonly HUD hud;
        private Item? item;
        private readonly TextSprite labelText;
        private readonly FloatTween opacityTween = new() { StartDelay = 1000 };
        private Prop? prop;
        private readonly Random random = new();
        private int successChance;
        private PropState? successState;
        private int targetUnit;
        private int targetTen;
        private int ten;
        private const float tenDeceleration = .0015f;
        private float tenInterval = .02f;
        private bool tenStopped;
        private float tenTimer = 0;
        private int unit;
        private const float unitDeceleration = .0015f; // cuánto aumenta el intervalo por frame
        private float unitInterval = .02f; // tiempo entre cambios al inicio
        private bool unitStopped;
        private float unitTimer = 0;

        #endregion

        #region Constructor

        // Constructor
        public UIChanceRoll(HUD hud)
            : base(hud.Game)
        {
            this.hud = hud;

            // Bottom gradient
            bottomGradient = new ImageSprite(hud.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // Amount text
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Orange,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -5),
                Scale = ScaleInfo.Text.Galactus
            };

            // Label text
            this.labelText = new TextSprite(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Huge
            };
        }

        #endregion

        #region Private members

        // Reset
        private void Reset()
        {
            opacityTween.Stop();
            Success = false;
            IsRolling = false;
            IsVisible = false;
            amountText.Color = ColorPalette.Text.Default;
            amountText.Opacity = 1;
            labelText.Opacity = 1;
            prop = null;
            item = null;
            ten = 0;
            tenInterval = 0.05f;
            tenStopped = false;
            tenTimer = 0;
            unit = 0;
            unitInterval = 0.05f;
            unitStopped = false;
            unitTimer = 0;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsVisible)
            {
                // Gradient
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                bottomGradient.Draw(gameTime);
                Game.SpriteBatch.End();

                Game.SpriteBatch.Begin(Game.Camera);
                amountText.Draw(gameTime);

                if (!IsRolling)
                    labelText.Draw(gameTime);

                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Unit
            if (!unitStopped)
            {
                unitTimer += deltaTime;
                if (unitTimer >= unitInterval)
                {
                    unitTimer = 0f;
                    unit = random.Next(0, 10);

                    // Incrementa el intervalo para desacelerar
                    unitInterval += unitDeceleration;

                    // Condición para frenar unidad: probabilidad aleatoria
                    if (random.NextDouble() < 0.1)
                    {
                        unitStopped = true;
                        unit = targetUnit; // fijar valor real
                    }
                }
            }

            // Ten
            if (!tenStopped)
            {
                tenTimer += deltaTime;
                if (tenTimer >= tenInterval)
                {
                    tenTimer = 0f;
                    ten = random.Next(0, 10);

                    // Incrementa el intervalo para desacelerar
                    tenInterval += tenDeceleration;

                    // Solo empieza a frenar si unidad ya frenó
                    if (unitStopped && random.NextDouble() < 0.1)
                    {
                        tenStopped = true;
                        ten = targetTen; // fijar valor real
                    }
                }
            }

            if (IsRolling)
                amountText.Text = $"{ten}{unit}";

            if (IsRolling && tenStopped && unitStopped)
            {
                if (ten == 0 && unit == 0)
                    amountText.Text = "100";

                IsRolling = false;

                amountText.Color = Success ? ColorPalette.Text.Green : ColorPalette.Text.Red;

                opacityTween.Start(TweenStyle.CubicInOut, 1, 0, 600);

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

                labelText.Position = amountText.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2);

                if (item != null)
                {
                    item.Use();

                    if (item.MetaItem.IsStackable)
                        hud.Log.Show(LogVerb.Lost, item.DisplayText, item.MetaItem.Image);
                }
            }

            if (opacityTween.IsRunning)
            {
                opacityTween.Update(gameTime);
                amountText.Opacity = opacityTween.CurrentValue;
                labelText.Opacity = opacityTween.CurrentValue;

                if (!opacityTween.IsRunning)
                    IsVisible = false;
            }
        }

        #endregion

        // IsRolling
        public bool IsRolling { get; private set; }

        // IsVisible
        public bool IsVisible { get; private set; }

        // Show
        public void Show(Item item, Prop prop, PropState? successState)
        {
            Reset();

            var finalValue = random.Next(0, 100);

            this.item = item;
            this.prop = prop;
            this.successState = successState;

            successChance = item.SkillChance - prop.SkillChancePenalty;
            Success = finalValue <= successChance;

            IsRolling = true;
            IsVisible = true;
            Sound.Play(SoundNames.ChanceRoll);

            // Cifras reales
            if (finalValue == 100)
            {
                targetTen = 0;
                targetUnit = 0;
            }
            else
            {
                targetTen = finalValue / 10;
                targetUnit = finalValue % 10;
            }
        }

        // Success
        public bool Success { get; private set; }
    }
}
