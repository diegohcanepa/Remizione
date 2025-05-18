using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione.UI
{
    /// <summary>
    /// PopupMenuOption
    /// </summary>
    public sealed class PopupMenuOption<TLinkedObject> where TLinkedObject : class
    {
        private readonly Action? action;
        private readonly PopupMenu<TLinkedObject> menu;
        private readonly TextSprite nameText;

        // Constructor
        public PopupMenuOption(PopupMenu<TLinkedObject> menu, TLinkedObject linkedObject, Action? action)
        {
            this.menu = menu;
            this.LinkedObject = linkedObject;
            this.action = action;

            this.nameText = new TextSprite(menu.Game, menu.Font)
            {
                MaximumWidth = (int)(menu.BoundingBox.Width * .9f),
                Scale = ScaleInfo.Text.Medium,
            };

            if (menu.HorizontalAlignment == HorizontalAlignment.Center)
                nameText.PivotOrigin = RectanglePoint.Middle;

            else if (menu.HorizontalAlignment == HorizontalAlignment.Left)
                nameText.PivotOrigin = RectanglePoint.LeftTop;

            else
                nameText.PivotOrigin = RectanglePoint.RightTop;

            Invalidate();

            UpdateColor();
        }

        #region Private members

        // UpdateColor
        private void UpdateColor()
        {
            if (menu.HideSelectedOption)
                nameText.Color = ColorPalette.Text.Default;

            else if (IsSelected)
                nameText.Color = ColorPalette.Text.Highlight;

            else if (IsHovered)
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

        // Execute
        public void Execute() => action?.Invoke();

        // Index
        public int Index { get; }

        // Invalidate
        public void Invalidate()
        {
            nameText.Text = LinkedObject.ToString();
            nameText.Scale = menu.TextScale;
        }

        // IsHovered
        public bool IsHovered => menu.HoveredOption == this;

        // IsSelected
        public bool IsSelected => menu.SelectedOption == this;

        // LinkedObject
        public TLinkedObject LinkedObject { get; }

        // Position
        public Vector2 Position
        {
            get => nameText.Position;
            set
            {
                nameText.Position = value;
                if (menu.BoundingBox.IsEmpty)
                {
                    BoundingBox = nameText.BoundingBox;
                }
                else
                {
                    var box = menu.BoundingBox;
                    BoundingBox = new(box.X + 1, nameText.BoundingBox.Top - 1, box.Width - 2, nameText.BoundingBox.Height + 1);
                }
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
