using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIHealthBonus
    /// </summary>
    public sealed class UIHealthBonus : GameObject
    {
        private readonly ImageSprite icon;
        private DiceExpression? value;
        private readonly TextSprite valueText;

        // Constructor
        public UIHealthBonus(EngendroGame game)
            : base(game)
        {
            // Icon
            this.icon = new(Game, Atlases.UI.HeartIconWithShadow)
            {
                Scale = ScaleInfo.UIElement.Medium
            };

            // ValueText
            this.valueText = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Large,
                ShadowOffset = new(0, .75f)
            };
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            valueText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 1, .5f);
            BoundingBox = RectangleF.Union(valueText.BoundingBox, icon.BoundingBox);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            valueText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Position
        public Vector2 Position
        {
            get => icon.Position;
            set
            {
                if (value != icon.Position)
                {
                    icon.Position = value;
                    Invalidate();
                }
            }
        }

        // Value
        public DiceExpression? Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    valueText.Text = value == null ? null : "+" + value.GetValueRangeAsString();
                    Invalidate();
                }
            }
        }
    }
}
