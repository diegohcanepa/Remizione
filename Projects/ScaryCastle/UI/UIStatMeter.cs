using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIStatMeter
    /// </summary>
    public abstract class UIStatMeter : GameObject
    {
        #region Private fields

        private int fullIcons;
        private bool hasHalfIcon;
        private readonly Sprite[] icons;
        private readonly AtlasImage[] images = new AtlasImage[3];
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private int totalIcons;

        #endregion

        #region Constructor

        // Constructor
        protected UIStatMeter(Vector2 margin, AtlasImage emptyImage, AtlasImage halfImage, AtlasImage fullImage)
        {
            images[0] = emptyImage;
            images[1] = halfImage;
            images[2] = fullImage;

            this.icons = new Sprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, margin);

            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new(images[2])
                {
                    Position = pos
                };

                pos.X += icons[i].BoundingBox.Width + .5f;
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Actor == null)
                return;

            var statValue = GetStatValue();
            var statMaxValue = GetStatMaxValue();

            fullIcons = statValue / 2;
            hasHalfIcon = statValue % 2 == 1;
            totalIcons = statMaxValue / 2;

            for (int i = 0; i < totalIcons; i++)
            {
                if (i < fullIcons)
                    icons[i].RenderImage = images[2];

                else if (i == fullIcons && hasHalfIcon)
                    icons[i].RenderImage = images[1];

                else
                    icons[i].RenderImage = images[0];
            }

            lastKnownValue = statValue;
            lastKnownMaxValue = statMaxValue;
        }

        #endregion

        #region Protected members

        // GetStatMaxValue
        protected abstract int GetStatMaxValue();

        // GetStatValue
        protected abstract int GetStatValue();

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < totalIcons; i++)
            {
                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor != null)
            {
                if (lastKnownValue != GetStatValue() || lastKnownMaxValue != GetStatMaxValue())
                    Refresh();
            }
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field == null)
                    {
                        lastKnownValue = int.MinValue;
                        lastKnownMaxValue = int.MinValue;
                    }

                    Refresh();
                }
            }
        }
    }
}
