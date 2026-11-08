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
        private static readonly Vector2 defaultGlobalLightScale = new(12);
        private static readonly Color defaultLightMapColor = new(45, 45, 45);
        private readonly FireflyEmitter fireflyEmitter;
        private readonly ColorTween lightMapColorTween = new();
        private readonly GameSession session;

        // Constructor
        public RoomEnvironment(GameSession session)
        {
            this.session = session;
            this.ashEmitter = new DustEmitter(session.Camera, 6, 1000, 35);
            this.fireflyEmitter = new FireflyEmitter(session, 1, 500, 20);
            this.GlobalLight = new("Light", LightKind.Global)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(12)
            };

            ashEmitter.Activate();
            fireflyEmitter.Activate();
        }

        // BeginDarkCycle
        public void BeginDarkCycle(bool immediate = false)
        {
            if (immediate)
            {
                lightMapColorTween.Stop();
                LightMapColor = Color.Black;
                GlobalLight.Unlit(true);
            }
            else
            {
                Sound.Play(SoundNames.CycleHorn);
                lightMapColorTween.Start(TweenStyle.CubicIn, LightMapColor, Color.Black, CycleTransitionDuration);
                GlobalLight.Unlit();
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
            }
            else
            {
                lightMapColorTween.Start(TweenStyle.CubicIn, LightMapColor, defaultLightMapColor, CycleTransitionDuration);
                GlobalLight.Lit();
            }
        }

        // GlobalLight
        public Light GlobalLight { get; }

        // LightMapColor
        public Color LightMapColor { get; private set; } = defaultLightMapColor;

        // Update
        public void Update(GameTime gameTime)
        {
            if (session.Room == null)
                return;

            if (lightMapColorTween.IsRunning)
            {
                lightMapColorTween.Update(gameTime);
                LightMapColor = lightMapColorTween.CurrentValue;
            }

            GlobalLight.Position = session.Camera.Position - session.Camera.Offset;
            GlobalLight.Update(gameTime);

            if (session.Room.AllowAshParticles)
                ashEmitter.Update(gameTime);

            if (session.Room.AllowFireflyParticles)
                fireflyEmitter.Update(gameTime);
        }
    }
}
