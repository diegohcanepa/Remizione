using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIPassiveItemMeter
    /// </summary>
    public sealed class UIPassiveItemMeter : GameObject
    {
        private readonly ImageSprite[] icons;
        private int lastSeenInventoryVersion = -1;

        private readonly GameSession session;

        #region Constructor

        // Constructor
        public UIPassiveItemMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.icons = new ImageSprite[Inventory.MaximumCapacity];

            var pos = Screen.Area.GetPoint(RectanglePoint.LeftTop, 5, 10);
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new(Game)
                {
                    PivotOrigin = RectanglePoint.LeftTop,
                    Scale = ScaleInfo.UIElement.Tiny
                };
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            // Clean up images
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i].Image = null;
            }

            var index = 0;
            var iconPos = Screen.Area.GetPoint(RectanglePoint.LeftTop, 5, 11);
            for (var i = 0; i < session.Inventory.Count; i++)
            {
                if (session.Inventory[i].Definition.IsPassive)
                {
                    icons[index].Image = session.Inventory[i].Definition.Image;
                    icons[index].Position = iconPos;
                    iconPos.X += icons[index].Width;
                    index++;
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < icons.Length; i++)
            {
                if (icons[i].Image == null)
                    break;
                else
                    icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastSeenInventoryVersion != session.Inventory.ContentVersion)
            {
                lastSeenInventoryVersion = session.Inventory.ContentVersion;
                Refresh();
            }
        }

        #endregion
    }
}
