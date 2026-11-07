using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// RoomEnvironment
    /// </summary>
    public sealed class RoomEnvironment
    {
        private readonly DustEmitter ashEmitter;
        private readonly FireflyEmitter fireflyEmitter;
        private readonly GameSession session;

        // Constructor
        public RoomEnvironment(GameSession session)
        {
            this.session = session;
            this.ashEmitter = new DustEmitter(session.Camera, 6, 1000, 35);
            this.fireflyEmitter = new FireflyEmitter(session, 1, 500, 20);

            ashEmitter.Activate();
            fireflyEmitter.Activate();
        }

        // DrawAshParticles
        public void DrawAshParticles(GameTime gameTime)
        {
            session.Game.SpriteBatch.Begin(session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            ashEmitter.Draw(gameTime);
            session.Game.SpriteBatch.End();
        }

        // DrawFireflyParticles
        public void DrawFireflyParticles(GameTime gameTime)
        {
            session.Game.SpriteBatch.Begin(session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            fireflyEmitter.Draw(gameTime);
            session.Game.SpriteBatch.End();
        }

        // UpdateAshParticles
        public void UpdateAshParticles(GameTime gameTime)
        {
            ashEmitter.Update(gameTime);
        }

        // UpdateFireflyParticles
        public void UpdateFireflyParticles(GameTime gameTime)
        {
            fireflyEmitter.Update(gameTime);
        }
    }
}
