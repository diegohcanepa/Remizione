using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // DebrisPieces
        public ObjectPool<DebrisPiece> DebrisPieces { get; } = new ObjectPool<DebrisPiece>(() => new DebrisPiece(), 100);

        // FloatingTexts
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // Projectiles
        public ObjectPool<Projectile> Projectiles { get; } = new ObjectPool<Projectile>(() => new Projectile(session), 30);

        // Sacks
        public ObjectPool<Sack> Sacks { get; } = new ObjectPool<Sack>(() => new Sack(session, string.Empty), 30);
    }
}
