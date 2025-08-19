using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione.UI
{
    /// <summary>
    /// UIGameMeter
    /// </summary>
    public class UIGameMeter : GameObject
    {
        private readonly ImageSprite container;
        private readonly ImageSprite icon;
        private int lastKnownValue = -1;
        private readonly GameSession session;
        private readonly TextSprite valueText;

        // Constructor
        public UIGameMeter(GameSession session, GameMeterUnit unit)
            : base(session.Game)
        {
            this.session = session;
            this.Unit = unit;

            // Container
            this.container = new(Game, Atlases.UI.GetImageNotNull("GameMeterContainer"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Icon
            this.icon = new(Game, Atlases.UI.GetImageNotNull($"{unit}Icon"))
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Value text
            this.valueText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.Text.Large
            };

            Invalidate();
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            icon.Position = container.BoundingBox.GetPoint(RectanglePoint.Right, -5, 0);

            if (Unit == GameMeterUnit.Stage)
            {
                icon.X -= .6f;
                icon.Y -= .2f;
            }

            else if (Unit == GameMeterUnit.Time)
            {
                icon.X -= .5f;
                icon.Y -= .5f;
            }

            valueText.Position = container.BoundingBox.GetPoint(RectanglePoint.Right, -21, .2f);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            container.Draw(gameTime);
            icon.Draw(gameTime);
            valueText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            // Energy
            if (Unit == GameMeterUnit.Energy)
            {
                if (lastKnownValue != session.Energy)
                {
                    lastKnownValue = session.Energy;
                    valueText.Text = $"{session.Energy}/{session.RequiredEnergy}";
                }
            }

            // Level
            else if (Unit == GameMeterUnit.Stage)
            {
                if (lastKnownValue != session.Stage)
                {
                    lastKnownValue = session.Stage;
                    valueText.Text = $"{session.Stage}/{GameSettings.MaximumLevel}";
                }
            }

            // Time
            else if (Unit == GameMeterUnit.Time)
            {
                if (lastKnownValue != session.RemainingTime)
                {
                    lastKnownValue = session.RemainingTime;
                    var t = TimeSpan.FromMilliseconds(session.RemainingTime);
                    valueText.Text = string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);

                    if (lastKnownValue <= GameSettings.TimeCritical)
                        valueText.Color = ColorPalette.Text.Red;
                    else if (lastKnownValue <= GameSettings.TimeWarning)
                        valueText.Color = ColorPalette.Text.Highlight;
                    else
                        valueText.Color = ColorPalette.Text.Default;
                }
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => container.BoundingBox;

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => container.PivotOrigin;
            set
            {
                container.PivotOrigin = value;
                Invalidate();
            }
        }   

        // Position
        public Vector2 Position
        {
            get => container.Position;
            set
            {
                container.Position = value;
                Invalidate();
            }
        }

        // Unit
        public GameMeterUnit Unit { get; }
    }
}
