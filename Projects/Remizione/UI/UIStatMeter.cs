using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// UIStatMeter
    /// </summary>
    public class UIStatMeter : SessionGameObject<GameSession>
    {
        private readonly ColorTween colorTween = new();
        private readonly Sprite icon = new() { Scale = ScaleInfo.UIElement.Small };
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private readonly TextSprite maxValueText;
        private readonly Vector2Tween scaleTween = new();
        private readonly TextSprite valueText;

        // Constructor
        public UIStatMeter(GameSession session, StatName stat, Vector2 position)
            : base(session)
        {
            this.Stat = stat;
            this.icon.Position = position;

            lastKnownMaxValue = int.MinValue;
            lastKnownValue = int.MinValue;
            icon.RenderImage = Atlases.UI.GetImage($"Stat{stat}Icon");

            this.valueText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.StatMeter.CurrentValue,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.VeryLarge
            };

            this.maxValueText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.StatMeter.MaximumValue,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Small
            };

            this.valueText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 1, .5f);
        }

        #region Private members

        // GetStatMaxValue
        private int GetStatMaxValue(StatName statName)
        {
            if (Actor == null)
                return 0;

            return statName switch
            {
                StatName.HP => Actor.MaxHP,
                StatName.Energy => Actor.MaxEnergy,
                _ => throw new ArgumentOutOfRangeException(nameof(statName), statName, null)
            };
        }

        // GetStatValue
        private int GetStatValue(StatName statName)
        {
            if (Actor == null)
                return 0;

            return statName switch
            {
                StatName.HP => Actor.HP,
                StatName.Energy => Actor.Energy,
                _ => throw new ArgumentOutOfRangeException(nameof(statName), statName, null)
            };
        }

        // Invalidate
        private void Invalidate(bool animate)
        {
            var value = GetStatValue(Stat);
            if (value < 0)
                value = 0;

            if (lastKnownValue == value)
                return;

            var maxValue = GetStatMaxValue(Stat);

            var color = lastKnownValue > value ? ColorPalette.Text.Red : ColorPalette.Text.Green;
            lastKnownValue = value;
            valueText.Text = value.ToString(CultureInfo.InvariantCulture);
            if (lastKnownMaxValue != maxValue)
            {
                lastKnownMaxValue = maxValue;
                maxValueText.Text = " | " + maxValue.ToString();
            }

            maxValueText.Position = valueText.BoundingBox.GetPoint(RectanglePoint.Right, -.5f, 0);

            if (animate)
            {
                colorTween.Start(TweenStyle.Linear, ColorPalette.Text.Terra, color, 350, 6);
                scaleTween.Start(TweenStyle.Linear, valueText.Scale, valueText.Scale * .9f, 250, 2);
                valueText.Tweens.ColorTween = colorTween;
                valueText.Tweens.ScaleTween = scaleTween;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            valueText.Draw(gameTime);
            maxValueText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Invalidate(true);
            valueText.Update(gameTime);
            maxValueText.Update(gameTime);
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field == null)
                    {
                        lastKnownValue = int.MinValue;
                        lastKnownMaxValue = int.MinValue;
                    }

                    Invalidate(false);
                }
            }
        }

        // Stat
        public StatName Stat { get; }
    }
}
