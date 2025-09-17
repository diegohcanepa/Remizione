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

        // EnergyBolts
        public ObjectPool<EnergyBolt> EnergyBolts { get; } = new ObjectPool<EnergyBolt>(() => new EnergyBolt(session), 30);

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

        // Pickables
        public ObjectPool<Pickup> Pickables { get; } = new ObjectPool<Pickup>(() => new Pickup(session), 30);

        // ReturnThrowable
        public void ReturnThrowable(ThrownItem item)
        {
            if (item is ThrownDuck duck)
                Ducks.Return(duck);
        }
    }
}
