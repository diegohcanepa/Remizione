using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UIDerivedStats
    /// </summary>
    internal class UIDerivedStats : GameObject
    {
        private Actor? actor;
        private readonly ImageSprite[] icons;
        private readonly int[] lastKnownMaxValues = [int.MinValue, int.MinValue];
        private readonly int[] lastKnownValues = [int.MinValue, int.MinValue];
        private readonly TextSprite[] maxValues;
        private readonly Vector2Tween[] scaleTweens = [new Vector2Tween(), new Vector2Tween()];
        private readonly TextSprite[] values;

        // Constructor
        public UIDerivedStats(RemizioneGame game)
            : base(game)
        {
            icons = new ImageSprite[2];
            icons[0] = new ImageSprite(Game, Atlases.UI.SpiritIcon) { Scale = ScaleInfo.UIIcon.Small };
            icons[1] = new ImageSprite(Game, Atlases.UI.FaithIcon) { Scale = ScaleInfo.UIIcon.Small };

            icons[0].Position = new(4);
            icons[1].Position = icons[0].BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);


            this.maxValues = new TextSprite[2];
            this.values = new TextSprite[2];

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

            if (lastKnownValues[index] != value)
            {
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
                    scaleTweens[index].Start(TweenStyle.Linear, values[index].Scale, values[index].Scale * .9f, 100, 2);
                    values[index].Tweens.ScaleTween = scaleTweens[index];
                }
            }
        }

        // Invalidate
        private void Invalidate(bool animate)
        {
            if (actor != null)
            {
                InvalidateCore(0, actor.HP, actor.MaxHP, animate);
                InvalidateCore(1, actor.Faith, actor.MaxFaith, animate);
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
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            values[0].Draw(gameTime);
            values[1].Draw(gameTime);
            maxValues[0].Draw(gameTime);
            maxValues[1].Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Invalidate(true);
            values[0].Update(gameTime);
            values[1].Update(gameTime);
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

                        lastKnownMaxValues[0] = int.MinValue;
                        lastKnownMaxValues[1] = int.MinValue;
                    }

                    Invalidate(false);
                }
            }
        }
    }
}
