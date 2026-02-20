using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// AutoSaveAdviceScene
    /// </summary>
    public sealed partial class AutoSaveAdviceScene : Scene
    {
        private int duration = 6000;
        private readonly ScaryCastleGame game;
        private readonly ImageSprite icon;
        private readonly int slotNumber;
        private readonly TextSprite text;
        private const int transitionOutDuration = 500;

        #region Constructor

        // Constructor
        public AutoSaveAdviceScene(ScaryCastleGame game, int slotNumber)
            : base(game)
        {
            this.game = game;
            this.PausePreviousScenes = true;
            this.slotNumber = slotNumber;

            BackgroundColor = Color.Black;

            text = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.TextWhite,
                MaximumWidth = 250,
                PivotOrigin = RectanglePoint.Center,
                Position = new Vector2(Screen.Center.X, 100),
                Text = TextRepository.GetValue("@Menu.Messages.AutoSaveAdvice"),
                Scale = ScaleInfo.Text.Medium
            };

            icon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = text.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 5)
            };
            icon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .5f, 300, -1);
        }

        #endregion

        #region Private members

        // StartSession
        private void StartSession()
        {
            Game.SceneManager.Pop();
            game.StartSession(slotNumber);
            TransitionManager.CurrentTransition.Out(1000);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            text.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            text.Update(gameTime);
            icon.Update(gameTime);

            if (duration > 0)
            {
                duration -= gameTime.ElapsedGameTime.Milliseconds;

                if (duration <= 0)
                {
                    InputManager.Suspend(transitionOutDuration);
                    TransitionManager.CurrentTransition.In(transitionOutDuration, StartSession);
                }
            }
        }

        #endregion
    }
}
