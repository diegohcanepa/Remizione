using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UIDerivedStats
    /// </summary>
    public class UIDerivedStats : GameObject
    {
        private Actor? actor;
        private readonly ColorTween[] colorTweens;
        private readonly ImageSprite[] icons;
        private readonly int[] lastKnownMaxValues;
        private readonly int[] lastKnownValues;
        private readonly TextSprite[] maxValues;
        private readonly Vector2Tween[] scaleTweens;
        private const int statCount = 3;
        private readonly TextSprite[] values;

        // Constructor
        public UIDerivedStats(RemizioneGame game)
            : base(game)
        {
            colorTweens = new ColorTween[statCount];
            for (var i = 0; i < statCount; i++)
            {
                colorTweens[i] = new ColorTween();
            }

            lastKnownMaxValues = new int[statCount];
            for (var i = 0; i < statCount; i++)
            {
                lastKnownMaxValues[i] = int.MinValue;
            }

            lastKnownValues = new int[statCount];
            for (var i = 0; i < statCount; i++)
            {
                lastKnownValues[i] = int.MinValue;
            }

            scaleTweens = new Vector2Tween[statCount];
            for (var i = 0; i < statCount; i++)
            {
                scaleTweens[i] = new Vector2Tween();
            }

            icons = new ImageSprite[3];
            icons[0] = new ImageSprite(Game, Atlases.UI.SpiritIcon) { Scale = ScaleInfo.UIElement.Small };
            icons[1] = new ImageSprite(Game, Atlases.UI.FaithIcon) { Scale = ScaleInfo.UIElement.Small };
            icons[2] = new ImageSprite(Game, Atlases.UI.WillpowerIcon) { Scale = ScaleInfo.UIElement.Small };

            icons[0].Position = new(4);
            icons[1].Position = icons[0].BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);
            icons[2].Position = icons[1].BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);

            this.maxValues = new TextSprite[3];
            this.values = new TextSprite[3];

            for (var i = 0; i < values.Length; i++)
            {
                this.values[i] = new TextSprite(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.StatMeter.CurrentValue,
                    PivotOrigin = RectanglePoint.Left,
                    Scale = ScaleInfo.Text.VeryLarge
                };

                this.maxValues[i] = new TextSprite(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.StatMeter.MaximumValue,
                    PivotOrigin = RectanglePoint.Left,
                    Scale = ScaleInfo.Text.Small
                };

                this.values[i].Position = icons[i].BoundingBox.GetPoint(RectanglePoint.Right, 1, .5f);
            }
        }

        #region Private members

        // InvalidateCore
        private void InvalidateCore(int index, int value, int maxValue, bool animate)
        {
            if (value < 0)
                value = 0;

            if (lastKnownValues[index] == value)
                return;

            var color = lastKnownValues[index] > value ? ColorPalette.Text.Red : ColorPalette.Text.Green;
            lastKnownValues[index] = value;
            values[index].Text = value.ToString();

            if (lastKnownMaxValues[index] != maxValue)
            {
                lastKnownMaxValues[index] = maxValue;
                maxValues[index].Text = " | " + maxValue.ToString();
            }

            maxValues[index].Position = values[index].BoundingBox.GetPoint(RectanglePoint.Right, -.5f, 0);

            if (animate)
            {
                colorTweens[index].Start(TweenStyle.Linear, ColorPalette.Text.Terra, color, 350, 6);
                scaleTweens[index].Start(TweenStyle.Linear, values[index].Scale, values[index].Scale * .9f, 250, 2);
                values[index].Tweens.ColorTween = colorTweens[index];
                values[index].Tweens.ScaleTween = scaleTweens[index];
            }
        }

        // Invalidate
        private void Invalidate(bool animate)
        {
            if (actor != null)
            {
                InvalidateCore(0, actor.HP, actor.MaxHP, animate);
                InvalidateCore(1, actor.Faith, actor.MaxFaith, animate);
                InvalidateCore(2, actor.Willpower, actor.MaxWillpower, animate);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            icons[0].Draw(gameTime);
            icons[1].Draw(gameTime);
            icons[2].Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            values[0].Draw(gameTime);
            values[1].Draw(gameTime);
            values[2].Draw(gameTime);
            maxValues[0].Draw(gameTime);
            maxValues[1].Draw(gameTime);
            maxValues[2].Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Invalidate(true);
            values[0].Update(gameTime);
            values[1].Update(gameTime);
            values[2].Update(gameTime);
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get => actor;
            set
            {
                if (value != actor)
                {
                    actor = value;

                    if (actor == null)
                    {
                        lastKnownValues[0] = int.MinValue;
                        lastKnownValues[1] = int.MinValue;
                        lastKnownValues[2] = int.MinValue;

                        lastKnownMaxValues[0] = int.MinValue;
                        lastKnownMaxValues[1] = int.MinValue;
                        lastKnownMaxValues[2] = int.MinValue;
                    }

                    Invalidate(false);
                }
            }
        }
    }
}
