using Engendro;

namespace Remizione
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // Ducks
        public ObjectPool<ThrownDuck> Ducks { get; } = new ObjectPool<ThrownDuck>(() => new ThrownDuck(session), 50);

        // FloatingHeart
        public ObjectPool<FloatingHeart> FloatingHearts { get; } = new ObjectPool<FloatingHeart>(() => new FloatingHeart(session), 30);

        // FloatingText
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // GetThrowable
        public ThrownItem? GetThrowable(string itemName)
        {
            if (itemName == "Duck")
                return Ducks.Get();

            return null;
        }

        // Pickups
        public ObjectPool<Pickup> Pickups { get; } = new ObjectPool<Pickup>(() => new Pickup(session), 30);

        // ReturnThrowable
        public void ReturnThrowable(ThrownItem item)
        {
            if (item is ThrownDuck duck)
                Ducks.Return(duck);
        }
    }
}
