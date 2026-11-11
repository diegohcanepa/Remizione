using Microsoft.Xna.Framework;

namespace Engendro
{
    // IEmitterType
    public interface IEmitterType
    {
        Vector2 GetParticleDirection();
        Vector2 GetParticlePosition(Vector2 emitterPosition);
    }
}
