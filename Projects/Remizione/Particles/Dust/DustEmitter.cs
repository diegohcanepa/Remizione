using Engendro;

namespace Remizione
{
    /// <summary>
    /// DustEmitter
    /// </summary>
    public sealed class DustEmitter : ParticleEmitter
    {
        #region Constructors

        // Constructor
        public DustEmitter(Camera camera, int burstAmount, int burstInterval, int maxParticles)
            : base(new DustParticleState(), new DustEmitterType(camera), burstAmount, burstInterval, maxParticles)
        {
        }

        #endregion

        #region Protected members

        // CreateNewParticle
        protected override Particle CreateNewParticle()
        {
            return new DustParticle();
        }

        #endregion

        // DefaultBurstAmount
        public const int DefaultBurstAmount = 35;

        // DefaultBurstInterval
        public const int DefaultBurstInterval = 1000;
    }
}
