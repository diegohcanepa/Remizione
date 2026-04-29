using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIContextMenuOption
    /// </summary>
    public sealed class UIContextMenuOption<T>
    {
        #region Private fields

        private readonly Sprite iconSprite;
        private readonly UIContextMenu<T> menu;
        private Vector2 position;
        private readonly FloatTween shakeTween = new();
        private readonly TextSprite textSprite;

        #endregion

        // Constructor
        public UIContextMenuOption(UIContextMenu<T> menu, T key, string text, AtlasImage? icon)
        {
            this.menu = menu;
            this.Key = key;
            this.iconSprite = new(icon)
            {
            };

            this.textSprite = new(menu.Font)
            {
                Scale = menu.OptionTextScale,
                Text = text
            };

            this.Index = menu.Options.Count;

            Invalidate();
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            iconSprite.PivotOrigin = RectanglePoint.LeftTop;
            iconSprite.Position = position;
            textSprite.Position = position;

            if (!iconSprite.IsEmpty)
                textSprite.X += iconSprite.BoundingBox.Width + 2;

            BoundingBox = RectangleF.Union(textSprite.BoundingBox, iconSprite.BoundingBox);

            iconSprite.PivotOrigin = RectanglePoint.Center;
            iconSprite.X += iconSprite.BoundingBox.Width * .5f;
            iconSprite.Y = textSprite.BoundingBox.GetPoint(RectanglePoint.Left).Y;
            iconSprite.Position += IconOffset;

            textSprite.Scale = menu.OptionTextScale;
            textSprite.Color = IsSelected ? menu.OptionSelectedColor : menu.OptionColor;
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Draw
        public void Draw(GameTime gameTime)
        {
            iconSprite.Draw(gameTime);
            textSprite.Draw(gameTime);
        }

        // IconOffset
        public Vector2 IconOffset { get; set; }

        // IconScale
        public Vector2 IconScale
        {
            get => iconSprite.Scale;
            set => iconSprite.Scale = value;
        }

        // Index
        public int Index { get; }

        // IsSelected
        public bool IsSelected
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

        // Key
        public T Key { get; }

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
            if (shakeTween.IsRunning)
                return;

            shakeTween.Start(TweenStyle.Linear, textSprite.Y, textSprite.Y + .5f, 40, 4);
            textSprite.Tweens.YTween = shakeTween;
        }

        // ToString
        public override string ToString()
        {
            return textSprite.ToString();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            iconSprite.Update(gameTime);
            textSprite.Update(gameTime);
        }
    }
}
