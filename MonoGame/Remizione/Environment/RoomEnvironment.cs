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
        private static readonly Vector2 defaultPlayerLightSize = new(2.5f);
        private readonly FireflyEmitter fireflyEmitter;
        private bool pendingRespawn;
        private readonly Countdown respawnCountdown = new();
        private readonly ColorTween lightMapColorTween = new();
        private const int proceduralThingsFade = 2000;
        private readonly FloatTween procThingsOpacityTween = new();

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
                Scale = defaultPlayerLightSize
            };

            PlayerLight.Unlit(true);

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
            procThingsOpacityTween.StartDelay = 0;
            procThingsOpacityTween.Start(TweenStyle.CubicIn, 0, 1, proceduralThingsFade);
            RoomLight.Unlit(true);
            //PlayerLight.Lit(true);
        }

        #endregion

        // BeginDarkCycle
        public void BeginDarkCycle()
        {
            pendingRespawn = true;
            Sound.Play(SoundNames.CycleHorn);
            respawnCountdown.Start(CycleTransitionDuration / 1000);
            procThingsOpacityTween.StartDelay = CycleTransitionDuration - proceduralThingsFade;
            procThingsOpacityTween.Start(TweenStyle.CubicIn, 1, 0, proceduralThingsFade);
            lightMapColorTween.Start(TweenStyle.CubicIn, LightMapColor, Color.Black, CycleTransitionDuration);
            RoomLight.Unlit(CycleTransitionDuration);
            //PlayerLight.Lit(CycleTransitionDuration);
            AudioManager.Music.PlayTag("Dark", 8000);
        }

        // CycleTransitionDuration
        public const int CycleTransitionDuration = 20000;

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
            RoomLight.Lit(CycleTransitionDuration);
            //PlayerLight.Unlit(CycleTransitionDuration);
            AudioManager.Music.PlayTag("Main", 8000);
        }

        // IsChangingCycle
        public bool IsChangingCycle => lightMapColorTween.IsRunning;

        // LightMapColor
        public Color LightMapColor { get; private set; } = defaultLightMapColor;

        // PlayerLight
        public Light PlayerLight { get; }

        // RespawnOpacity
        public Ratio RespawnOpacity { get; private set; } = 1;

        // RoomLight
        public Light RoomLight { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (session.Room == null)
                return;

            if (procThingsOpacityTween.IsRunning)
            {
                procThingsOpacityTween.Update(gameTime);
                RespawnOpacity = procThingsOpacityTween.CurrentValue;
            }

            if (pendingRespawn && respawnCountdown.IsRunning)
            {
                respawnCountdown.Update(gameTime);
                if (!respawnCountdown.IsRunning)
                {
                    pendingRespawn = false;
                    Respawn();
                }
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
