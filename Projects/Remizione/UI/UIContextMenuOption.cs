using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIContextMenuOption
    /// </summary>
    public sealed class UIContextMenuOption
    {
        #region Private fields

        private readonly ImageSprite iconSprite;
        private bool isSelected;
        private readonly UIContextMenu menu;
        private Vector2 position;
        private readonly FloatTween shakeTween = new();
        private readonly TextSprite textSprite;

        #endregion

        // Constructor
        public UIContextMenuOption(UIContextMenu menu, string key, string text, AtlasImage? icon)
        {
            this.menu = menu;
            this.Key = key;
            this.iconSprite = new ImageSprite(menu.Game, icon)
            {
                Scale = ScaleInfo.ContextMenu.Icon
            };

            this.textSprite = new TextSprite(menu.Game, menu.Font)
            {
                Color = ColorPalette.ContextMenu.OptionText,
                Scale = menu.OptionTextScale,
                Text = text
            };

            Invalidate();
        }

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Draw
        public void Draw(GameTime gameTime)
        {
            iconSprite.Draw(gameTime);
            textSprite.Draw(gameTime);
        }

        // IconScale
        public Vector2 IconScale
        {
            get => iconSprite.Scale;
            set => iconSprite.Scale = value;
        }

        // Invalidate
        public void Invalidate()
        {
            iconSprite.PivotOrigin = RectanglePoint.LeftTop;
            iconSprite.Position = position;
            textSprite.Position = position;

            if (!iconSprite.IsEmpty)
                textSprite.X += iconSprite.BoundingBox.Width + 2;

            BoundingBox = RectangleF.Union(textSprite.BoundingBox, iconSprite.BoundingBox);

            iconSprite.PivotOrigin = RectanglePoint.Middle;
            iconSprite.X += iconSprite.BoundingBox.Width * .5f;
            iconSprite.Y = textSprite.BoundingBox.GetPoint(RectanglePoint.Left).Y;

            textSprite.Scale = menu.OptionTextScale;
            textSprite.Color = IsSelected ? ColorPalette.Text.Light : ColorPalette.Text.Default;
            iconSprite.Color = textSprite.Color;
        }

        // IsSelected
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value != isSelected)
                {
                    this.isSelected = value;
                    Invalidate();
                }
            }
        }

        // Key
        public string Key { get; }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                this.position = value;
                Invalidate();
            }
        }

        // Shake
        public void Shake()
        {
            shakeTween.Start(TweenStyle.Linear, textSprite.Y, textSprite.Y + .5f, 40, 4);
            textSprite.Tweens.YTween = shakeTween;
        }

        // ToString
        public override string ToString() => textSprite.ToString();

        // Update
        public void Update(GameTime gameTime)
        {
            iconSprite.Update(gameTime);
            textSprite.Update(gameTime);
        }
    }
}
