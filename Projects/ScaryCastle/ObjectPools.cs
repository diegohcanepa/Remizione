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

        // Projectiles
        public ObjectPool<Projectile> Projectiles { get; } = new ObjectPool<Projectile>(() => new Projectile(session), 30);

        // RemainsPieces
        public ObjectPool<RemainsPiece> RemainsPieces { get; } = new ObjectPool<RemainsPiece>(() => new RemainsPiece(), 100);
    }
}
