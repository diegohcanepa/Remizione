using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Remizione.UI
{
    /// <summary>
    /// Narrator
    /// </summary>
    public sealed class Narrator : GameObject
    {
        private readonly ContentManager content;
        private int textFadeTimer = -1;
        private SoundEffect? soundEffect;
        private SoundEffectInstance? soundEffectInstance;
        private readonly TextSprite textSprite;
        private FloatTween opacityTween = new();


        #region Constructor

        // Constructor
        public Narrator()
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

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            textSprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);

            if (soundEffectInstance != null && soundEffectInstance.State == SoundState.Stopped)
            {
                soundEffectInstance = null;
                textFadeTimer = 2000;
            }
            else if (textFadeTimer >= 0)
            {
                textFadeTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (textFadeTimer < 0)
                {
                    opacityTween.Start(TweenStyle.Linear, 1, 0, 400, () => IsPlaying = false);
                    textSprite.Tweens.OpacityTween = opacityTween;
                }
            }
        }

        #endregion

        // IsPlaying
        public bool IsPlaying { get; private set; }

        // Play
        public void Play(string text, string? soundName)
        {
            Stop();

            if (string.IsNullOrWhiteSpace(text))
                return;

            if (text == textSprite.Text)
                return;

            opacityTween.Start(TweenStyle.Linear, 0, 1, 400);
            this.textSprite.Text = text;
            this.textSprite.Tweens.OpacityTween = opacityTween;

            if (!string.IsNullOrWhiteSpace(soundName))
            {
                var filePath = Sound.EncodeAssetName(AudioManager.VoiceCategory, soundName);
                filePath = Path.Combine(Game.Content.RootDirectory, filePath);

                using var stream = File.OpenRead(filePath);
                this.soundEffect = SoundEffect.FromStream(stream);
                this.soundEffectInstance = soundEffect.CreateInstance();
                this.soundEffectInstance.Volume = AudioManager.VoiceCategory.Volume.Effective;
                this.soundEffectInstance.Play();
            }

            IsPlaying = true;
        }

        // Stop
        public void Stop()
        {
            textSprite.Clear();

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

            content.Unload();
        }
    }
}
