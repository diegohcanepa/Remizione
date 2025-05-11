using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// ItemMenuOption
    /// </summary>
    public sealed class ItemMenuOption
    {
        private readonly ItemMenu menu;
        private readonly TextSprite nameText;

        // Constructor
        public ItemMenuOption(ItemMenu menu, Item item)
        {
            this.menu = menu;
            this.Item = item;

            this.nameText = new TextSprite(menu.Game, menu.Font)
            {
                PivotOrigin = RectanglePoint.Middle,
                MaximumWidth = (int)(menu.BoundingBox.Width * .9f),
                Scale = ScaleInfo.Text.Large,
            };

            Invalidate();

            UpdateColor();
}

        #region Private members

        // UpdateColor
        private void UpdateColor()
        {
            if (IsSelected)
                nameText.Color = ColorPalette.Text.Highlight;

            else if (IsHovered )
                nameText.Color = ColorPalette.Text.Hover;
            
            else
                nameText.Color = ColorPalette.Text.Default;
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Draw
        public void Draw(GameTime gameTime)
        {
            nameText.Draw(gameTime);
        }

        // Index
        public int Index { get; }

        // Invalidate
        public void Invalidate() => nameText.Text = Item.DisplayText;

        // IsHovered
        public bool IsHovered => menu.HoveredOption == this;

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
            }
        }

        // TextBoundingBox
        public RectangleF TextBoundingBox => nameText.BoundingBox;

        // ToString
        public override string ToString() => nameText.ToString();

        // Update
        public void Update(GameTime gameTime)
        {
            UpdateColor();
        }
    }
}
