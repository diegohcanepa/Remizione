using EngendroAdventure;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CraftingData
    /// </summary>
    public sealed class CraftingData
    {
        private readonly Actor actor;

        // Constructor
        public CraftingData(Actor actor)
        {
            this.actor = actor;
        }

        // Invalidate
        public void Invalidate()
        {
            Position = null;
            Prop = null;
            CanPlace = false;
            EnoughAmount = false;
            EnoughFaith = false;

            Room = actor.Room as ProceduralRoom;
            if (Room == null)
                return;

            if (actor.Inventory.SelectedItem is not Item selectedItem)
                return;

            // Enough amount
            EnoughAmount = selectedItem.IsStackFull;

            // Enough faith
           // EnoughFaith = selectedItem.MetaItem.Faith == null || selectedItem.MetaItem.Faith.MaximumValue <= actor.Faith;

            // Prop
            Prop = selectedItem.MetaItem.CraftProp;
            if (Prop == null)
                return;

            var pos = actor.Position;
            var offset = Prop.BoundingBox.Width / 2 + 1;

            if (actor.Direction == FacingDirection.Right)
                pos.X += offset;
            else
                pos.X -= offset;

            pos.Y = actor.Y;

            Position = pos;

            CanPlace = Room.CanPlaceDynamicPropAt(Prop, pos);

            return;
        }

        // CanPlace
        public bool CanPlace { get; private set; }

        // EnoughAmount
        public bool EnoughAmount { get; private set; }

        // EnoughFaith
        public bool EnoughFaith { get; private set; }

        // Position
        public Vector2? Position { get; private set; }

        // Prop
        public IsometricProp? Prop { get; private set; }

        // Room
        public ProceduralRoom? Room { get; private set; }
    }
}
