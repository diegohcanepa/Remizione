using Engendro;

namespace Remizione
{
    /// <summary>
    /// RainEmitter
    /// </summary>
    public sealed class RainEmitter : ParticleEmitter
    {
        // Constructor
        public RainEmitter(GameSession session, Rain weather)
            : base(session.Game, new RainParticleState(weather), new RainEmitterType(session.Camera), 2, 70, 40)
        {
        }
    }
}
