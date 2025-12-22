using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// DustEmitter
    /// </summary>
    public sealed class DustEmitter : ParticleEmitter
    {
        #region Constructors

        // Constructor
        public DustEmitter(GameSession session, int burstAmount, int burstInterval, int maxParticles)
            : base(session.Game, new DustParticleState(session), new DustEmitterType(session.Camera), burstAmount, burstInterval, maxParticles)
        {
        }

        #endregion

        #region Protected members

        // CreateNewParticle
        protected override Particle CreateNewParticle()
        {
            return new DustParticle(Game);
        }

        #endregion

        // DefaultBurstAmount
        public const int DefaultBurstAmount = 15;

        // DefaultBurstInterval
        public const int DefaultBurstInterval = 1000;
    }
}
