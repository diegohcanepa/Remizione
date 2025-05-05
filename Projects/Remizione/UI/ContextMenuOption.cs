using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// ContextMenuOption
    /// </summary>
    public sealed class ContextMenuOption<TKey> : IDraw, IUpdate
    {
        private readonly ContextMenu<TKey> menu;
        private readonly TextSprite textSprite;

        // Constructor
        public ContextMenuOption(ContextMenu<TKey> menu, int index, TKey key, string text)
        {
            this.menu = menu;
            this.Key = key;
            this.Index = index;

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
            textSprite.Color = IsSelected ? ColorPalette.Text.Light : ColorPalette.Text.LightRed;
        }

        #endregion

        // Draw
        public void Draw(GameTime gameTime) => textSprite.Draw(gameTime);

        // Index
        public int Index { get; }

        // IsSelected
        public bool IsSelected => menu.SelectedIndex == Index;

        // Key
        public TKey Key { get; }

        // Position
        public Vector2 Position
        {
            get => textSprite.Position;
            set => textSprite.Position = value;
        }

        // TextBoundingBox
        public RectangleF TextBoundingBox => textSprite.BoundingBox;

        // ToString
        public override string ToString() => textSprite.ToString();

        // Update
        public void Update(GameTime gameTime)
        {
            UpdateColor();
        }
    }
}
