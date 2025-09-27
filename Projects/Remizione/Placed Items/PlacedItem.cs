using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// PlacedItem
    /// </summary>
    public abstract class PlacedItem : IsometricProp
    {
        // Constructor
        protected PlacedItem(GameSession session)
            : base(session, string.Empty)
        {
        }

        #region Protected members

        // OnPlaced
        protected virtual void OnPlaced()
        {
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
        }

        #endregion

        // Item
        public Item? Item { get; private set; }

        // Place
        public void Place(Item item, Vector2 position)
        {
            item.Use();
            this.Item = item;
            this.Position = position;

            OnPlaced();
        }
    }
}