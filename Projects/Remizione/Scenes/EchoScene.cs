using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Remizione.Scripting;

namespace Remizione
{
    /// <summary>
    /// EchoScene
    /// </summary>
    public sealed class EchoScene : Scene
    {
        #region Private fields

        private bool allowSkip;
        private readonly ContentManager content;
        private readonly Sprite background = new(Atlases.UI.EchoBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly FloatTween opacityTween = new();
        private readonly GameSession session;
        private SoundEffect? soundEffect;
        private SoundEffectInstance? soundEffectInstance;
        private readonly TextSprite subjectSprite;
        private readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        public EchoScene(GameSession session)
            : base()
        {
            this.session = session;
            this.PausePreviousScenes = false;
            this.content = new(Game.Services, Game.Content.RootDirectory);

            this.textSprite = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Terra,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PivotOrigin = RectanglePoint.Top,
                Position = background.BoundingBox.GetPoint(RectanglePoint.Top, 0, 20),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            this.subjectSprite = new(Fonts.Common)
            {
                Color = ColorPalette.Text.MouseCursor,
                PivotOrigin = RectanglePoint.LeftBottom,
                Scale = ScaleInfo.Text.VeryLarge
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

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() ||
                InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (textSprite.IsTyping)
                {
                    textSprite.StopTyping();
                }
                else
                {
                    DisposeSound();
                    CanClose = true;

                    if (session.AwaitingScript?.NextStatement is not EchoCommand)
                        Game.SceneManager.Pop();
                }

                return true;
            }

            return false;
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

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            background.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            subjectSprite.Draw(gameTime);
            textSprite.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (allowSkip && HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);
            background.Opacity = textSprite.Opacity;

            if (!CanClose && !allowSkip && soundEffectInstance?.State == SoundState.Stopped)
            {
                DisposeSound();
                opacityTween.Start(TweenStyle.CubicIn, 1, 0, 500, () => Game.SceneManager.Pop());
                textSprite.Tweens.OpacityTween = opacityTween;
            }
        }

        #endregion

        // CanClose
        public bool CanClose { get; private set; }

        // Show
        public void Show(string text, string? soundName)
        {
            var index = text.IndexOf("|");
            if (index > 0)
                this.subjectSprite.Text = text.Substring(0, index);
            else
                this.subjectSprite.Clear();

            this.textSprite.Text = index == -1 ? text : text.Substring(index + 1);

            if (!string.IsNullOrWhiteSpace(soundName))
                this.textSprite.Color = ColorPalette.Text.Yellow * .8f;
            else if (index >= 0)
                this.textSprite.Color = ColorPalette.Text.Orange * .8f;
            else
                this.textSprite.Color = ColorPalette.Text.TerraLight * .8f;

            if (subjectSprite.Length > 0)
                subjectSprite.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.LeftTop);

            DisposeSound();

            MouseCursor.Tooltip = null;

            CanClose = false;

            //textSprite.StartTyping();

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

            background.Opacity = 0;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = opacityTween;

            MouseCursor.CustomImage = null;
            MouseCursor.Icon = this.allowSkip ? MouseCursorIcon.Talk : MouseCursorIcon.Wait;

            this.allowSkip = soundEffect == null;
        }
    }
}
