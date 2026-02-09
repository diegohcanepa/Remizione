using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // FloatingTexts
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // Sacks
        public ObjectPool<Sack> Sacks { get; } = new ObjectPool<Sack>(() => new Sack(session, string.Empty), 30);
    }
}
