using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle.UI
{
    /// <summary>
    /// UIHPInfo
    /// </summary>
    public sealed class UIHPInfo : GameObject
    {
        private readonly Sprite heartIcon;
        private readonly TextSprite amountText;

        // Constructor
        public UIHPInfo()
        {
            this.heartIcon = new(Atlases.UI.HeartFull)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.UIElement.Medium
            };

            this.amountText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Huge
            };
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            if (Target == null)
                return;

            heartIcon.Position = Target.GetOverheadPosition() + new Vector2(0, -2);
            amountText.Position = heartIcon.BoundingBox.GetPoint(RectanglePoint.Right, .5f, .65f);
            amountText.Text = Target.HP.ToString();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Target == null || Target.Session.IsAwaiting)
                return;

            Game.SpriteBatch.Begin(Target.Session.Camera);
            heartIcon.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        #endregion

        // Target
        public GameThing? Target
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
