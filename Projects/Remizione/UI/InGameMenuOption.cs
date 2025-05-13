using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione.UI
{
    /// <summary>
    /// InGameMenuOption
    /// </summary>
    public sealed class InGameMenuOption<TKey> where TKey : Enum
    {
        private readonly InGameMenu<TKey> menu;
        private readonly TextSprite text;

        // Constructor
        public InGameMenuOption(InGameMenu<TKey> menu, TKey key, string text, Action action)
        {
            this.menu = menu;
            this.Key = key;
            this.Action = action;

            this.text = new TextSprite(menu.Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.Text.VeryLarge,
                Text = text
            };

            UpdateColor();
        }

        #region Private members

        // UpdateColor
        private void UpdateColor()
        {
            if (IsSelected)
                text.Color = ColorPalette.Text.Highlight;

            else if (IsHovered )
                text.Color = ColorPalette.Text.Hover;
            
            else
                text.Color = ColorPalette.Text.Default;
        }

        #endregion

        // Action
        public Action Action { get; }

        // BoundingBox
        public RectangleF BoundingBox => text.BoundingBox;

        // Draw
        public void Draw(GameTime gameTime) => text.Draw(gameTime);

        // Index
        public int Index { get; }

        // IsHovered
        public bool IsHovered => menu.HoveredOption == this;

        // IsSelected
        public bool IsSelected => menu.SelectedOption == this;

        // Key
        public TKey Key { get; }

        // Position
        public Vector2 Position
        {
            get => text.Position;
            set => text.Position = value;
        }

        // ToString
        public override string ToString() => text.ToString();

        // Update
        public void Update(GameTime gameTime) => UpdateColor();
    }
}
