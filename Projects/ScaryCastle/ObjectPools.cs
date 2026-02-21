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

        // FindThrownObject
        public ThrownObject? FindThrownObject(string itemName)
        {
            if (itemName == "Duck")
                return Ducks.Get();

            return null;
        }

        // FloatingTexts
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // ReturnThrownObject
        public void ReturnThrownObject(ThrownObject obj)
        {
            if (obj is ThrownDuck duck)
                Ducks.Return(duck);
        }

        // Sacks
        public ObjectPool<Sack> Sacks { get; } = new ObjectPool<Sack>(() => new Sack(session, string.Empty), 30);
    }
}
