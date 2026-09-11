using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Remizione.UI
{
    /// <summary>
    /// Narrator
    /// </summary>
    public sealed class Narrator : SessionGameObject<GameSession>
    {
        private readonly ContentManager content;
        private readonly Queue<(string Text, SoundEffect? SoundEffect, SoundEffectInstance? SoundInstance)> playQueue = new();
        private Script? script;
        private SoundEffect? soundEffect;
        private SoundEffectInstance? soundEffectInstance;
        private int textFadeTimer = -1;
        private readonly TextSprite textSprite;
        private readonly FloatTween opacityTween = new();


        #region Constructor

        // Constructor
        public Narrator(GameSession session)
            : base(session)
        {
            content = new(Game.Services, Game.Content.RootDirectory);

            this.textSprite = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                MaximumWidth = 140,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -8),
                Scale = ScaleInfo.Text.Medium
            };
        }

        #endregion

        #region Private members

        // DisposeActiveSound
        private void DisposeActiveSound()
        {
            if (soundEffectInstance != null)
            {
                if (soundEffectInstance.State == SoundState.Playing)
                    soundEffectInstance.Stop();

                soundEffectInstance.Dispose();
                soundEffectInstance = null;
            }

            if (soundEffect != null)
            {
                soundEffect.Dispose();
                soundEffect = null;
            }
        }

        // Next
        private void Next()
        {
            DisposeActiveSound();

            textFadeTimer = -1;

            if (playQueue.Count == 0)
            {
                Stop();
                return;
            }

            var item = playQueue.Dequeue();

            opacityTween.Start(TweenStyle.Linear, 0, 1, 400);
            this.textSprite.Text = item.Text;
            this.textSprite.Tweens.OpacityTween = opacityTween;

            item.SoundInstance?.Play();
            soundEffect = item.SoundEffect;
            soundEffectInstance = item.SoundInstance;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            textSprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.AwaitingScript != script)
                script = null;

            textSprite.Update(gameTime);

            if (textFadeTimer >= 0)
            {
                textFadeTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (textFadeTimer < 0)
                {
                    opacityTween.Start(TweenStyle.Linear, 1, 0, 250, Next);
                    textSprite.Tweens.OpacityTween = opacityTween;
                }
            }
            else if (soundEffectInstance != null && soundEffectInstance.State == SoundState.Stopped)
            {
                textFadeTimer = 500;
            }
        }

        #endregion

        // IsPlaying
        public bool IsPlaying { get; private set; }

        // Play
        public void Play(string text, string? soundName)
        {
            if (Session.AwaitingScript != script)
            {
                Stop();
                script = Session.AwaitingScript;
            }

            if (string.IsNullOrWhiteSpace(text))
                return;

            if (text == textSprite.Text)
                return;

            if (!string.IsNullOrWhiteSpace(soundName))
            {
                var filePath = Sound.EncodeAssetName(AudioManager.VoiceCategory, soundName);
                content.Load<SoundEffect>(filePath);
                var sfx = content.Load<SoundEffect>(filePath);
                var sfxInstance = sfx.CreateInstance();
                sfxInstance.Volume = AudioManager.VoiceCategory.Volume.Effective * .5f;
                playQueue.Enqueue(new(text, sfx, sfxInstance));
            }

            if (soundEffectInstance == null)
            {
                Next();
                IsPlaying = true;
            }
        }

        // Stop
        public void Stop()
        {
            textSprite.Clear();

            while (playQueue.Count > 0)
            {
                var item = playQueue.Dequeue();
                item.SoundInstance?.Stop();
                item.SoundInstance?.Dispose();
                item.SoundEffect?.Dispose();
            }

            DisposeActiveSound();
            content.Unload();
        }
    }
}
