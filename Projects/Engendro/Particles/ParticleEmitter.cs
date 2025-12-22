using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// ParticleEmitter
    /// </summary>
    public class ParticleEmitter : GameObject
    {
        #region Private fields

        private readonly LinkedList<Particle> activeParticles = new();
        private readonly LinkedList<Particle> inactiveParticles = new();
        private readonly IEmitterType emitterType;
        private readonly int maxParticles;
        private int particlesEmitCooldown;
        private readonly ParticleState particleState;

        #endregion

        #region Constructor

        // Constructor
        public ParticleEmitter(EngendroGame game, ParticleState particleState, IEmitterType emitterType, int burstAmount, int burstInterval, int maxParticles)
            : base(game)
        {
            this.BurstInterval = burstInterval;
            this.BurstAmount = burstAmount;
            this.particleState = particleState;
            this.emitterType = emitterType;
            this.maxParticles = maxParticles;
        }

        #endregion

        #region Private members

        // EmitNewParticle
        private void EmitNewParticle(Particle particle)
        {
            particle.Activate(particleState, Position, emitterType);
            activeParticles.AddLast(particle);
        }

        // EmitParticles
        private void EmitParticles()
        {
            // Make sure we're not at max particles
            if (activeParticles.Count >= maxParticles)
            {
                return;
            }

            var maxAmountThatCanBeCreated = maxParticles - activeParticles.Count;
            var neededParticles = Math.Min(maxAmountThatCanBeCreated, BurstAmount);

            // Reuse inactive particles first before creating new ones
            var nbToReuse = Math.Min(inactiveParticles.Count, neededParticles);
            var nbToCreate = neededParticles - nbToReuse;

            for (var i = 0; i < nbToReuse; i++)
            {
                var particleNode = inactiveParticles.First;
                if (particleNode != null)
                {
                    EmitNewParticle(particleNode.Value);
                    inactiveParticles.Remove(particleNode);
                }
            }

            for (var i = 0; i < nbToCreate; i++)
            {
                EmitNewParticle(CreateNewParticle());
            }
        }

        #endregion

        #region Protected members

        // CreateNewParticle
        protected virtual Particle CreateNewParticle()
        {
            return new(Game);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            var node = activeParticles.First;
            while (node != null)
            {
                node.Value.Draw(gameTime);
                node = node.Next;
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsActive)
                return;

            if (IsActive)
            {
                particlesEmitCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (particlesEmitCooldown <= 0)
                {
                    EmitParticles();
                    particlesEmitCooldown = BurstInterval;
                }
            }

            var particleNode = activeParticles.First;
            while (particleNode != null)
            {
                var nextNode = particleNode.Next;
                particleNode.Value.Update(gameTime);
                if (!particleNode.Value.IsActive)
                {
                    activeParticles.Remove(particleNode);
                    inactiveParticles.AddLast(particleNode.Value);
                }

                particleNode = nextNode;
            }

            if (Duration > 0)
            {
                Age += gameTime.ElapsedGameTime.Milliseconds;
                if (Age >= Duration)
                {
                    Deactivate();
                }
            }
        }

        #endregion

        // Activate
        public void Activate()
        {
            if (IsActive)
                return;

            Age = Duration <= 0 ? -1 : 0;
            IsActive = true;
            particlesEmitCooldown = 0;
        }

        // Age
        public int Age { get; private set; }

        // BurstAmount
        public int BurstAmount { get; set; }

        // BurstInterval
        public int BurstInterval { get; set; }

        // Deactivate
        public void Deactivate()
        {
            IsActive = false;
        }

        // Duration
        public int Duration { get; set; }

        // IsActive
        public bool IsActive { get; private set; }

        // Position
        public Vector2 Position { get; set; }
    }
}
