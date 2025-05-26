using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione
{
    /// <summary>
    /// Weather
    /// </summary>
    public sealed class Weather
    {
        #region Private fields

        private readonly RainDropImpactEmitter rainDropImpacts;
        private readonly RainEmitter rainEmitter;
        private const int RainEaseInOut = 10000;
        private readonly FloatTween rainIntensityTween = new();
        private readonly SoundInstance rainSound;
        private readonly GameSession session;
        private readonly Countdown thunderCooldown = new() { TimeRange = new Int32Range(5000, 10000) };

        #endregion

        #region Constructor

        // Constructor
        public Weather(GameSession session)
        {
            this.session = session;

            rainDropImpacts = new RainDropImpactEmitter(session, RainDropImpactKind.Ground);

            rainEmitter = new RainEmitter(session, this);
            rainEmitter.Activate();
            rainSound = GetSoundInstance(SoundNames.Rain);
        }

        #endregion

        #region Private members

        // GetSoundInstance
        private static SoundInstance GetSoundInstance(string soundName)
        {
            var result = Sound.Find(soundName)?.PopInstance() ?? throw new InvalidOperationException("Cannot instantiate sound.");
            result.AllowReuse = false;
            result.IsLooped = true;
            return result;
        }

        // IsRainAllowedInRoom
        private bool IsRainAllowedInRoom => session.Room is GameRoom room && room.IsOutdoor;

        // Prepare
        private void Prepare()
        {
            var room = session.Room;
            if (room == null)
                return;

            rainEmitter.Activate();

            if (rainSound.IsPlaying)
                rainSound.Volume.FadeIn(250);
            else
                rainSound.Play();
        }

        #endregion

        #region Internal members

        // EnterRoom
        internal void EnterRoom()
        {
            if (!IsRainAllowedInRoom || !IsRaining || IsRainEnding)
                rainSound.Stop(0);
            else
                Prepare();
        }

        #endregion

        // BeginRain
        public void BeginRain(int duration, bool immediate)
        {
            thunderCooldown.StartFromRange();

            if (IsRaining || !IsRainAllowedInRoom)
            {
                RainRemainingTime = duration;
                return;
            }

            rainEmitter.Activate();
            rainIntensityTween.Stop();

            if (!immediate)
                rainIntensityTween.Start(TweenStyle.CubicIn, 0, 1, RainEaseInOut);

            Prepare();

            RainRemainingTime = duration;
        }

        // CanDrawRainDropImpacts
        public bool CanDrawRainDropImpacts => IsRaining && IsRainAllowedInRoom && Intensity > .7f;

        // DrawRain
        public void DrawRain(GameTime gameTime)
        {
            if (IsRaining && IsRainAllowedInRoom)
            {
                session.Game.SpriteBatch.Begin(session.Camera, SamplerState.PointClamp);
                rainEmitter.Draw(gameTime);
                session.Game.SpriteBatch.End();
            }
        }

        // DrawRainDropImpacts
        public void DrawRainDropImpacts(GameTime gameTime)
        {
            if (CanDrawRainDropImpacts)
            {
                session.Game.SpriteBatch.Begin(session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
                rainDropImpacts.Draw(gameTime);
                session.Game.SpriteBatch.End();
            }
        }

        // EndRain
        public void EndRain(bool immediate)
        {
            if (immediate)
            {
                RainRemainingTime = 0;
                rainSound?.Stop(0);
                rainIntensityTween.Stop();
                rainEmitter.Deactivate();
            }
            else
            {
                RainRemainingTime = RainEaseInOut;
            }
        }

        // Intensity
        public float Intensity
        {
            get
            {
                if (rainIntensityTween.IsRunning)
                    return rainIntensityTween.CurrentValue;
                else if (IsRaining)
                    return RainRemainingTime > RainEaseInOut ? 1 : (float)RainRemainingTime / RainEaseInOut;
                else
                    return 0;
            }
        }

        // IsRainEnding
        public bool IsRainEnding => rainIntensityTween.IsRunning && rainIntensityTween.EndValue == 0;

        // IsRaining
        public bool IsRaining => RainRemainingTime > 0;

        // RainRemainingTime
        public int RainRemainingTime { get; private set; }

        // Thunder
        public void Thunder(bool force, bool extendedDuration)
        {
            if (session.Room is not GameRoom room)
                return;

            if (RainRemainingTime > 15000 || force)
            {
                room.ShowLightning(extendedDuration);

                var soundName = SoundNames.Thunder.ToString();

                if (Sound.Find(soundName)?.PopInstance() is SoundInstance sound)
                {
                    sound.TransitionAware = false;

                    if (force)
                        sound.Volume.Current = 1;

                    if (sound.Volume.Current == 1)
                        InputManager.Players[0].GamePad.Vibrate(400, .4f, .4f);

                    sound.Play();
                }

                thunderCooldown.StartFromRange();
            }
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (RainRemainingTime > 0)
                RainRemainingTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (IsRaining && IsRainAllowedInRoom)
            {
                if (session.Room is GameRoom room && room.IsOutdoor)
                {
                    rainDropImpacts.Update(gameTime);
                    rainEmitter.Update(gameTime);
                }

                if (thunderCooldown.TimeLeft > 0)
                {
                    thunderCooldown.Update(gameTime);
                    if (!thunderCooldown.IsRunning)
                        Thunder(false, false);
                }
            }

            rainIntensityTween.Update(gameTime);
        }
    }
}
