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
        private readonly DustEmitter ashEmitter;
        private static readonly Color defaultLightMapColor = new(45, 45, 45);
        private readonly FireflyEmitter fireflyEmitter;
        private readonly FloatTween globalOpacityTween = new();
        private readonly ColorTween lightMapColorTween = new();
        private readonly GameSession session;

        // Constructor
        public RoomEnvironment(GameSession session)
        {
            this.session = session;
            this.ashEmitter = new DustEmitter(session.Camera, 6, 1000, 35);
            this.fireflyEmitter = new FireflyEmitter(session, 1, 500, 20);
            this.GlobalLight = new("GlobalLight", LightKind.Global)
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

        // BeginDarkCycle
        public void BeginDarkCycle(bool immediate = false)
        {
            session.RespawnWorld();
            globalOpacityTween.Start(TweenStyle.CubicIn, 0, 1, 4000);

            if (immediate)
            {
                lightMapColorTween.Stop();
                LightMapColor = Color.Black;
                GlobalLight.Unlit(true);
                PlayerLight.Lit(true);
            }
            else
            {
                Sound.Play(SoundNames.CycleHorn);
                lightMapColorTween.Start(TweenStyle.CubicIn, LightMapColor, Color.Black, CycleTransitionDuration);
                GlobalLight.Unlit();
                PlayerLight.Lit();
                AudioManager.Music.PlayTag("Dark", 8000);
            }
        }

        // CycleTransitionDuration
        public const int CycleTransitionDuration = 10000;

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
        public void EndDarkCycle(bool immediate = false)
        {
            if (immediate)
            {
                lightMapColorTween.Stop();
                LightMapColor = defaultLightMapColor;
                GlobalLight.Lit(true);
                PlayerLight.Unlit(true);
            }
            else
            {
                lightMapColorTween.Start(TweenStyle.CubicIn, LightMapColor, defaultLightMapColor, CycleTransitionDuration);
                GlobalLight.Lit();
                PlayerLight.Unlit();
                AudioManager.Music.PlayTag("Main", 8000);
            }
        }

        // GlobalLight
        public Light GlobalLight { get; }

        // GlobalOpacity
        public Ratio GlobalOpacity { get; private set; } = 1;

        // LightMapColor
        public Color LightMapColor { get; private set; } = defaultLightMapColor;

        // PlayerLight
        public Light PlayerLight { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (session.Room == null)
                return;

            if (globalOpacityTween.IsRunning)
            {
                globalOpacityTween.Update(gameTime);
                GlobalOpacity = globalOpacityTween.CurrentValue;
            }

            if (lightMapColorTween.IsRunning)
            {
                lightMapColorTween.Update(gameTime);
                LightMapColor = lightMapColorTween.CurrentValue;
            }

            GlobalLight.Position = session.Camera.Position - session.Camera.Offset;
            GlobalLight.Update(gameTime);

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
