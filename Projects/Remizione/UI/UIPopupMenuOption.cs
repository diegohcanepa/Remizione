using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione
{
    /// <summary>
    /// UIPopupMenuOption
    /// </summary>
    public sealed class UIPopupMenuOption<TLinkedObject> where TLinkedObject : class
    {
        private readonly Action? action;
        private readonly Sprite icon;
        private readonly UIPopupMenu<TLinkedObject> menu;
        private readonly TextSprite nameText;

        // Constructor
        public UIPopupMenuOption(UIPopupMenu<TLinkedObject> menu, TLinkedObject linkedObject, Action? action)
        {
            this.menu = menu;
            this.LinkedObject = linkedObject;
            this.action = action;

            // Name
            this.nameText = new(menu.Font)
            {
                MaximumWidth = (int)(menu.BoundingBox.Width * .9f)
            };

            if (menu.HorizontalAlignment == HorizontalAlignment.Center)
                nameText.PivotOrigin = RectanglePoint.Center;

            else if (menu.HorizontalAlignment == HorizontalAlignment.Left)
                nameText.PivotOrigin = RectanglePoint.LeftTop;

            else
                nameText.PivotOrigin = RectanglePoint.RightTop;

            // Icon
            this.icon = new Sprite()
            {
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.UIElement.Small
            };
            this.icon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, .5f, 1000, -1);

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
            menu.Game.SpriteBatch.Begin(menu.Game.Camera, SamplerState.LinearClamp);
            nameText.Draw(gameTime);
            menu.Game.SpriteBatch.End();

            if (!icon.IsEmpty)
            {
                menu.Game.SpriteBatch.Begin(menu.Game.Camera);
                icon.Draw(gameTime);
                menu.Game.SpriteBatch.End();
            }
        }

        // Execute
        public void Execute()
        {
            action?.Invoke();
        }

        // IconImage
        public AtlasImage? IconImage
        {
            get => icon.RenderImage;
            set => icon.RenderImage = value;
        }

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

                icon.Position = nameText.BoundingBox.GetPoint(RectanglePoint.Left, -1, -.5f);
            }
        }

        // TextBoundingBox
        public RectangleF TextBoundingBox => nameText.BoundingBox;

        // ToString
        public override string ToString()
        {
            return nameText.ToString();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            UpdateColor();
            icon.Update(gameTime);
        }
    }
}
