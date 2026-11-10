using Engendro;
using Engendro.Audio;
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
        private static readonly Color defaultLightMapColor = new(45, 45, 45);
        private readonly FireflyEmitter fireflyEmitter;
        private readonly FloatTween globalOpacityTween = new();
        private readonly ColorTween lightMapColorTween = new();
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public RoomEnvironment(GameSession session)
        {
            this.session = session;
            this.ashEmitter = new DustEmitter(session.Camera, 6, 1000, 35);
            this.fireflyEmitter = new FireflyEmitter(session, 1, 500, 20);

            this.RoomLight = new("RoomLight", LightKind.Global)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(20,10)
            };

            this.PlayerLight = new("PlayerLight", LightKind.Player)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center,
                Scale = new(4, 3)
            };

            ashEmitter.Activate();
            fireflyEmitter.Activate();
        }

        #endregion

        #region Private members

        // Respawn
        private void Respawn()
        {
            if (session.Room is ProceduralRoom room)
                room.Respawn();
            globalOpacityTween.Start(TweenStyle.CubicIn, Light.GlobalOpacity, 1, 1000);
            RoomLight.Unlit(true);
            PlayerLight.Lit(true);
        }

        #endregion

        // BeginDarkCycle
        public void BeginDarkCycle()
        {
            Sound.Play(SoundNames.CycleDark);
            globalOpacityTween.Start(TweenStyle.CubicIn, Light.GlobalOpacity, 0, CycleTransitionDuration, Respawn);
            lightMapColorTween.Start(TweenStyle.CubicIn, LightMapColor, Color.Black, CycleTransitionDuration);
            AudioManager.Music.PlayTag("Dark", 8000);
        }

        // CycleTransitionDuration
        public const int CycleTransitionDuration = 4000;

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

        // EndDarkCycle
        public void EndDarkCycle()
        {
            lightMapColorTween.Start(TweenStyle.CubicIn, LightMapColor, defaultLightMapColor, CycleTransitionDuration);
            RoomLight.Lit();
            PlayerLight.Unlit();
            AudioManager.Music.PlayTag("Main", 8000);
        }

        // IsChangingCycle
        public bool IsChangingCycle => lightMapColorTween.IsRunning;

        // LightMapColor
        public Color LightMapColor { get; private set; } = defaultLightMapColor;

        // PlayerLight
        public Light PlayerLight { get; }

        // RoomLight
        public Light RoomLight { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (session.Room == null)
                return;

            if (globalOpacityTween.IsRunning)
            {
                globalOpacityTween.Update(gameTime);
                Light.GlobalOpacity = globalOpacityTween.CurrentValue;
            }

            if (lightMapColorTween.IsRunning)
            {
                lightMapColorTween.Update(gameTime);
                LightMapColor = lightMapColorTween.CurrentValue;
            }

            RoomLight.Position = session.Camera.Position - session.Camera.Offset;
            RoomLight.Update(gameTime);

            if (session.Player != null)
                PlayerLight.Position = session.Player.BoundingBox.Center;

            PlayerLight.Update(gameTime);

            if (session.Room.AllowAshParticles)
                ashEmitter.Update(gameTime);

            if (session.Room.AllowFireflyParticles)
                fireflyEmitter.Update(gameTime);
        }
    }
}
