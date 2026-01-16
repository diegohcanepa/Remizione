using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // Coins
        public ObjectPool<Coin> Coins { get; } = new ObjectPool<Coin>(() => new Coin(session), 30);

        // FloatingHearts
        public ObjectPool<FloatingHeart> FloatingHearts { get; } = new ObjectPool<FloatingHeart>(() => new FloatingHeart(session), 30);

        // FloatingTexts
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);
    }
}
