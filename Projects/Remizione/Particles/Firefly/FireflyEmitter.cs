using Engendro;

namespace Remizione
{
    /// <summary>
    /// FireflyEmitter
    /// </summary>
    public sealed class FireflyEmitter : ParticleEmitter
    {
        #region Constructors

        // Constructor
        public FireflyEmitter(GameSession session, int burstAmount, int burstInterval, int maxParticles)
            : base(session.Game, new FireflyParticleState(), new FireflyEmitterType(session.Camera), burstAmount, burstInterval, maxParticles)
        {
        }

        #endregion

        // CreateNewParticle
        protected override Particle CreateNewParticle() => new FireflyParticle(Game);
    }
}
