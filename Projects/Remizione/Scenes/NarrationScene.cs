using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Remizione.Scripting;
using System;

namespace Remizione
{
    /// <summary>
    /// NarrationScene
    /// </summary>
    public sealed class NarrationScene : Scene
    {
        #region Private fields

        private readonly ContentManager content;
        private readonly Sprite background = new(Atlases.UI.EchoBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly FloatTween opacityTween = new();
        private readonly GameSession session;
        private SoundEffect? soundEffect;
        private SoundEffectInstance? soundEffectInstance;
        private readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        public NarrationScene(GameSession session)
            : base()
        {
            this.session = session;
            this.PausePreviousScenes = false;
            this.content = new(Game.Services, Game.Content.RootDirectory);

            // Text sprite
            this.textSprite = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Yellow * .8f,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PivotOrigin = RectanglePoint.Top,
                Position = background.BoundingBox.GetPoint(RectanglePoint.Top, 0, 20),
                Scale = ScaleInfo.Text.ExtraLarge
            };
        }

        #endregion

        #region Private members

        // DisposeSound
        private void DisposeSound()
        {
            if (soundEffectInstance != null)
            {
                soundEffectInstance.Stop();
                soundEffectInstance.Dispose();
                soundEffectInstance = null;
            }

            soundEffect?.Dispose();
            soundEffect = null;
        }

        // LoadVoiceSoundEffect
        private SoundEffect? LoadVoiceSoundEffect(string soundName, string languageTag)
        {
            if (string.IsNullOrWhiteSpace(soundName) || string.IsNullOrWhiteSpace(languageTag))
                return null;

            var filePath = Sound.EncodeAssetName(AudioManager.VoiceCategory, languageTag, soundName);

            try
            {
                return content.Load<SoundEffect>(filePath);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            MouseCursor.Icon = MouseCursorIcon.Wait;
            MouseCursor.Tooltip = null;
            MouseCursor.CustomImage = null;
        }

        // OnDeactivate
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            soundEffectInstance?.Stop();
            soundEffectInstance = null;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            background.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            textSprite.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);

            if (soundEffectInstance?.State == SoundState.Stopped)
            {
                CanClose = true;

                DisposeSound();

                Action? action = null;
                if (session.AwaitingScript?.NextStatement is not NarrateCommand and not AwaitCommand)
                    action = () => Game.SceneManager.Pop();

                opacityTween.Start(TweenStyle.CubicIn, 1, 0, 500, action);
                textSprite.Tweens.OpacityTween = opacityTween;
            }
        }

        #endregion

        // CanClose
        public bool CanClose { get; private set; }

        // Show
        public void Show(string text, string? soundName)
        {
            this.textSprite.Text = text;
            if (!string.IsNullOrWhiteSpace(soundName))
            {
                this.textSprite.Color = ColorPalette.Text.Yellow * .8f;
                MouseCursor.Icon = MouseCursorIcon.Wait;
            }

            DisposeSound();

            CanClose = false;

            SoundEffect? soundEffect = null;
            if (!string.IsNullOrWhiteSpace(soundName))
            {
                if (TextRepository.LanguagePackage?.LanguageTag is string languageTag)
                    soundEffect = LoadVoiceSoundEffect(soundName, languageTag);

                soundEffect ??= LoadVoiceSoundEffect(soundName, "en-US");
            }

            if (!string.IsNullOrWhiteSpace(soundName))
            {
                if (soundEffect != null)
                {
                    soundEffectInstance = soundEffect.CreateInstance();
                    soundEffectInstance.Volume = AudioManager.VoiceCategory.Volume.Effective;
                    soundEffectInstance.Pitch = .1f;
                    soundEffectInstance.Play();
                }
            }

            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = opacityTween;
        }
    }
}