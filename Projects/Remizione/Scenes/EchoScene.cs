using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
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

        private readonly Sprite background = new(Atlases.UI.EchoBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly GameSession session;
        private SoundInstance? speakerSoundInstance;
        private readonly FloatTween textOpacityTween = new();
        private readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        public EchoScene(GameSession session)
            : base()
        {
            this.session = session;
            this.PausePreviousScenes = false;

            // Text sprite
            this.textSprite = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Terra,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PivotOrigin = RectanglePoint.Top,
                Position = background.BoundingBox.GetPoint(RectanglePoint.Top, 0, 20),
                Scale = ScaleInfo.Text.ExtraLarge
            };
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            if (!textSprite.IsTyping)
                speakerSoundInstance?.Stop(400);

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() ||
                InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (textSprite.IsTyping)
                {
                    textSprite.StopTyping();
                }
                else
                {
                    CanClose = true;
                    if (session.AwaitingScript?.NextStatement is not EchoCommand and not AwaitCommand)
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
            MouseCursor.Icon = MouseCursorIcon.Talk;
            MouseCursor.Tooltip = null;
            MouseCursor.CustomImage = null;
        }

        // OnDeactivate
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            speakerSoundInstance?.Stop();
            speakerSoundInstance = null;
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

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);
            background.Update(gameTime);
        }

        #endregion

        // CanClose
        public bool CanClose { get; private set; }

        // Show
        public void Show(string text)
        {
            this.textSprite.Text = text;
            this.textSprite.Color = ColorPalette.Text.TerraLight * .8f;

            CanClose = false;

            textSprite.StartTyping();

            textOpacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = textOpacityTween;
        }
    }
}
