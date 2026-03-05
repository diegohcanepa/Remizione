using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// SplashScene
    /// </summary>
    public partial class SplashScene : Scene
    {
        #region Private fields

        private const int defaultDuration = 1500;
        private readonly Sprite gradientSpot;
        private readonly Timer gradientSpotStartTimer;

        #endregion

        #region Constructors

        // Constructor
        public SplashScene(ScaryCastleGame game)
            : this(game, null, Vector2.Zero, 0, 0)
        {
        }

        // Constructor
        public SplashScene(ScaryCastleGame game, AtlasImage? image, Vector2 imagePosition, float imageScale)
            : this(game, image, imagePosition, imageScale, defaultDuration)
        {
        }

        // Constructor
        public SplashScene(ScaryCastleGame game, AtlasImage? image, Vector2 imagePosition, float imageScale, int duration)
            : base(game)
        {
            this.ExclusiveDraw = true;

            BackgroundColor = Color.Black;

            Image = new Sprite(game, image)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = imagePosition,
                Scale = new Vector2(imageScale)
            };

            gradientSpot = new Sprite(game, Atlases.Menu.FadeCircle)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new Vector2(1.8f)
            };

            Duration = duration;

            this.gradientSpotStartTimer = new Timer(ShowGradientSpot);
        }

        #endregion

        #region Private members

        // SetNextScene
        private void SetNextScene()
        {
            if (NextScene != null)
            {
                Game.SceneManager.Pop();
                Game.SceneManager.Push(NextScene);
            }
        }

        // ShowGradientSpot
        private void ShowGradientSpot()
        {
            TransitionManager.CurrentTransition.Out(FadeDuration);
            gradientSpot.Position = Screen.HUDArea.GetPoint(RectanglePoint.Left);
            gradientSpot.Tweens.XTween = FloatTween.Create(TweenStyle.CubicIn, gradientSpot.X, 240, 3000, GoToNextScene);
        }

        #endregion

        #region Protected members

        // FadeDuration 
        protected int FadeDuration { get; set; } = 1000;

        // GoToNextScene
        protected void GoToNextScene()
        {
            GoToNextScene(false);
        }

        // GoToNextScene
        protected void GoToNextScene(bool immediate)
        {
            TransitionManager.CurrentTransition.In(immediate ? 1 : FadeDuration, SetNextScene);
        }

        // Image
        protected Sprite Image { get; }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            Image.Draw(gameTime);

            if (!Image.IsEmpty)
            {
                gradientSpot.Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            TransitionManager.CurrentTransition.In(0);

            if (Image.IsEmpty)
            {
                TransitionManager.CurrentTransition.Out(FadeDuration);
            }
            else
            {
                Image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, Image.Scale, Image.Scale * 1.3f, 6000);
                gradientSpotStartTimer.Start(1000);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            gradientSpot.Update(gameTime);
            gradientSpotStartTimer.Update(gameTime);
            Image.Update(gameTime);
        }

        #endregion

        // Duration
        public int Duration { get; }

        // NextScene
        public Scene? NextScene { get; set; }
    }
}
