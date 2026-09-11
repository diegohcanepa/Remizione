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
        private readonly ContentManager content;
        private bool fade;
        private readonly Sprite background = new(Atlases.UI.EchoBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly FloatTween opacityTween = new();
        private readonly GameSession session;
        private SoundEffect? soundEffect;
        private SoundEffectInstance? soundEffectInstance;
        private readonly TextSprite textSprite;

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
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Top,
                Position = background.BoundingBox.GetPoint(RectanglePoint.Top, 0, 20),
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #endregion

        #region Private members

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
                    if (soundEffectInstance != null)
                    {
                        soundEffectInstance.Stop();
                        soundEffectInstance.Dispose();
                        soundEffectInstance = null;
                    }

                    if (soundEffect != null)
                    {
                        soundEffect.Dispose();
                        soundEffect = null;
                    }

                    CanClose = true;

                    if (session.AwaitingScript?.NextStatement is not EchoCommand)
                        Game.SceneManager.Pop();
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            MouseCursor.Tooltip = null;
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

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            MouseCursor.Icon = MouseCursorIcon.Talk;
            fade = true;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);
            background.Opacity = textSprite.Opacity;
        }

        #endregion

        // CanClose
        public bool CanClose { get; private set; }

        // Show
        public void Show(string text, string? soundName)
        {
            CanClose = false;
            textSprite.Text = text;
            textSprite.StartTyping();

            if (!string.IsNullOrWhiteSpace(soundName))
            {
                var filePath = Sound.EncodeAssetName(AudioManager.VoiceCategory, soundName);
                content.Load<SoundEffect>(filePath);
                soundEffect = content.Load<SoundEffect>(filePath);
                soundEffectInstance = soundEffect.CreateInstance();
                soundEffectInstance.Volume = AudioManager.VoiceCategory.Volume.Effective;
                soundEffectInstance.Play();
            }

            if (fade)
            {
                background.Opacity = 0;
                opacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
                textSprite.Tweens.OpacityTween = opacityTween;
                fade = false;
            }
        }
    }
}
