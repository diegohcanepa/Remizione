using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// UIDerivedStatModifier
    /// </summary>
    public sealed class UIDerivedStatModifier : GameObject
    {
        #region Private fields

        private int amount;
        private readonly TextSprite amountText;
        private readonly ImageSprite containerImage;
        private DerivedStat stat;

        #endregion

        #region Constructor

        // Constructor
        public UIDerivedStatModifier(EngendroGame game, DerivedStat stat, int amount = 0)
            : base(game)
        {
            // Amount text
            this.amountText = new TextSprite(game, Fonts.CommonOutline)
            {
                Scale = ScaleInfo.Text.Medium
            };

            // Icon
            this.containerImage = new ImageSprite(game)
            {
                Scale = ScaleInfo.UIElement.Small
            };

            this.Stat = stat;
            this.Amount = amount;
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            containerImage.Image = Atlases.UI.GetImage($"{Stat}Icon");

            if (containerImage.Pivot.AtLeft)
            {
                amountText.PivotOrigin = RectanglePoint.Right;
                amountText.Position = containerImage.BoundingBox.GetPoint(RectanglePoint.Right);
            }
            else
            {
                amountText.PivotOrigin = RectanglePoint.Left;
                amountText.Position = containerImage.BoundingBox.GetPoint(RectanglePoint.Left);
            }

            amountText.Color = amount >= 0 ? ColorPalette.Text.Default : ColorPalette.Text.Red;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            containerImage.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            amountText.Update(gameTime);
            containerImage.Update(gameTime);
        }

        #endregion

        // Amount
        public int Amount
        {
            get => amount;
            set
            {
                if (value != amount)
                {
                    amount = value;
                    var sign = amount > 0 ? "+" : "-";
                    amountText.Text = sign + Math.Abs(amount).ToString(CultureInfo.InvariantCulture);
                    Invalidate();
                }
            }
        }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => containerImage.PivotOrigin;
            set
            {
                containerImage.PivotOrigin = value;
                Invalidate();
            }
        }

        // Position
        public Vector2 Position
        {
            get => containerImage.Position;
            set
            {
                containerImage.Position = value;
                Invalidate();
            }
        }

        // Stat
        public DerivedStat Stat
        {
            get => stat;
            set
            {
                if (value != stat)
                {
                    stat = value;
                    Invalidate();
                }
            }
        }
    }
}
