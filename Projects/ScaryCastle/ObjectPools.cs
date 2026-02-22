using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // Bibles
        public ObjectPool<Bible> Bibles { get; } = new ObjectPool<Bible>(() => new Bible(session), 50);

        // FloatingTexts
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // GetThrownObject
        public ThrownObject? GetThrownObject(ThrownObjectType objectType)
        {
            if (objectType == ThrownObjectType.Bible)
                return Bibles.Get();

            return null;
        }

        // ReturnThrownObject
        public void ReturnThrownObject(ThrownObject obj)
        {
            if (obj is Bible bible)
                Bibles.Return(bible);
        }

        // Sacks
        public ObjectPool<Sack> Sacks { get; } = new ObjectPool<Sack>(() => new Sack(session, string.Empty), 30);
    }
}
