using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione
{
    /// <summary>
    /// Rain
    /// </summary>
    public sealed class Rain : GameObject
    {
        #region Private fields

        private readonly RainDropImpactEmitter rainDropImpacts;
        private readonly RainEmitter rainEmitter;
        private const int RainEaseInOut = 10000;
        private readonly FloatTween rainIntensityTween = new() { StartDelay = 10000 };
        private readonly SoundInstance rainSound;
        private readonly GameSession session;
        private readonly Countdown thunderCooldown = new() { TimeRange = new Int32Range(40000, 90000) };

        #endregion

        #region Constructor

        // Constructor
        public Rain(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            rainDropImpacts = new RainDropImpactEmitter(session, RainDropImpactKind.Ground);
            rainEmitter = new RainEmitter(session, this);
            rainEmitter.Activate();
            rainSound = Sound.Find(SoundNames.Rain)?.PopInstance() ?? throw new InvalidOperationException("Cannot instantiate sound.");
            rainSound.AllowReuse = false;
            rainSound.IsLooped = true;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsRaining && IsVisibleInRoom)
            {
                session.Game.SpriteBatch.Begin(session.Camera, SamplerState.PointClamp);
                rainEmitter.Draw(gameTime);
                session.Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (RemainingTime > 0)
                RemainingTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (IsRaining && IsVisibleInRoom)
            {
                rainDropImpacts.Update(gameTime);
                rainEmitter.Update(gameTime);

                if (thunderCooldown.TimeLeft > 0)
                {
                    thunderCooldown.Update(gameTime);
                    if (!thunderCooldown.IsRunning)
                        Thunder(false, false);
                }
            }

            rainIntensityTween.Update(gameTime);
        }

        #endregion

        #region Internal members

        // EnterRoom
        internal void EnterRoom()
        {
            if (!IsVisibleInRoom || !IsRaining || IsEnding)
                rainSound.Stop(0);
            else
                rainSound.Play();
        }

        #endregion

        // Begin
        public void Begin(int duration, bool immediate)
        {
            thunderCooldown.Start(2000);

            // Renew time if is already raining or rain isn't visible in current room
            if (IsRaining || !IsVisibleInRoom)
            {
                RemainingTime = duration;
                return;
            }

            rainIntensityTween.Stop();
            if (immediate)
            {
                rainSound.Play();
            }
            else
            {
                rainIntensityTween.Start(TweenStyle.CubicIn, 0, 1, RainEaseInOut);
                rainSound.Play(RainEaseInOut);
            }

            RemainingTime = duration;
        }

        // CanDrawImpacts
        public bool CanDrawImpacts => IsRaining && IsVisibleInRoom && Intensity > .7f;

        // DrawImpacts
        public void DrawImpacts(GameTime gameTime)
        {
            if (CanDrawImpacts)
            {
                session.Game.SpriteBatch.Begin(session.Camera);
                rainDropImpacts.Draw(gameTime);
                session.Game.SpriteBatch.End();
            }
        }

        // End
        public void End(bool immediate)
        {
            if (immediate)
            {
                RemainingTime = 0;
                rainSound?.Stop(0);
                rainIntensityTween.Stop();
                rainEmitter.Deactivate();
            }
            else
            {
                RemainingTime = RainEaseInOut;
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
                    return RemainingTime > RainEaseInOut ? 1 : (float)RemainingTime / RainEaseInOut;
                else
                    return 0;
            }
        }

        // IsEnding
        public bool IsEnding => rainIntensityTween.IsRunning && rainIntensityTween.EndValue == 0;

        // IsRaining
        public bool IsRaining => RemainingTime > 0;

        // IsVisibleInRoom
        private bool IsVisibleInRoom => session.Room is GameRoom room && room.IsOutdoor;

        // RemainingTime
        public int RemainingTime { get; private set; }

        // Thunder
        public void Thunder(bool force, bool extendedDuration)
        {
            if (RemainingTime > 15000 || force)
            {
                session.Room?.ShowLightning(extendedDuration);

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
    }
}
