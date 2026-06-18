using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// UIAmountDisplay
    /// </summary>
    public sealed class UIAmountDisplay : GameObject
    {
        private readonly TextSprite currentText;
        private readonly TextSprite maximumText;

        // Constructor
        public UIAmountDisplay()
        {
            this.currentText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.VeryLarge
            };

            this.maximumText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                Opacity = .6f,
                PivotOrigin = RectanglePoint.LeftBottom,
                Scale = ScaleInfo.Text.Medium
            };
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            currentText.Position = Position;
            currentText.Text = Current.ToString(CultureInfo.InvariantCulture);

            if (Current == 0)
                currentText.Color = ColorPalette.Text.Terra;
            else if (Current == Maximum)
                currentText.Color = ColorPalette.Text.Green;
            else
                currentText.Color = ColorPalette.Text.Highlight;

            maximumText.Position = currentText.BoundingBox.GetPoint(RectanglePoint.RightBottom, -.5f, -1);
            maximumText.Text = $" | {Maximum}";
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            currentText.Draw(gameTime);
            maximumText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        // Current
        public int Current
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

        // Maximum
        public int Maximum
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
        }
    }
}
