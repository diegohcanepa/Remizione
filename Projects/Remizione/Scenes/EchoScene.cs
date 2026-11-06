using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.Scripting;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// EchoScene
    /// </summary>
    public sealed class EchoScene : SessionOverlayScene
    {
        #region Private fields

        private readonly Sprite background = new(Atlases.UI.EchoBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly TextSprite graceAmountText;
        private readonly Sprite graceIcon;
        private SoundInstance? speakerSoundInstance;
        private readonly FloatTween textOpacityTween = new();
        private readonly TextSprite textSprite;
        private readonly TextSprite titleSprite;

        #endregion

        #region Constructor

        // Constructor
        public EchoScene(GameSession session)
            : base(session)
        {
            this.PausePreviousScenes = false;

            // Title
            this.titleSprite = new(Fonts.Common)
            {
                Color = Colors.Palette.Text,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Text sprite
            this.textSprite = new(Fonts.Common)
            {
                Color = Colors.Palette.TextDark,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PivotOrigin = RectanglePoint.Top,
                Position = background.BoundingBox.GetPoint(RectanglePoint.Top, 0, 20),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Grace icon
            this.graceIcon = new(Atlases.UI.GraceIcon)
            {
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Grace amount text
            this.graceAmountText = new(Fonts.Common)
            {
                Color = Colors.Palette.SteelBlue,
                PivotOrigin = RectanglePoint.Bottom,
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

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() ||
                InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                Step();
                return true;
            }

            return false;
        }

        // Step
        private void Step()
        {
            CanClose = true;
            if (Session.AwaitingScript?.NextStatement is not EchoCommand and not AwaitCommand)
                Game.SceneManager.Pop();
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
            titleSprite.Draw(gameTime);

            if (!graceAmountText.IsEmpty)
            {
                graceIcon.Draw(gameTime);
                graceAmountText.Draw(gameTime);
            }

            textSprite.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (InputBindings.Back.IsPressed(PlayerIndex.One))
            {
                Step();
                return HandleInputResult.Handled;
            }

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);
            titleSprite.Opacity = textSprite.Opacity;
            graceAmountText.Opacity = textSprite.Opacity;
            graceIcon.Opacity = textSprite.Opacity;
            background.Update(gameTime);
        }

        #endregion

        // CanClose
        public bool CanClose { get; private set; }

        // Show
        public void Show(GameThing target, string displayText)
        {
            this.textSprite.Text = displayText;
            this.titleSprite.Opacity = 0;
            this.titleSprite.Text = target.Label;
            this.titleSprite.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.Top);

            textOpacityTween.Start(TweenStyle.CubicIn, 0, 1, 500);
            textSprite.Tweens.OpacityTween = textOpacityTween;

            if (target.GraceCost > 0)
            {
                graceIcon.Position = titleSprite.BoundingBox.GetPoint(RectanglePoint.Right, 5, -1);
                graceAmountText.Position = titleSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom, 8 + graceIcon.BoundingBox.Width, 0);
                graceAmountText.Text = target.GraceCost.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                graceAmountText.Clear();
            }

            CanClose = false;
        }
    }
}
