using Engendro;

namespace Remizione
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // FlyOffs
        public ObjectPool<FlyOff> FlyOffs { get; } = new(() => new FlyOff(session), 30);

        // Projectiles
        public ObjectPool<Projectile> Projectiles { get; } = new(() => new Projectile(session), 30);

        // RemainsPieces
        public ObjectPool<RemainsPiece> RemainsPieces { get; } = new(() => new RemainsPiece(), 100);
    }
}
