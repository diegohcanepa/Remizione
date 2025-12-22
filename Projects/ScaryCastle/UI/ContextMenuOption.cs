using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ContextMenuOption
    /// </summary>
    public sealed class ContextMenuOption<TKey>
    {
        private readonly ContextMenu<TKey> menu;
        private readonly TextSprite textSprite;

        // Constructor
        public ContextMenuOption(ContextMenu<TKey> menu, TKey key, string text, object? tag = null)
        {
            this.menu = menu;
            this.Key = key;
            this.Tag = tag;

            this.textSprite = new TextSprite(menu.Game, menu.Font)
            {
                Scale = menu.TextScale,
                Text = text
            };

            UpdateColor();
        }

        #region Private members

        // UpdateColor
        private void UpdateColor()
        {
            textSprite.Color = IsSelected ? ColorPalette.Text.TerraLight : ColorPalette.Text.Default;
        }

        #endregion

        // Draw
        public void Draw(GameTime gameTime)
        {
            textSprite.Draw(gameTime);
        }

        // Index
        public int Index { get; }

        // IsSelected
        public bool IsSelected => menu.SelectedOption == this;

        // Key
        public TKey Key { get; }

        // Position
        public Vector2 Position
        {
            get => textSprite.Position;
            set => textSprite.Position = value;
        }

        // Tag
        public object? Tag { get; set; }

        // TextBoundingBox
        public RectangleF TextBoundingBox => textSprite.BoundingBox;

        // ToString
        public override string ToString()
        {
            return textSprite.ToString();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            UpdateColor();
        }
    }
}
