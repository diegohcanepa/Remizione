using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// UIDerivedStatIcon
    /// </summary>
    public sealed class UIDerivedStatIcon : GameObject
    {
        private int amount;
        private readonly TextSprite amountText;
        private readonly ImageSprite icon;
        private DerivedStat stat;

        #region Constructor

        // Constructor
        public UIDerivedStatIcon(EngendroGame game, DerivedStat stat, int amount = 0)
            : base(game)
        {
            // Amount text
            this.amountText = new TextSprite(game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Medium,
            };

            // Icon
            this.icon = new ImageSprite(game)
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
            var offset = (icon.BoundingBox.Width / 2) - amountText.BoundingBox.Width + 1;
            amountText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, offset / 2, .5f);
            amountText.Color = Amount < 0 ? ColorPalette.UIDerivedStatIcon.NegativeAmount : ColorPalette.UIDerivedStatIcon.PositiveAmount;
            icon.Image = Atlases.UI.GetImage($"{Stat}AmountIcon");
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            amountText.Update(gameTime);
            icon.Update(gameTime);
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
                    var sign = amount < 0 ? "-" : "+";
                    amountText.Text = sign + Math.Abs(amount).ToString(CultureInfo.InvariantCulture);
                    Invalidate();
                }
            }   
        }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => icon.PivotOrigin;
            set
            {
                icon.PivotOrigin = value;
                Invalidate();
            }
        }

        // Position
        public Vector2 Position
        {
            get => icon.Position;
            set
            {
                icon.Position = value;
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
