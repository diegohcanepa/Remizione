using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// UIPocketItems
    /// </summary>
    public sealed class UIPocketItems : GameObject
    {
        private bool isInitialized;
        private readonly ReadOnlyCollection<UIPocketItemMeter> items;

        // Constructor
        public UIPocketItems(PocketItemManager manager)
            : base()
        {
            this.Manager = manager;

            var list = new List<UIPocketItemMeter>();

            foreach (var value in Enum.GetValues<PocketItemType>())
            {
                list.Add(new(value));
            }

            items = new ReadOnlyCollection<UIPocketItemMeter>(list);

            Refresh();
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Value > 0)
                    items[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                Refresh();
            }

            for (var i = 0; i < items.Count; i++)
            {
                items[i].Update(gameTime);
            }
        }

        #endregion

        // Manager
        public PocketItemManager Manager { get; }

        // Refresh
        public void Refresh()
        {
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -26, 0);
            for (var i = 0; i < items.Count; i++)
            {
                var count = Manager.GetCount(items[i].PocketItemType);
                items[i].Value = count;
                if (count > 0)
                {
                    items[i].Position = pos;
                    pos.X -= items[i].BoundingBox.Width + 4;
                }
            }
        }
    }
}
