using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// PopupScene
    /// </summary>
    public sealed partial class PopupScene : Scene
    {
        #region Private fields

        private readonly InputBinding[] bindings;
        private readonly UIControl[] buttons;
        private readonly ImageSprite container;
        private readonly ImageSprite shadow;
        private readonly TextSprite text;
        private readonly TextSprite titleSprite;

        #endregion

        // Constructor
        public PopupScene(GameSession session, string title, string text, params InputBinding[] inputBindings)
            : base(session.Game)
        {
            this.bindings = inputBindings;

            this.container = new ImageSprite(session.Game, Atlases.UI.PopupContainer) { PivotOrigin = RectanglePoint.Middle, Position = Screen.Area.Center.ToVector2() };
            this.shadow = new ImageSprite(session.Game, Atlases.UI.PopupContainerShadow) { Position = container.BoundingBox.GetPoint(RectanglePoint.LeftTop, 7, 7) };

            this.titleSprite = new TextSprite(session.Game, Fonts.Main) { Color = ColorPalette.PopupTitle, PivotOrigin = RectanglePoint.Top, Position = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 12), Scale = ScaleInfo.PopupTitle, Text = title };
            this.text = new TextSprite(session.Game, Fonts.Main) { Color = ColorPalette.TextWhite, MaximumWidth = 220, PivotOrigin = RectanglePoint.Top, Position = titleSprite.BoundingBox.GetPoint(RectanglePoint.Bottom), Scale = ScaleInfo.PopupText, Text = text };

            buttons = new UIControl[inputBindings.Length];
            for (var i = 0; i < inputBindings.Length; i++)
            {
                buttons[i] = new UIControl(Game, inputBindings[i]) { PivotOrigin = RectanglePoint.RightTop };
            }
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            //DrawBackgroundShade();
            container.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            titleSprite.Draw(gameTime);
            text.Draw(gameTime);
            shadow.Draw(gameTime);
            Game.SpriteBatch.End();

            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Draw(gameTime);
            }

            base.OnDraw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].TestPressed(0))
                {
                    Result = bindings[i];
                    SceneController.Pop();
                    return HandleInputResult.Handled;
                }
            }

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            container.Position = Screen.Area.Center.ToVector2();

            var pos = container.BoundingBox.GetPoint(RectanglePoint.RightBottom, -6, 4);
            for (var i = buttons.Length - 1; i >= 0; i--)
            {
                buttons[i].Position = pos;
                pos.X -= buttons[i].BoundingBox.Width + 26;
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            text.Update(gameTime);
            titleSprite.Update(gameTime);

            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Update(gameTime);
            }
        }

        #endregion

        // Result
        public InputBinding? Result { get; private set; }
    }
}
