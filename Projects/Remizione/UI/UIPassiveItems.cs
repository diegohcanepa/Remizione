using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIPassiveItems
    /// </summary>
    public sealed class UIPassiveItems : GameObject
    {
        private readonly Sprite[] icons;
        //private int lastSeenInventoryVersion = -1;

        private readonly GameSession session;

        #region Constructor

        // Constructor
        public UIPassiveItems(GameSession session)
            : base()
        {
            this.session = session;

            this.icons = new Sprite[ItemContainer.MaximumCapacity];

            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new()
                {
                    PivotOrigin = RectanglePoint.RightBottom,
                    Scale = ScaleInfo.UIElement.Medium
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
                icons[i].RenderImage = null;
            }

            // TODO: Check

            /*
            var index = 0;
            var iconPos = Screen.Area.GetPoint(RectanglePoint.RightBottom, -30, -5);
            for (var i = session.PlayerInventory.Count - 1; i >= 0; i--)
            {
                if (session.PlayerInventory[i].Definition.IsPassive)
                {
                    icons[index].RenderImage = session.PlayerInventory[i].Definition.Image;
                    icons[index].Position = iconPos;
                    icons[index].Scale = ScaleInfo.UIElement.Small;
                    iconPos.X -= icons[index].Width;
                    index++;
                }
            }
            */
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < icons.Length; i++)
            {
                if (icons[i].RenderImage == null)
                    break;
                else
                    icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            // TODO: Check

            /*
            if (lastSeenInventoryVersion != session.PlayerInventory.ContentVersion)
            {
                lastSeenInventoryVersion = session.PlayerInventory.ContentVersion;
                Refresh();
            }
            */
        }

        #endregion
    }
}
