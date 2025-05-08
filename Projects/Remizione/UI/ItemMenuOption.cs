using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// ItemMenuOption
    /// </summary>
    public sealed class ItemMenuOption
    {
        private readonly TextSprite infoText;
        private readonly ItemMenu menu;
        private readonly TextSprite nameText;

        // Constructor
        public ItemMenuOption(ItemMenu menu, Item item)
        {
            this.menu = menu;
            this.Item = item;

            this.nameText = new TextSprite(menu.Game, menu.Font)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                MaximumWidth = (int)menu.Width-10,
                Scale = menu.TextScale,
                Text = item.MetaItem.LocalizedName
            };
            
            if (item.Level > 0)
                nameText.Text += $" +item.Level";

            this.infoText = new TextSprite(menu.Game, menu.Font)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Scale = menu.TextScale,
                Text = ""
            };

            UpdateColor();
}

        #region Private members

        // UpdateColor
        private void UpdateColor()
        {
            nameText.Color = IsSelected ? ColorPalette.Text.Highlight : ColorPalette.Text.Default;
            infoText.Color = nameText.Color;
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Draw
        public void Draw(GameTime gameTime)
        {
            nameText.Draw(gameTime);
            infoText.Draw(gameTime);
        }

        // Index
        public int Index { get; }

        // IsSelected
        public bool IsSelected => menu.SelectedOption == this;

        // Item
        public Item Item { get; }

        // Position
        public Vector2 Position
        {
            get => nameText.Position;
            set
            {
                nameText.Position = value;
             
                var box = menu.BoundingBox;
                BoundingBox = new(box.X + 1, nameText.BoundingBox.Top - 1, box.Width-2, nameText.BoundingBox.Height + 1);

                infoText.X = BoundingBox.Right - 2;
                infoText.Y = nameText.Y;
            }
        }

        // TextBoundingBox
        public RectangleF TextBoundingBox => RectangleF.Union(nameText.BoundingBox, infoText.BoundingBox);

        // ToString
        public override string ToString() => nameText.ToString();

        // Update
        public void Update(GameTime gameTime)
        {
            UpdateColor();
        }
    }
}
