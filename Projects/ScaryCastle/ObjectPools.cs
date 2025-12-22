using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // Ducks
        public ObjectPool<ThrownDuck> Ducks { get; } = new ObjectPool<ThrownDuck>(() => new ThrownDuck(session), 50);

        // Firecrackers
        public ObjectPool<Firecracker> Firecrackers { get; } = new ObjectPool<Firecracker>(() => new Firecracker(session), 20);

        // FloatingHearts
        public ObjectPool<FloatingHeart> FloatingHearts { get; } = new ObjectPool<FloatingHeart>(() => new FloatingHeart(session), 30);

        // FloatingTexts
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // FindPlacedItem
        public PlacedItem? FindPlacedItem(string itemName)
        {
            if (itemName == "Firecracker")
                return Firecrackers.Get();

            return null;
        }

        // FindThrownItem
        public ThrownItem? FindThrownItem(string itemName)
        {
            if (itemName == "Duck")
                return Ducks.Get();

            return null;
        }

        // Pickups
        public ObjectPool<Pickup> Pickups { get; } = new ObjectPool<Pickup>(() => new Pickup(session, string.Empty), 30);

        // ReturnThrownItem
        public void ReturnThrownItem(ThrownItem item)
        {
            if (item is ThrownDuck duck)
                Ducks.Return(duck);
        }

        // Tickets
        public ObjectPool<Ticket> Tickets { get; } = new ObjectPool<Ticket>(() => new Ticket(session), 30);
    }
}
