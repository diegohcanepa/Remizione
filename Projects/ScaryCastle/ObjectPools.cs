using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // FlyOffs
        public ObjectPool<FlyOff> FlyOffs { get; } = new ObjectPool<FlyOff>(() => new FlyOff(session), 30);

        // Projectiles
        public ObjectPool<Projectile> Projectiles { get; } = new ObjectPool<Projectile>(() => new Projectile(session), 30);

        // RemainsPieces
        public ObjectPool<RemainsPiece> RemainsPieces { get; } = new ObjectPool<RemainsPiece>(() => new RemainsPiece(), 100);
    }
}
