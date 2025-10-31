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
            this.Owner = null;
        }

        #endregion

        // Item
        public Item? Item { get; private set; }

        // Owner
        public GameThing? Owner { get; private set; }

        // Place
        public void Place(GameThing owner, Item item, Vector2 position)
        {
            this.Owner = owner;
            item.Use(owner);
            this.Item = item;
            this.Position = position;

            OnPlaced();
        }
    }
}