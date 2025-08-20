using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione
{
    /// <summary>
    /// UIStageMeter
    /// </summary>
    public class UIStageMeter : GameObject
    {
        private readonly ImageSprite container;
        private readonly ColorTween colorTween = new();
        private static readonly Color energyTextColor = new(240, 181, 65);
        private readonly int[] lastKnownValues = new int[3];
        private readonly Vector2Tween scaleTween = new();
        private readonly GameSession session;
        private static readonly Color textColor = ColorPalette.Text.Default;    
        private static readonly Vector2 textSize = ScaleInfo.Text.VeryLarge;
        private readonly TextSprite[] values;

        // Constructor
        public UIStageMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Container
            this.container = new(Game, Atlases.UI.GetImageNotNull("GameMeterContainer"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -2, -1),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Value text
            values = new TextSprite[lastKnownValues.Length];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = new(Game, Fonts.CommonOutline)
                {
                    Color = textColor,
                    PivotOrigin = RectanglePoint.Top,
                    Scale = textSize
                };
            }

            Invalidate();
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            values[0].Position = container.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 6, -1);
            values[1].Position = container.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 23, -1);
            values[2].Position = container.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 40, -1);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            container.Draw(gameTime);
            for (var i = 0; i < values.Length; i++)
            {
                values[i].Draw(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            // Energy
            if (lastKnownValues[0] != session.Energy)
            {
                lastKnownValues[0] = session.Energy;
                values[0].Text = $"{session.Energy}/{session.RequiredEnergy}";
                values[0].Color = session.IsEnergyFull ? ColorPalette.Text.Green : textColor;

                if (session.IsEnergyFull)
                {
                    scaleTween.Stop();
                    colorTween.Stop();
                }
                else if (!scaleTween.IsRunning)
                {
                    scaleTween.Start(TweenStyle.CubicInOut, textSize, textSize * 1.2f, 150, 4);
                    values[0].Tweens.ScaleTween = scaleTween;

                    colorTween.Start(TweenStyle.CubicInOut, textColor, energyTextColor, 300, 2);
                    values[0].Tweens.ColorTween = colorTween;
                }
            }

            // RemainingTime
            if (lastKnownValues[1] != session.RemainingTime)
            {
                lastKnownValues[1] = session.RemainingTime;
                var t = TimeSpan.FromMilliseconds(session.RemainingTime);
                values[1].Text = string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);

                if (lastKnownValues[1] <= GameSettings.TimeCritical)
                    values[1].Color = ColorPalette.Text.Red;
                else if (lastKnownValues[1] <= GameSettings.TimeWarning)
                    values[1].Color = ColorPalette.Text.Highlight;
                else
                    values[1].Color = ColorPalette.Text.Default;
            }

            // Stage
            if (lastKnownValues[2] != session.Stage)
            {
                lastKnownValues[2] = session.Stage;
                values[2].Text = $"{session.Stage}";
            }


            values[0].Update(gameTime);
        }

        #endregion

        // Reset
        public void Reset()
        {
            for (var i = 0; i < lastKnownValues.Length; i++)
            {
                lastKnownValues[i] = -1;
                values[i].Clear();
                values[i].Color = ColorPalette.Text.Default;
            }
        }
    }
}
