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
        #region Private members

        private readonly DustEmitter ashEmitter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public RoomEnvironment(GameSession session)
        {
            this.session = session;
            this.ashEmitter = new DustEmitter(session.Camera, 6, 1000, 35);
            ashEmitter.Activate();

            LightMask = new()
            {
                Color = new Color(235, 170, 170),
                RenderImage = Atlases.Environment.DefaultLight,
                PivotOrigin = RectanglePoint.Center,
                Scale = new(20, 10),
                Opacity = session.CycleManager.LightOpacity
            };
        }

        #endregion

        // DrawAshParticles
        public void DrawAshParticles(GameTime gameTime)
        {
            session.Game.SpriteBatch.Begin(session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            ashEmitter.Draw(gameTime);
            session.Game.SpriteBatch.End();
        }

        // LightMapColor
        public Color LightMapColor => session.CycleManager.LightMapColor;

        // LightMask
        public Sprite LightMask { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (session.Room == null)
                return;

            LightMask.Opacity = session.CycleManager.LightOpacity;
            LightMask.Position = session.Camera.Position;
            LightMask.Update(gameTime);

            if (session.Room.AllowAshParticles)
                ashEmitter.Update(gameTime);


        }
    }
}