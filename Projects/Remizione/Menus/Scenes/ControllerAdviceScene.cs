using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.Menus
{
    public sealed partial class ControllerAdviceScene : MenuScene
    {
        private readonly Sprite image;
        private readonly RemizioneGame game;
        private readonly TextSprite message;
        private int nextSceneCooldown;

        // Constructor
        public ControllerAdviceScene(RemizioneGame game)
            : base(game, null)
        {
            this.game = game;

            BackgroundColor = Color.Black;

            // Image
            image = new(Atlases.Menu.ControllerAdvice)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = new Vector2(Screen.Center.X, 0),
                Scale = new Vector2(.23f)
            };

            // Message
            message = new(Fonts.Common)
            {
                Color = ColorPalette.TextWhite,
                PivotOrigin = RectanglePoint.Top,
                Position = image.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 15),
                Scale = ScaleInfo.Text.ExtraLarge,
                Text = "@Menu.Messages.ControllerAdvice"
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            image.Draw(gameTime);
            message.Draw(gameTime);
            game.SpriteBatch.End();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            TransitionManager.CurrentTransition.Out(TransitionDuration);
            nextSceneCooldown = 3000;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (nextSceneCooldown >= 0)
            {
                nextSceneCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (nextSceneCooldown <= 0)
                {
                    GoToScene(new TitleMenuScene(game), 1000);
                }
            }
        }

        #endregion
    }
}