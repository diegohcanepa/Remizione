using Engendro;

namespace Remizione
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // FloatingText
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // LootBags
        public ObjectPool<LootBag> LootBags { get; } = new ObjectPool<LootBag>(() => new LootBag(session), 20);
    }
}
